using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Events;

public class DetectionModule : MonoBehaviour
{
    public UnityAction onDetectedTarget;
    public UnityAction onLostTarget;

    #region Const

    const string ANIM_ATTACK_PARAMETER = "Attack";
    const string ANIM_ON_DAMAGED_PARAMETER = "OnDamaged";

    #endregion

    #region Fields
    
    [SerializeField] Transform detectionSourcePoint;
    [SerializeField] float detectionRange = 20f;
    [SerializeField] float attackRange = 10f;
    [SerializeField] float knownTargetTimeout = 4f;
    [SerializeField] Animator animator;

    #endregion

    #region Properties
    public Transform DetectionSourcePoint { get => detectionSourcePoint; }
    public float DetectionRange { get => detectionRange; }
    public float AttackRange { get => attackRange; }
    public GameObject KnownDetectedTarget { get; private set; }
    public bool IsTargetInAttackRange { get; private set; }
    public bool IsSeeingTarget { get; private set; }
    public bool HadKnownTarget { get; private set; }
    protected float TimeLastSeenTarget = Mathf.NegativeInfinity;

    #endregion

    #region Public Methods

     /// <summary>
    /// Handle monster's target detection.
    /// </summary>
    /// <param name="actor">The actor of the monster.</param>
    /// <param name="selfColliders">The monster's colliders.</param>
    public virtual void HandleTargetDetection(Actor actor, Collider[] selfColliders)
    {
        if (KnownDetectedTarget && !IsSeeingTarget && (Time.time - TimeLastSeenTarget) > knownTargetTimeout)
        {
            KnownDetectedTarget = null;
        }
        float sqrDetectionRange = detectionRange * detectionRange;
        IsSeeingTarget = false;
        float closestSqrDistance = Mathf.Infinity;
        foreach (Actor otherActor in ActorManager.Instance.Actors)
        {
            if (otherActor.Affiliation != actor.Affiliation)
            {
                float sqrDistance = (otherActor.transform.position - detectionSourcePoint.position).sqrMagnitude;
                if (sqrDistance < sqrDetectionRange && sqrDistance < closestSqrDistance)
                {
                    RaycastHit[] hits = Physics.RaycastAll(detectionSourcePoint.position,
                        (otherActor.AimPoint.position - detectionSourcePoint.position).normalized, detectionRange,
                        -1, QueryTriggerInteraction.Ignore);
                    RaycastHit closestValidHit = new RaycastHit();
                    closestValidHit.distance = Mathf.Infinity;
                    bool foundValidHit = false;
                    foreach (var hit in hits)
                    {
                        if (!selfColliders.Contains(hit.collider) && hit.distance < closestValidHit.distance)
                        {
                            closestValidHit = hit;
                            foundValidHit = true;
                        }
                    }
                    if (foundValidHit)
                    {
                        Actor hitActor = closestValidHit.collider.GetComponentInParent<Actor>();
                        if (hitActor == otherActor)
                        {
                            IsSeeingTarget = true;
                            closestSqrDistance = sqrDistance;
                            TimeLastSeenTarget = Time.time;
                            KnownDetectedTarget = otherActor.AimPoint.gameObject;
                        }
                    }
                }
            }
        }
        IsTargetInAttackRange = KnownDetectedTarget != null &&
                                Vector3.Distance(transform.position, KnownDetectedTarget.transform.position) <=
                                attackRange;
        
        if (!HadKnownTarget &&
            KnownDetectedTarget != null)
        {
            OnDetect();
        }

        if (HadKnownTarget &&
            KnownDetectedTarget == null)
        {
            OnLostTarget();
        }
        HadKnownTarget = KnownDetectedTarget != null;
    }

    public virtual void OnLostTarget() => onLostTarget?.Invoke();

    public virtual void OnDetect() => onDetectedTarget?.Invoke();

    public virtual void OnDamaged(GameObject damageSource)
    {
        TimeLastSeenTarget = Time.time;
        KnownDetectedTarget = damageSource;
        if (animator)
        {
            animator.SetTrigger(ANIM_ON_DAMAGED_PARAMETER);
        }
    }
    public virtual void OnAttack()
    {
        if (animator)
        {
            animator.SetTrigger(ANIM_ATTACK_PARAMETER);
        }
    }

    #endregion
}

