
using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Utilities;
using Random = UnityEngine.Random;

[RequireComponent(typeof(HealthController), typeof(Actor), typeof(NavMeshAgent))]
public class MonsterController : MonoBehaviour
{

    #region Internal Struct
    [System.Serializable]
    public struct RendererIndexData
    {
        public Renderer Renderer;
        public int MaterialIndex;

        public RendererIndexData(Renderer renderer, int index)
        {
            Renderer = renderer;
            MaterialIndex = index;
        }
    }
    #endregion
    
    public UnityAction onAttack;
    public UnityAction onDetectedTarget;
    public UnityAction onLostTarget;
    public UnityAction onDamaged;

    #region Fields
    
    [SerializeField] float SelfDestructYHeight = -20f;
    [SerializeField]  float PathReachingRadius = 2f;
    [SerializeField]  float OrientationSpeed = 10f;
    [SerializeField]  float DeathDuration = 0f;
    [SerializeField]  bool SwapToNextWeapon = false;
    [SerializeField]  float DelayAfterWeaponSwap = 0f;
    [SerializeField]  Material EyeColorMaterial;
    
    [SerializeField][ColorUsageAttribute(true, true)]  Color DefaultEyeColor;
    [SerializeField][ColorUsageAttribute(true, true)]  Color AttackEyeColor;
    
    [SerializeField]  Material BodyMaterial;
    [SerializeField]  Gradient OnHitBodyGradient;
    [SerializeField]  float FlashOnHitDuration = 0.5f;
    
    [SerializeField]  AudioClip DamageTick;
    [SerializeField]  GameObject DeathVfx;
    [SerializeField]  Transform DeathVfxSpawnPoint;

    [SerializeField]  GameObject LootPrefab;
    [SerializeField]  float DropRate = 1f;
    
    
    List<RendererIndexData> bodyRenderers = new List<RendererIndexData>();
    MaterialPropertyBlock bodyFlashMaterialPropertyBlock;
    float lastTimeDamaged = float.NegativeInfinity;
    RendererIndexData eyeRendererData;
    MaterialPropertyBlock eyeColorMaterialPropertyBlock;
    
    int pathDestinationNodeIndex;
    HealthController health;
    Actor actor;
    Collider[] selfColliders;
    bool wasDamagedThisFrame;
    float lastTimeWeaponSwapped = Mathf.NegativeInfinity;
    int currentWeaponIndex;
    WeaponController currentWeapon;
    WeaponController[] weapons;
    #endregion
    
    #region Properties

    public PatrolPath PatrolPath { get; set; }
    public GameObject KnownDetectedTarget => DetectionModule.KnownDetectedTarget;
    public bool IsTargetInAttackRange => DetectionModule.IsTargetInAttackRange;
    public bool IsSeeingTarget => DetectionModule.IsSeeingTarget;
    public bool HadKnownTarget => DetectionModule.HadKnownTarget;
    public NavMeshAgent NavMeshAgent { get; private set; }
    public DetectionModule DetectionModule { get; private set; }

    #endregion
    
    void Start()
    {
        MonsterManager.Instance.RegisterEnemy(this);

        health = GetComponent<HealthController>();
        actor = GetComponent<Actor>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        selfColliders = GetComponentsInChildren<Collider>();
        
        health.onDie += OnDie;
        health.onDamaged += OnDamaged;
        
        FindAndInitializeAllWeapons();
        var weapon = GetCurrentWeapon();
        weapon.ShowWeapon(true);

        var detectionModules = GetComponentsInChildren<DetectionModule>();
        DetectionModule = detectionModules[0];
        DetectionModule.onDetectedTarget += OnDetectedTarget;
        DetectionModule.onLostTarget += OnLostTarget;
        onAttack += DetectionModule.OnAttack;
        
        foreach (var renderer in GetComponentsInChildren<Renderer>(true))
        {
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                if (renderer.sharedMaterials[i] == EyeColorMaterial)
                {
                    eyeRendererData = new RendererIndexData(renderer, i);
                }

                if (renderer.sharedMaterials[i] == BodyMaterial)
                {
                    bodyRenderers.Add(new RendererIndexData(renderer, i));
                }
            }
        }

        bodyFlashMaterialPropertyBlock = new MaterialPropertyBlock();
        
