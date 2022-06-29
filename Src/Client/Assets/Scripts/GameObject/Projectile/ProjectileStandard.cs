using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ProjectileStandard : ProjectileBase
{

    #region Const
    const QueryTriggerInteraction TRIGGER_INTERACTION = QueryTriggerInteraction.Collide;
    #endregion

    #region Fields

    public float Radius = 0.1f;
    public Transform Root;
    public Transform Tip;
    public float MaxLifeTime = 5f;
    public GameObject ImpactVfx;
    public float ImpactVfxLifetime = 5f;
    public float ImpactVfxSpawnOffset = 0.1f;
    public AudioClip ImpactSfxClip;
    public LayerMask HittableLayers = -1;
    public float Speed = 20f;
    public float GravityDownAcceleration = 0f;
    public float TrajectoryCorrectionDistance = -1;
    public bool InheritWeaponVelocity = false;
    public float Damage = 40f;
    public DamageArea AreaOfDamage;

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
        Destroy(gameObject, MaxLifeTime);
    }

    new void OnShoot()
    {
        shootTime = Time.time;
        lastRootPosition = Root.position;
        velocity = transform.forward * Speed;
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
            if (TrajectoryCorrectionDistance == 0)
            {
                transform.position += trajectoryCorrectionVector;
                consumedTrajectoryCorrectionVector = trajectoryCorrectionVector;
            }
            else if (TrajectoryCorrectionDistance < 0)
            {
                hasTrajectoryOverride = false;
            }

            if (Physics.Raycast(playerWeaponsManager.WeaponCamera.transform.position, cameraToMuzzle.normalized,
                out RaycastHit hit, cameraToMuzzle.magnitude, HittableLayers, TRIGGER_INTERACTION))
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
        if (InheritWeaponVelocity)
        {
            transform.position += projectileBase.InheritedMuzzleVelocity * Time.deltaTime;
        }
        
        if (hasTrajectoryOverride && consumedTrajectoryCorrectionVector.sqrMagnitude <
            trajectoryCorrectionVector.sqrMagnitude)
        {
            Vector3 correctionLeft = trajectoryCorrectionVector - consumedTrajectoryCorrectionVector;
            float distanceThisFrame = (Root.position - lastRootPosition).magnitude;
            Vector3 correctionThisFrame =
                (distanceThisFrame / TrajectoryCorrectionDistance) * trajectoryCorrectionVector;
            correctionThisFrame = Vector3.ClampMagnitude(correctionThisFrame, correctionLeft.magnitude);
            consumedTrajectoryCorrectionVector += correctionThisFrame;
            
            if (consumedTrajectoryCorrectionVector.sqrMagnitude == trajectoryCorrectionVector.sqrMagnitude)
            {
                hasTrajectoryOverride = false;
            }

            transform.position += correctionThisFrame;
        }
        
        transform.forward = velocity.normalized;

        if (GravityDownAcceleration > 0)
        {
            velocity += Vector3.down * GravityDownAcceleration * Time.deltaTime;
        }

        // Hit detection
        {
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            bool foundHit = false;

            Vector3 displacementSinceLastFrame = Tip.position - lastRootPosition;
            RaycastHit[] hits = Physics.SphereCastAll(lastRootPosition, Radius,
                displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, HittableLayers,
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
                    closestHit.point = Root.position;
                    closestHit.normal = -transform.forward;
                }
                OnHit(closestHit.point, closestHit.normal, closestHit.collider);
            }
        }

        lastRootPosition = Root.position;
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
        if (AreaOfDamage)
        {
            // area damage
            AreaOfDamage.InflictDamageInArea(Damage, point, HittableLayers, TRIGGER_INTERACTION,
                projectileBase.Owner);
        }
        else
        {
            // point damage
            DamageableModule damageable = collider.GetComponent<DamageableModule>();
            if (damageable)
            {
                damageable.InflictDamage(Damage, projectileBase.Owner);
            }
        }
        // impact vfx
        if (ImpactVfx)
        {
            GameObject impactVfxInstance = Instantiate(ImpactVfx, point + (normal * ImpactVfxSpawnOffset),
                Quaternion.LookRotation(normal));
            if (ImpactVfxLifetime > 0)
            {
                Destroy(impactVfxInstance.gameObject, ImpactVfxLifetime);
            }
        }
        // Self Destruct
        Destroy(this.gameObject);
    }
}

