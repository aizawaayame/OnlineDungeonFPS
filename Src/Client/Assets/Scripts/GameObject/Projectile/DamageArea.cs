
using System.Collections.Generic;
using Unity.FPS.Game;
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
        List<DamageableModule> damageables = new List<DamageableModule>();
        Collider[] affectedColliders = Physics.OverlapSphere(center, areaOfEffectDistance, layers, interaction);
        foreach (var coll in affectedColliders)
        {
            DamageableModule damageable = coll.GetComponent<DamageableModule>();
            if (damageable != null)
            {
                if (!damageables.Contains(damageable))
                {
                    damageables.Add(damageable);
                }
            }
        }
        foreach (DamageableModule damageable in damageables)
        {
            float distance = Vector3.Distance(damageable.transform.position, transform.position);
            damageable.InflictDamage(
                damage * damageRatioOverDistance.Evaluate(distance / areaOfEffectDistance), owner);
        }
    }

}

