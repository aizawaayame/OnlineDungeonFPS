using System;
using System.Runtime.CompilerServices;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;
using Managers;
using Models;
using Protocol.Message;
using Services;

[RequireComponent(typeof(CharacterController), typeof(AudioSource),typeof(HealthController))]
public class PlayerController : MonoBehaviour
{

    public UnityAction<bool> onStanceChanged;

    #region Const

    const float GRAVITY_DOWN_FORCE = 20f;
    readonly LayerMask GRAVITY_CHECK_LAYER = -1;
    const float GROUND_CHECK_DISTANCE = 0.05f;
    const float MAX_SPEED_ON_GROUND = 10f;
    const float MOVEMENT_SHARPNESS_ON_GROUND = 15;
    const float MAX_SPEED_CROUCHED_RATIO = 0.5f;
    const float MAX_SPEED_IN_AIR = 10f;
    const float ACCELARATION_SPEED_IN_AIR = 25f;
    const float SPRINT_SPEED_MODIFIER = 2f;
    const float ROTATION_SPEED = 200f;
    const float AIM_ROTATION_MULTIPLIER = 0.4f;
    const float JUMP_FORCE = 9f;
    const float CAMERA_HEIGHT_RATIO = 0.9f;
    const float CAPSULE_HEIGHT_STANDING = 1.8f;
    const float CAPUSULE_HEIGHT_CROUCHING = 0.9f;
    const float CROUCHING_SHARPNESS = 10f;
    const float JUMP_GROUNDING_PREVENTION_TIME = 0.2f;
    const float GOUNDING_CHECK_DISTANCE_IN_AIR = 0.07f;

    #endregion

    #region Fields

    [SerializeField] AudioSource audioSource;
    [SerializeField] Camera mainCamera;
    [SerializeField] PlayerWeaponController playerWeaponController;
    [SerializeField] float footstepSfxFrequency = 1f;
    [SerializeField] float FootstepSfxFrequencyWhileSprinting = 1f;
    [SerializeField] AudioClip footstepSfx;
    [SerializeField] AudioClip jumpSfx;
    [SerializeField] AudioClip landSfx;
    CharacterController charController;
    Actor actor;
    HealthController health;
    Vector3 groundNormal;
    Vector3 characterVelocity;
    Vector3 lastImpactSpeed;
    float lastTimeJumped = 0f;
    float cameraVerticalAngle = 0f;
    float footstepDistanceCounter;
    float targetCharacterHeight;

    #endregion

    #region Properties

