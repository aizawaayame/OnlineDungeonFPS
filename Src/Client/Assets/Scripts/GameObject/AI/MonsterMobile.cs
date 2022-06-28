
using System;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MonsterController))]
public class MonsterMobile : MonoBehaviour
{
    public enum AIState
    {
        Patrol,
        Follow,
        Attack,
    }

    #region Const

    const string ANIM_MOVE_SPEED_PARAMETER = "MoveSpeed";
    const string ANIM_ATTACK_PARAMETER = "Attack";
    const string ANIM_ALERTED_PARAMETER = "Alerted";
    const string ANIM_ON_DAMAGED_PARAMETER = "OnDamaged";

    #endregion

    #region Fields

    [SerializeField] Animator animator;
    [SerializeField][Range(0f, 1f)] public float attackStopDistanceRatio = 0.5f;
    [SerializeField] ParticleSystem[] RandomHitSparks;

    [SerializeField] ParticleSystem[] OnDetectVfx;
    [SerializeField] AudioClip OnDetectSfx;

    [SerializeField] AudioClip MovementSound;
    [SerializeField] MinMaxFloat PitchDistortionMovementSpeed;
    
    MonsterController monsterController;
    AudioSource audioSource;

    #endregion
    
    #region Properties

    public AIState AiState { get; private set; }

    #endregion
    
    void Start()
    {
        monsterController = GetComponent<MonsterController>();
        monsterController.onAttack += OnAttack;
        monsterController.onDetectedTarget += OnDetectedTarget;
        monsterController.onLostTarget += OnLostTarget;
        monsterController.SetPathDestinationToClosestNode();
        monsterController.onDamaged += OnDamaged;
        
        AiState = AIState.Patrol;
        
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = MovementSound;
        audioSource.Play();
    }

    void Update()
    {
        UpdateAiStateTransitions();
        UpdateCurrentAiState();

        float moveSpeed = monsterController.NavMeshAgent.velocity.magnitude;
        
        animator.SetFloat(ANIM_MOVE_SPEED_PARAMETER, moveSpeed);
        
        audioSource.pitch = Mathf.Lerp(PitchDistortionMovementSpeed.Min, PitchDistortionMovementSpeed.Max,
            moveSpeed / monsterController.NavMeshAgent.speed);
    }

    #region Private Methods

        void UpdateAiStateTransitions()
    {
        switch (AiState)
        {
            case AIState.Follow:
                if (monsterController.IsSeeingTarget && monsterController.IsTargetInAttackRange)
                {
                    AiState = AIState.Attack;
                    monsterController.SetNavDestination(transform.position);
                }

                break;
            case AIState.Attack:
                if (!monsterController.IsTargetInAttackRange)
                {
                    AiState = AIState.Follow;
                }

                break;
        }
    }

    void UpdateCurrentAiState()
    {
        // Handle logic 
        switch (AiState)
        {
            case AIState.Patrol:
                monsterController.UpdatePathDestination();
                monsterController.SetNavDestination(monsterController.GetDestinationOnPath());
                break;
            case AIState.Follow:
                monsterController.SetNavDestination(monsterController.KnownDetectedTarget.transform.position);
                monsterController.OrientTowards(monsterController.KnownDetectedTarget.transform.position);
                monsterController.OrientWeaponsTowards(monsterController.KnownDetectedTarget.transform.position);
                break;
            case AIState.Attack:
                if (Vector3.Distance(monsterController.KnownDetectedTarget.transform.position,
                        monsterController.DetectionModule.DetectionSourcePoint.position)
                    >= (attackStopDistanceRatio * monsterController.DetectionModule.AttackRange))
                {
                    monsterController.SetNavDestination(monsterController.KnownDetectedTarget.transform.position);
                }
                else
                {
                    monsterController.SetNavDestination(transform.position);
                }

                monsterController.OrientTowards(monsterController.KnownDetectedTarget.transform.position);
                monsterController.TryAtack(monsterController.KnownDetectedTarget.transform.position);
                break;
        }
    }

    void OnAttack()
    {
        animator.SetTrigger(ANIM_ATTACK_PARAMETER);
    }

    void OnDetectedTarget()
    {
        if (AiState == AIState.Patrol)
        {
            AiState = AIState.Follow;
        }

        for (int i = 0; i < OnDetectVfx.Length; i++)
        {
            OnDetectVfx[i].Play();
        }
        animator.SetBool(ANIM_ALERTED_PARAMETER, true);
    }

    void OnLostTarget()
    {
        if (AiState == AIState.Follow || AiState == AIState.Attack)
        {
            AiState = AIState.Patrol;
        }

        for (int i = 0; i < OnDetectVfx.Length; i++)
        {
            OnDetectVfx[i].Stop();
        }

        animator.SetBool(ANIM_ALERTED_PARAMETER, false);
    }

    void OnDamaged()
    {
        if (RandomHitSparks.Length > 0)
        {
            int n = Random.Range(0, RandomHitSparks.Length - 1);
            RandomHitSparks[n].Play();
        }

        animator.SetTrigger(ANIM_ON_DAMAGED_PARAMETER);
    }

    #endregion


}


