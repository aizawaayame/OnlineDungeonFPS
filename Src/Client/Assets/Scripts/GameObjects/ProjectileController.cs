using System;
using System.Collections.Generic;
using Entities;
using log4net.Repository.Hierarchy;
using UnityEngine;

namespace GameObjects
{
    public class ProjectileController : MonoBehaviour
    {
        
        #region Public Fields
        [Header("General")]
        public float radius = 0.01f;

        public Transform rootTransform;

        public Transform tipTransform;
        
        public float maxLifeTime = 5f;

        public GameObject impactVfx;

        public float impactVfxLifetime = 5f;

        public float impactVfxSpawnOffset = 0.1f;
        
        public LayerMask HittableLayers = -1;
        
        [Header("Movement")]
        public float speed = 20f;
        
        public float gravityDownAcceleration = 0f;
        
        public float trajectoryCorrectionDistance = -1;
        #endregion

        #region Public Properties
        public WeaponController WeaponController { get; private set; }
        public Character OwnerCharacter
        {
            get => WeaponController.OwnerCharacter;
        }

        public GameObject OwnerGameObject
        {
            get => WeaponController.OwnerGameObject;
        }
        
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 InheritedMuzzleVelocity { get; private set; }
        public float InitialCharge { get; private set; }
        /*public float Damage
        {
            get=>
        }*/

        #endregion

        #region Private Fields

        Vector3 lastRootPosition;
        Vector3 velocity;
        List<Collider> ignoredColliders;
        #endregion
        #region Private Methods

        void OnEnable()
        {
            Destroy(gameObject,maxLifeTime);
        }

        void OnShoot()
        {
            lastRootPosition = rootTransform.position;
            velocity = transform.forward * speed;
            ignoredColliders = new List<Collider>();
            transform.position = InheritedMuzzleVelocity * Time.deltaTime;
            
            // Ignore colliders of owner
            Collider[] ownerColliders = OwnerGameObject.GetComponentsInChildren<Collider>();
            ignoredColliders.AddRange(ownerColliders);

        }

        void Update()
        {
            // Move
            transform.position += velocity * Time.deltaTime;
            transform.forward = velocity.normalized;
            // Gravity
            if (gravityDownAcceleration > 0)
            {
                velocity += Vector3.down * gravityDownAcceleration * Time.deltaTime;
            }
            // Hit detection
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            bool foundHit = false;
            // Sphere cast
            Vector3 displacementSinceLastFrame = tipTransform.position - lastRootPosition;
            RaycastHit[] hits = Physics.SphereCastAll(lastRootPosition, radius,
                displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, HittableLayers, QueryTriggerInteraction.Collide);
            foreach (var hit in hits)
            {
                if (IsHitValid(hit) && hit.distance < closestHit.distance)
                {
                    foundHit = true;
                    closestHit = hit;
                }
            }

            if (foundHit)
            {
                // Handle case of casting while already inside a collider
                if (closestHit.distance <= 0f)
                {
                    closestHit.point = rootTransform.position;
                    closestHit.normal = -transform.forward;
                }
                /*OnHit(closestHit.point, closestHit.normal, closestHit.collider);*/
            }
            lastRootPosition = rootTransform.position;

        }

        bool IsHitValid(RaycastHit hit)
        {
            return true;
        }

        #endregion
        #region Public Methods

        public void Shoot(WeaponController weaponController)
        {
            this.WeaponController = weaponController;
            InitialPosition = transform.position;
            InitialDirection = transform.forward;
            InheritedMuzzleVelocity = weaponController.MuzzleWorldVelocity;
            InitialCharge = weaponController.CurrentCharge;

            OnShoot();
        }

        #endregion
    }
}
