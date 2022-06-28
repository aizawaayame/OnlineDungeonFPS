
using UnityEngine;
using Utilities;

[RequireComponent(typeof(MonsterController))]
public class MonsterTurret : MonoBehaviour
{
    public enum AIState
    {
        Idle,
        Attack,
    }

    #region Const

    const string ANIM_ON_DAMAGED_PARAMETER = "OnDamaged";
    const string ANIM_IS_ACTIVE_PARAMETER = "IsActive";

    #endregion
    
    #region Fields

    [SerializeField] Transform turretPivot;
    [SerializeField] Transform turretAimPoint;
    [SerializeField] Animator animator;
    [SerializeField] float aimRotationSharpness = 5f;
    [SerializeField] float lookAtRotationSharpness = 2.5f;
    [SerializeField] float detectionFireDelay = 1f;
    [SerializeField] float aimingTransitionBlendTime = 1f;
    [SerializeField] ParticleSystem[] randomHitSparks;
    [SerializeField] ParticleSystem[] onDetectVfx;
    [SerializeField] AudioClip onDetectSfx;
    
    MonsterController monsterController;
    HealthController health;
    Quaternion rotationWeaponForwardToPivot;
    float timeStartedDetection;
    float timeLostDetection;
    Quaternion previousPivotAimingRotation;
    Quaternion pivotAimingRotation;
    #endregion

    #region Properties

    public AIState AiState { get; private set; }

    #endregion

    void Start()
    {
        health = GetComponent<HealthController>();
        health.onDamaged += OnDamaged;

        monsterController = GetComponent<MonsterController>();
        monsterController.onDetectedTarget += OnDetectedTarget;
        monsterController.onLostTarget += OnLostTarget;


        rotationWeaponForwardToPivot =
            Quaternion.Inverse(monsterController.GetCurrentWeapon().WeaponMuzzle.rotation) * turretPivot.rotation;
        AiState = AIState.Idle;

        timeStartedDetection = Mathf.NegativeInfinity;
        previousPivotAimingRotation = turretPivot.rotation;
    }

    void Update()
    {
        UpdateCurrentAiState();
    }

    void LateUpdate()
    {
        UpdateTurretAiming();
    }

    #region Private Methods

        void UpdateCurrentAiState()
    {
        switch (AiState)
        {
            case AIState.Attack:
                bool mustShoot = Time.time > timeStartedDetection + detectionFireDelay;
                Vector3 directionToTarget =
                    (monsterController.KnownDetectedTarget.transform.position - turretAimPoint.position).normalized;
                Quaternion offsettedTargetRotation =
                    Quaternion.LookRotation(directionToTarget) * rotationWeaponForwardToPivot;
                pivotAimingRotation = Quaternion.Slerp(previousPivotAimingRotation, offsettedTargetRotation,
                    (mustShoot ? aimRotationSharpness : lookAtRotationSharpness) * Time.deltaTime);

                if (mustShoot)
                {
                    Vector3 correctedDirectionToTarget =
                        (pivotAimingRotation * Quaternion.Inverse(rotationWeaponForwardToPivot)) *
                        Vector3.forward;

                    monsterController.TryAtack(turretAimPoint.position + correctedDirectionToTarget);
                }

                break;
        }
    }

    void UpdateTurretAiming()
    {
        switch (AiState)
        {
            case AIState.Attack:
                turretPivot.rotation = pivotAimingRotation;
                break;
            default:
                // Use the turret rotation of the animation
                turretPivot.rotation = Quaternion.Slerp(pivotAimingRotation, turretPivot.rotation,
                    (Time.time - timeLostDetection) / aimingTransitionBlendTime);
                break;
        }

        previousPivotAimingRotation = turretPivot.rotation;
    }

    void OnDamaged(float dmg, GameObject source)
    {
        if (randomHitSparks.Length > 0)
        {
            int n = Random.Range(0, randomHitSparks.Length - 1);
            randomHitSparks[n].Play();
        }

        animator.SetTrigger(ANIM_ON_DAMAGED_PARAMETER);
    }

    void OnDetectedTarget()
    {
        Debug.Log("进入警戒状态");
        if (AiState == AIState.Idle)
        {
            AiState = AIState.Attack;
        }

        for (int i = 0; i < onDetectVfx.Length; i++)
        {
            onDetectVfx[i].Play();
        }

        animator.SetBool(ANIM_IS_ACTIVE_PARAMETER, true);
        timeStartedDetection = Time.time;
    }

    void OnLostTarget()
    {
        if (AiState == AIState.Attack)
        {
            AiState = AIState.Idle;
        }

        for (int i = 0; i < onDetectVfx.Length; i++)
        {
            onDetectVfx[i].Stop();
        }

        animator.SetBool(ANIM_IS_ACTIVE_PARAMETER, false);
        timeLostDetection = Time.time;
    }

    #endregion

}

