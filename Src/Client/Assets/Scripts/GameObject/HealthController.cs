
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    public UnityAction<float> OnHealed;
    public UnityAction<float, GameObject> OnDamaged;
    public UnityAction OnDie;

    #region Const

    const float CRITICAL_HEALTH_RATIO = 0.3f;

    #endregion

    #region Properties

    public float MaxHealth { get; set; }
    public float CurrentHealth { get; set; }
    
    bool CanPickup { get => CurrentHealth < MaxHealth; }

    bool IsCritical
    {
        get
        {
            return CurrentHealth / MaxHealth <= CRITICAL_HEALTH_RATIO;
        }
    }

    bool IsDead;
    #endregion
    
    void Start()
    {
        CurrentHealth = MaxHealth;
    }
    
    #region Public Methods
    
    public void Heal(float healAmount)
    {
        float healthBefore = CurrentHealth;
        CurrentHealth += healAmount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        // call OnHeal action
        float trueHealAmount = CurrentHealth - healthBefore;
        if (trueHealAmount > 0f)
        {
            OnHealed?.Invoke(trueHealAmount);
        }
    }

    public void TakeDamage(float dmg, GameObject damageSource)
    {
        float healthBefore = CurrentHealth;
        CurrentHealth -= dmg;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        float trueDamageAmount = healthBefore - CurrentHealth;
        if (trueDamageAmount > 0f)
        {
            OnDamaged?.Invoke(trueDamageAmount, damageSource);
        }
    }

    public void InflictDamage(float damage,GameObject damageSource)
    {
        float totalDamage = damage;
        this.TakeDamage(totalDamage,damageSource);
        
    }
    

    #endregion
    
    #region Private Methods
    
    void HandleDeath()
    {
        if (IsDead)
            return;

        // call OnDie action
        if (CurrentHealth <= 0f)
        {
            IsDead = true;
            OnDie?.Invoke();
        }
    }
    
    #endregion




  

}

