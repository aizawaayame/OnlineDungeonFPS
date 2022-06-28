using UnityEngine;

public class DamageableModule : MonoBehaviour
{

    #region Fields

    [SerializeField] HealthController health;

    #endregion
    
    public void InflictDamage(float damage,GameObject damageSource)
    {
        float totalDamage = damage;
        health.TakeDamage(totalDamage,damageSource);
    
    }

}