        if (eyeRendererData.Renderer != null)
        {
            eyeColorMaterialPropertyBlock = new MaterialPropertyBlock();
            eyeColorMaterialPropertyBlock.SetColor("_EmissionColor", DefaultEyeColor);
            eyeRendererData.Renderer.SetPropertyBlock(eyeColorMaterialPropertyBlock,
                eyeRendererData.MaterialIndex);
        }
    }

    void Update()
    {
        EnsureIsWithinLevelBounds();

        DetectionModule.HandleTargetDetection(actor, selfColliders);

        Color currentColor = OnHitBodyGradient.Evaluate((Time.time - lastTimeDamaged) / FlashOnHitDuration);
        bodyFlashMaterialPropertyBlock.SetColor("_EmissionColor", currentColor);
        foreach (var data in bodyRenderers)
        {
            data.Renderer.SetPropertyBlock(bodyFlashMaterialPropertyBlock, data.MaterialIndex);
        }

        wasDamagedThisFrame = false;
    }

    void OnDestroy()
    {
        health.onDie -= OnDie;
        health.onDamaged -= OnDamaged;
    }

    #region Private Methods

    void EnsureIsWithinLevelBounds()
    {
        if (transform.position.y < SelfDestructYHeight)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void OnLostTarget()
    {
        onLostTarget?.Invoke();
        
        if (eyeRendererData.Renderer != null)
        {
            eyeColorMaterialPropertyBlock.SetColor("_EmissionColor", DefaultEyeColor);
            eyeRendererData.Renderer.SetPropertyBlock(eyeColorMaterialPropertyBlock,
                eyeRendererData.MaterialIndex);
        }
    }
    
    void OnDetectedTarget()
    {
        onDetectedTarget.Invoke();
        
        if (eyeRendererData.Renderer != null)
        {
            eyeColorMaterialPropertyBlock.SetColor("_EmissionColor", AttackEyeColor);
            eyeRendererData.Renderer.SetPropertyBlock(eyeColorMaterialPropertyBlock,
                eyeRendererData.MaterialIndex);
        }
    }
    
    public void OrientTowards(Vector3 lookPosition)
    {
        Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - transform.position, Vector3.up).normalized;
        if (lookDirection.sqrMagnitude != 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * OrientationSpeed);
        }
    }
    
    bool IsPathValid()
    {
        return PatrolPath && PatrolPath.PathNodes.Count > 0;
    }
    
    public void ResetPathDestination()
    {
        pathDestinationNodeIndex = 0;
    }
    
    void FindAndInitializeAllWeapons()
    {
        if (weapons == null)
        {
            weapons = GetComponentsInChildren<WeaponController>();
            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].Owner = gameObject;
            }
        }
    }
    
    void SetCurrentWeapon(int index)
    {
        currentWeaponIndex = index;
        currentWeapon = weapons[currentWeaponIndex];
        if (SwapToNextWeapon)
        {
            lastTimeWeaponSwapped = Time.time;
        }
        else
        {
            lastTimeWeaponSwapped = Mathf.NegativeInfinity;
        }
    }
    
    #endregion

    #region Public Methods

    public void SetPathDestinationToClosestNode()
    {
        if (IsPathValid())
        {
            int closestPathNodeIndex = 0;
            for (int i = 0; i < PatrolPath.PathNodes.Count; i++)
            {
                float distanceToPathNode = PatrolPath.GetDistanceToNode(transform.position, i);
                if (distanceToPathNode < PatrolPath.GetDistanceToNode(transform.position, closestPathNodeIndex))
                {
                    closestPathNodeIndex = i;
                }
            }

            pathDestinationNodeIndex = closestPathNodeIndex;
        }
        else
        {
            pathDestinationNodeIndex = 0;
        }
    }

    public Vector3 GetDestinationOnPath()
    {
        if (IsPathValid())
        {
            return PatrolPath.GetPositionOfPathNode(pathDestinationNodeIndex);
        }
        else
        {
            return transform.position;
        }
    }

    public void SetNavDestination(Vector3 destination)
    {
        if (NavMeshAgent)
        {
            NavMeshAgent.SetDestination(destination);
        }
    }

    public void UpdatePathDestination(bool inverseOrder = false)
    {
        if (IsPathValid())
        {
            // Check if reached the path destination
            if ((transform.position - GetDestinationOnPath()).magnitude <= PathReachingRadius)
            {
                pathDestinationNodeIndex =
                    inverseOrder ? (pathDestinationNodeIndex - 1) : (pathDestinationNodeIndex + 1);
                if (pathDestinationNodeIndex < 0)
                {
                    pathDestinationNodeIndex += PatrolPath.PathNodes.Count;
                }

                if (pathDestinationNodeIndex >= PatrolPath.PathNodes.Count)
                {
                    pathDestinationNodeIndex -= PatrolPath.PathNodes.Count;
                }
            }
        }
    }

    public void OrientWeaponsTowards(Vector3 lookPosition)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            // orient weapon towards player
            Vector3 weaponForward = (lookPosition - weapons[i].WeaponRoot.transform.position).normalized;
            weapons[i].transform.forward = weaponForward;
        }
    }

    public bool TryAtack(Vector3 enemyPosition)
    {
        if (GameFlowManager.Instance.GameIsEnding)
            return false;

        OrientWeaponsTowards(enemyPosition);

        if ((lastTimeWeaponSwapped + DelayAfterWeaponSwap) >= Time.time)
            return false;

        // Shoot the weapon
        bool didFire = GetCurrentWeapon().HandleShootInputs(false, true, false);

        if (didFire && onAttack != null)
        {
            onAttack.Invoke();

            if (SwapToNextWeapon && weapons.Length > 1)
            {
                int nextWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
                SetCurrentWeapon(nextWeaponIndex);
            }
        }

        return didFire;
    }

    public bool TryDropItem()
    {
        if (DropRate == 0 || LootPrefab == null)
            return false;
        else if (DropRate == 1)
            return true;
        else
            return (Random.value <= DropRate);
    }
    
    public WeaponController GetCurrentWeapon()
    {
        FindAndInitializeAllWeapons();
        if (currentWeapon == null)
        {
            SetCurrentWeapon(0);
        }
        return currentWeapon;
    }
    
    #endregion
    
    #region Events

    void OnDamaged(float damage, GameObject damageSource)
    {
        if (damageSource && !damageSource.GetComponent<MonsterController>())
        {
            DetectionModule.OnDamaged(damageSource);
            onDamaged?.Invoke();
            lastTimeDamaged = Time.time;

            if (DamageTick && !wasDamagedThisFrame)
                AudioUtil.CreateSFX(DamageTick, transform.position, AudioUtil.AudioGroups.DamageTick, 0f);
        
            wasDamagedThisFrame = true;
        }
    }

    void OnDie()
    {
        var vfx = Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);
        Destroy(vfx, 5f);

        MonsterManager.Instance.UnregisterEnemy(this);

        if (TryDropItem())
        {
            Instantiate(LootPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject, DeathDuration);
    }

    #endregion
}

