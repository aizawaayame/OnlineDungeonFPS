using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ProjectileStandard : ProjectileBase
{

    #region Const
    const QueryTriggerInteraction TRIGGER_INTERACTION = QueryTriggerInteraction.Collide;
    #endregion

    #region Fields

    [SerializeField] float radius = 0.1f;
    [SerializeField] Transform root;
    [SerializeField] Transform tip;
    [SerializeField] float maxLifeTime = 5f;
    [SerializeField] GameObject impactVfx;
    [SerializeField] float impactVfxLifetime = 5f;
    [SerializeField] float impactVfxSpawnOffset = 0.1f;
    [SerializeField] AudioClip impactSfxClip;
    [SerializeField] LayerMask hittableLayers = -1;
    [SerializeField] float speed = 20f;
    [SerializeField] float gravityDownAcceleration = 0f;
    [SerializeField] float trajectoryCorrectionDistance = -1;
    [SerializeField] bool inheritWeaponVelocity = false;
    [SerializeField] float damage = 40f;
    [SerializeField] DamageArea areaOfDamage;

    ProjectileBase projectileBase;
    Vector3 lastRootPosition;
    Vector3 velocity;
    bool hasTrajectoryOverride;
    float shootTime;
    Vector3 trajectoryCorrectionVector;
    Vector3 consumedTrajectoryCorrectionVector;
    List<Collider> ignoredColliders;

    #endregion
    
    void OnEnable()
    {
        projectileBase = GetComponent<ProjectileBase>();
        projectileBase.onShoot += OnShoot;
        Destroy(gameObject, maxLifeTime);
    }

    new void OnShoot()
    {
        shootTime = Time.time;
        lastRootPosition = root.position;
        velocity = transform.forward * speed;
        ignoredColliders = new List<Collider>();
        transform.position += projectileBase.InheritedMuzzleVelocity * Time.deltaTime;
        
        Collider[] ownerColliders = projectileBase.Owner.GetComponentsInChildren<Collider>();
        ignoredColliders.AddRange(ownerColliders);

        PlayerWeaponController playerWeaponsManager = projectileBase.Owner.GetComponent<PlayerWeaponController>();
        if (playerWeaponsManager)
        {
            hasTrajectoryOverride = true;

            Vector3 cameraToMuzzle = (projectileBase.InitialPosition -
                                      playerWeaponsManager.WeaponCamera.transform.position);

            trajectoryCorrectionVector = Vector3.ProjectOnPlane(-cameraToMuzzle,
                playerWeaponsManager.WeaponCamera.transform.forward);
            if (trajectoryCorrectionDistance == 0)
            {
                transform.position += trajectoryCorrectionVector;
                consumedTrajectoryCorrectionVector = trajectoryCorrectionVector;
            }
            else if (trajectoryCorrectionDistance < 0)
            {
                hasTrajectoryOverride = false;
            }

            if (Physics.Raycast(playerWeaponsManager.WeaponCamera.transform.position, cameraToMuzzle.normalized,
                out RaycastHit hit, cameraToMuzzle.magnitude, hittableLayers, TRIGGER_INTERACTION))
            {
                if (IsHitValid(hit))
                {
                    OnHit(hit.point, hit.normal, hit.collider);
                }
            }
        }
    }

    void Update()
    {

        transform.position += velocity * Time.deltaTime;
        if (inheritWeaponVelocity)
        {
            transform.position += projectileBase.InheritedMuzzleVelocity * Time.deltaTime;
        }
        
        if (hasTrajectoryOverride && consumedTrajectoryCorrectionVector.sqrMagnitude <
            trajectoryCorrectionVector.sqrMagnitude)
        {
            Vector3 correctionLeft = trajectoryCorrectionVector - consumedTrajectoryCorrectionVector;
            float distanceThisFrame = (root.position - lastRootPosition).magnitude;
            Vector3 correctionThisFrame =
                (distanceThisFrame / trajectoryCorrectionDistance) * trajectoryCorrectionVector;
            correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
            consumedTrajectoryCorrectionVector += correctionThisFrame;
            
            if (consumedTrajectoryCorrectionVector.sqrMagnitude == trajectoryCorrectionVector.sqrMagnitude)
            {
                hasTrajectoryOverride = false;
            }

            transform.position += correctionThisFrame;
        }
        
        transform.forward = velocity.normalized;

        if (gravityDownAcceleration > 0)
        {
            velocity += Vector3.down * gravityDownAcceleration * Time.deltaTime;
        }

        // Hit detection
        {
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            bool foundHit = false;

            Vector3 displacementSinceLastFrame = tip.position - lastRootPosition;
            RaycastHit[] hits = Physics.SphereCastAll(lastRootPosition, radius,
                displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, hittableLayers,
                TRIGGER_INTERACTION);
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
                if (closestHit.distance <= 0f)
                {
                    closestHit.point = root.position;
                    closestHit.normal = -transform.forward;
                }
                OnHit(closestHit.point, closestHit.normal, closestHit.collider);
            }
        }

        lastRootPosition = root.position;
    }

    bool IsHitValid(RaycastHit hit)
    {

        if (hit.collider.isTrigger && hit.collider.GetComponent<DamageableModule>() == null)
        {
            return false;
        }

        if (ignoredColliders != null && ignoredColliders.Contains(hit.collider))
        {
            return false;
        }
        return true;
    }

    void OnHit(Vector3 point, Vector3 normal, Collider collider)
    {
        // damage
        if (areaOfDamage)
        {
            // area damage
            areaOfDamage.InflictDamageInArea(damage, point, hittableLayers, TRIGGER_INTERACTION,
                projectileBase.Owner);
        }
        else
        {
            // point damage
            DamageableModule damageable = collider.GetComponent<DamageableModule>();
            if (damageable)
            {
                damageable.InflictDamage(damage, projectileBase.Owner);
            }
        }
        // impact vfx
        if (impactVfx)
        {
            GameObject impactVfxInstance = Instantiate(impactVfx, point + (normal * impactVfxSpawnOffset),
                Quaternion.LookRotation(normal));
            if (impactVfxLifetime > 0)
            {
                Destroy(impactVfxInstance.gameObject, impactVfxLifetime);
            }
        }
        // Self Destruct
        Destroy(this.gameObject);
    }
}