    public Vector3 CharacterVelocity { get; set; }
    public bool IsGrounded { get; private set; }
    public bool HasJumpedThisFrame { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsCrouching { get; private set; }

    public float RotationMultiplier
    {
        get
        {
            if (playerWeaponController.IsAiming)
            {
                return AIM_ROTATION_MULTIPLIER;
            }

            return 1f;
        }
    }

    public float MaxSpeedOnGound { get => MAX_SPEED_ON_GROUND; }
    public float SprintSpeedModifier { get => SPRINT_SPEED_MODIFIER; }
    public Camera MainCamera { get => mainCamera; }
    public Camera WeaponCamera { get => playerWeaponController.WeaponCamera; }

    #endregion

    void Start()
    {
        ActorManager.Instance.SetPlayer(gameObject);
        charController = GetComponent<CharacterController>();
        actor = GetComponent<Actor>();
        health = GetComponent<HealthController>();
        health.onDie += OnDie;
        charController.enableOverlapRecovery = true;
        SetCrouchingState(false, true);
        UpdateCharacterHeight(true);
    }
    void Update()
    {
        HasJumpedThisFrame = false;
        bool wasGrounded = IsGrounded;
        GroundCheck();
        if (IsGrounded && !wasGrounded)
        {
            audioSource.PlayOneShot(landSfx);
        }
        // crouching
        if (PlayerInputManager.Instance.GetCrouchInputDown())
        {
            SetCrouchingState(!IsCrouching, false);
        }
        UpdateCharacterHeight(false);
        HandleCharacterMovement();
        SendEntitySync();
    }

    void OnDie()
    {
        IsDead = true;
        playerWeaponController.SwitchToWeaponIndex(-1, true);
        EventManager.Broadcast(Events.PlayerDeathEvent);
    }

    void GroundCheck()
    {
        float chosenGroundCheckDistance =
            IsGrounded ? (charController.skinWidth + GROUND_CHECK_DISTANCE) : GOUNDING_CHECK_DISTANCE_IN_AIR;
        IsGrounded = false;
        groundNormal = Vector3.up;
        if (Time.time >= lastTimeJumped + JUMP_GROUNDING_PREVENTION_TIME)
        {
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(charController.height),
                charController.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, GRAVITY_CHECK_LAYER,
                QueryTriggerInteraction.Ignore))
            {
                groundNormal = hit.normal;
                if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                    IsNormalUnderSlopeLimit(groundNormal))
                {
                    IsGrounded = true;
                    if (hit.distance > charController.skinWidth)
                    {
                        charController.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    void HandleCharacterMovement()
    {
        {
            transform.Rotate(
                new Vector3(0f, (PlayerInputManager.Instance.GetLookInputsHorizontal() * ROTATION_SPEED * RotationMultiplier),
                    0f), Space.Self);
        }
        {

            cameraVerticalAngle += PlayerInputManager.Instance.GetLookInputsVertical() * ROTATION_SPEED * RotationMultiplier;
            cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);
            mainCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);
        }

        bool isSprinting = PlayerInputManager.Instance.GetSprintInputHeld();
        {
            if (isSprinting)
            {
                isSprinting = SetCrouchingState(false, false);
            }
            float speedModifier = isSprinting ? SPRINT_SPEED_MODIFIER : 1f;
            Vector3 worldspaceMoveInput = transform.TransformVector(PlayerInputManager.Instance.GetMoveInput());
            if (IsGrounded)
            {
                Vector3 targetVelocity = worldspaceMoveInput * MAX_SPEED_ON_GROUND * speedModifier;
                // reduce speed if crouching by crouch speed ratio
                if (IsCrouching)
                    targetVelocity *= MAX_SPEED_CROUCHED_RATIO;
                targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, groundNormal) *
                                 targetVelocity.magnitude;
                CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                    MOVEMENT_SHARPNESS_ON_GROUND * Time.deltaTime);

                if (IsGrounded && PlayerInputManager.Instance.GetJumpInputDown())
                {
                    if (SetCrouchingState(false, false))
                    {
                        CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);
                        CharacterVelocity += Vector3.up * JUMP_FORCE;
                        audioSource.PlayOneShot(jumpSfx);
                        lastTimeJumped = Time.time;
                        HasJumpedThisFrame = true;
                        IsGrounded = false;
                        groundNormal = Vector3.up;
                    }
                }

                // footsteps sound
                float chosenFootstepSfxFrequency =
                    (isSprinting ? FootstepSfxFrequencyWhileSprinting : footstepSfxFrequency);
                if (footstepDistanceCounter >= 1f / chosenFootstepSfxFrequency)
                {
                    footstepDistanceCounter = 0f;
                    audioSource.PlayOneShot(footstepSfx);
                }
                footstepDistanceCounter += CharacterVelocity.magnitude * Time.deltaTime;
            }
            else
            {

                CharacterVelocity += worldspaceMoveInput * ACCELARATION_SPEED_IN_AIR * Time.deltaTime;
                float verticalVelocity = CharacterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MAX_SPEED_IN_AIR * speedModifier);
                CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
                CharacterVelocity += Vector3.down * GRAVITY_DOWN_FORCE * Time.deltaTime;
            }
        }
        Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(charController.height);
        charController.Move(CharacterVelocity * Time.deltaTime);
        lastImpactSpeed = Vector3.zero;
        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, charController.radius,
            CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
            QueryTriggerInteraction.Ignore))
        {
            lastImpactSpeed = CharacterVelocity;
            CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
        }
    }
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= charController.slopeLimit;
    }
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * charController.radius);
    }
 
    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - charController.radius));
    }
    
    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }

    void UpdateCharacterHeight(bool force)
    {
        if (force)
        {
            charController.height = targetCharacterHeight;
            charController.center = Vector3.up * charController.height * 0.5f;
            mainCamera.transform.localPosition = Vector3.up * targetCharacterHeight * CAMERA_HEIGHT_RATIO;
            actor.AimPoint.transform.localPosition = charController.center;
        }
        else if (charController.height != targetCharacterHeight)
        {
            charController.height = Mathf.Lerp(charController.height, targetCharacterHeight,
                CROUCHING_SHARPNESS * Time.deltaTime);
            charController.center = Vector3.up * charController.height * 0.5f;
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition,
                Vector3.up * targetCharacterHeight * CAMERA_HEIGHT_RATIO, CROUCHING_SHARPNESS * Time.deltaTime);
            actor.AimPoint.transform.localPosition = charController.center;
        }
    }
    bool SetCrouchingState(bool crouched, bool ignoreObstructions)
    {
        if (crouched)
        {
            targetCharacterHeight = CAPUSULE_HEIGHT_CROUCHING;
        }
        else
        {
            if (!ignoreObstructions)
            {
                Collider[] standingOverlaps = Physics.OverlapCapsule(
                    GetCapsuleBottomHemisphere(),
                    GetCapsuleTopHemisphere(CAPSULE_HEIGHT_STANDING),
                    charController.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);
                foreach (Collider c in standingOverlaps)
                {
                    if (c != charController)
                    {
                        return false;
                    }
                }
            }

            targetCharacterHeight = CAPSULE_HEIGHT_STANDING;
        }

        if (onStanceChanged != null)
        {
            onStanceChanged.Invoke(crouched);
        }

        IsCrouching = crouched;
        return true;
    }
    
    void SendEntitySync()
    {
        NEntity entitySync = User.Instance.CurrentCharacter.Entity;
        entitySync.Direction = GameObjectTool.WorldToLogicN(transform.forward);
        entitySync.Position = GameObjectTool.WorldToLogicN(transform.position);

        MapService.Instance.SendMapEntitySync(EntityEvent.None, User.Instance.CurrentCharacter.Entity);
    }
}
