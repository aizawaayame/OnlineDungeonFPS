
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{

    #region Fields

    [SerializeField] float areaOfEffectDistance = 5f;
    [SerializeField] AnimationCurve damageRatioOverDistance;

    #endregion
    
    public void InflictDamageInArea(float damage, Vector3 center, LayerMask layers,
        QueryTriggerInteraction interaction, GameObject owner)
    {
        List<HealthController> healths = new List<HealthController>();
        Collider[] affectedColliders = Physics.OverlapSphere(center, areaOfEffectDistance, layers, interaction);
        foreach (var coll in affectedColliders)
        {
            HealthController health = GetComponentInParent<HealthController>();
            if (health != null)
            {
                if (!healths.Contains(health))
                {
                    healths.Add(health);
                }
            }
        }
        foreach (HealthController health in healths)
        {
            float distance = Vector3.Distance(health.transform.position, transform.position);
            health.InflictDamage(
                damage * damageRatioOverDistance.Evaluate(distance / areaOfEffectDistance), owner);
        }
    }

}

