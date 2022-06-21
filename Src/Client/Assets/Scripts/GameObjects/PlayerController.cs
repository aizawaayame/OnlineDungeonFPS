using System;
using Entities;
using Managers;
using Models;
using Protocol;
using Services;
using UnityEngine;

namespace GameObjects
{
    [RequireComponent(typeof(UnityEngine.CharacterController),typeof(EntityController))]
    public class PlayerController : MonoBehaviour
    {

        #region Const
        
        const float JUMP_GROUNDING_PREVENTION_TIME = 0.2f;
        const float GROUND_CHECK_DISTANCE = 0.05f;
        const float GROUND_CHECK_DISTANCE_IN_AIR = 0.07f;
        const float SPRINT_SPEED_MULTIPLIER = 1.4f;
        const float AIMING_ROTATION_MULTIPLIER = 0.4f;
        const float GRAVITY_FORCE = 50f;

        #endregion
        
        #region Public Fields and Properties

        public Camera mainCamera;
        public Camera weaponCamera;

        public EntityController EntityController { get; set; }
        public Character Character { get; set; }
        public float Speed { get { return Character.Define.Speed / 100; } }
        public float SpeedInAir { get { return Character.Define.SpeedInAir / 100; } } 
        
        [Tooltip("Rotation speed for moving the camera")]
        public float RotationSpeed = 200f;
        
        [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
        public float MovementSharpnessOnGround = 15;
        
        [Tooltip("Force applied upward when jumping")]
        public float JumpForce = 30f;
        #endregion

        #region Private Fields

        CharacterState state = CharacterState.Idle;
        UnityEngine.CharacterController controller;

        bool isGrounded = false;
        bool hasJumpedThisFrame = false;
        
        Vector3 groundNormal;
        Vector3 characterVelocity;
        float lastTimeJumped = 0f;
        float cameraVerticalAngle = 0f;
        
        #endregion

        #region Private Methods

        void Start()
        {
            controller = GetComponent<UnityEngine.CharacterController>();
            Debug.Log($"初始化的Vector3Int坐标为{Character.Position.x},{Character.Position.y},{Character.Position.z}");
            Vector3Int posInt = new Vector3Int(){
                x = DataManager.Instance.MapDefines[Character.MapId].MapPosX,
                y = DataManager.Instance.MapDefines[Character.MapId].MapPosY,
                z = DataManager.Instance.MapDefines[Character.MapId].MapPosZ,
            };
            transform.position = posInt;
        }

        void Update()
        {
            hasJumpedThisFrame = false;
            GroundCheck();
            HandleCharacterMovement();
            Character.Direction = transform.forward;
            Character.Position = transform.position;
        }

        void FixedUpdate()
        {
            SendEntityEvent(EntityEvent.None);
        }
        void GroundCheck()
        {
            float groundCheckDistance =
                isGrounded ? controller.skinWidth + GROUND_CHECK_DISTANCE : GROUND_CHECK_DISTANCE_IN_AIR;
            isGrounded = false;
            groundNormal = Vector3.up;

            if (Time.time >= lastTimeJumped + JUMP_GROUNDING_PREVENTION_TIME)
            {
                if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(),GetCapsuleTopHemisphere(),
                        controller.radius,Vector3.down, out RaycastHit hit,groundCheckDistance,-1,
                        QueryTriggerInteraction.Ignore))
                {
                    groundNormal = hit.normal;
                    if (Vector3.Dot(hit.normal,transform.up) > 0f &&
                        IsNormalUnderSlopeLimit(groundNormal))
                    {
                        isGrounded = true;
                        //snapping to the ground
                        if (hit.distance > controller.skinWidth)
                        {
                            controller.Move(Vector3.down * hit.distance);
                        }
                    }
                }
            }
        }

        void HandleCharacterMovement()
        {
            // horizontal character rotation
            transform.Rotate(new Vector3(0f, PlayerInputController.Instance.GetLookInputsHorizontal() * RotationSpeed, 0f),
                    Space.Self);
          

            // vertical camera rotation
            cameraVerticalAngle += PlayerInputController.Instance.GetLookInputsVertical() * RotationSpeed;
            cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);
            mainCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0f, 0f);
            

            bool isSprinting = PlayerInputController.Instance.GetSprintInputHeld();
            float speedMultiplier = isSprinting ? SPRINT_SPEED_MULTIPLIER : 1f;

            Vector3 worldspaceMoveInput = transform.TransformVector(PlayerInputController.Instance.GetMoveInput());
            Vector3 targetVelocity = worldspaceMoveInput * Speed * speedMultiplier;
            targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, groundNormal) 
                             * targetVelocity.magnitude;
                
            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            this.characterVelocity = Vector3.Lerp(this.characterVelocity, targetVelocity,
                MovementSharpnessOnGround * Time.deltaTime);
            
            if (isGrounded)
            {
                // jumping 
                if (isGrounded && PlayerInputController.Instance.GetJumpInputDown())
                {
                    this.characterVelocity = new Vector3(this.characterVelocity.x, 0f, this.characterVelocity.z);
                    this.characterVelocity += Vector3.up * JumpForce;
                    lastTimeJumped = Time.time;
                    hasJumpedThisFrame = true;
                    isGrounded = false;
                    groundNormal = Vector3.up;
                }
            }
            else
            {
                // apply the gravity to the velocity
                this.characterVelocity += Vector3.down * GRAVITY_FORCE * Time.deltaTime;
            }
            
            // apply the final calculated velocity value as a character movement
            Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere();
            controller.Move(characterVelocity * Time.deltaTime);
            
            // detect obstructions to adjust velocity accordingly
            if (Physics.CapsuleCast(capsuleBottomBeforeMove,capsuleTopBeforeMove,controller.radius,
                    this.characterVelocity.normalized,out RaycastHit hit,this.characterVelocity.magnitude*Time.deltaTime,
                    -1,QueryTriggerInteraction.Ignore))
            {
                characterVelocity = Vector3.ProjectOnPlane(this.characterVelocity, hit.normal);
            }
        }
        bool IsNormalUnderSlopeLimit(Vector3 normal)
        {
            return Vector3.Angle(transform.up, normal) <= controller.slopeLimit;
        }
        
        /// <summary>
        /// Gets the center point of the bottom hemisphere of the character controller capsule 
        /// </summary>
        /// <returns></returns>
        Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * controller.radius);
        }
        
        /// <summary>
        /// // Gets the center point of the top hemisphere of the character controller capsule 
        /// </summary>
        /// <returns></returns>
        Vector3 GetCapsuleTopHemisphere()
        {
            return transform.position + (transform.up * (controller.height - controller.radius));
        }
    
        /// <summary>
        /// Gets a reoriented direction that is tangent to a given slope
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="slopeNormal"></param>
        /// <returns></returns>
        Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
        {
            Vector3 directionRight = Vector3.Cross(direction, transform.up);
            return Vector3.Cross(slopeNormal, directionRight).normalized;
        }

        void SendEntityEvent(EntityEvent entityEvent)
        {
            if (EntityController != null)   
            {
                
            }
            MapService.Instance.SendMapEntitySync(entityEvent,this.Character.NEntity);
        }
        #endregion
        
    }
}
