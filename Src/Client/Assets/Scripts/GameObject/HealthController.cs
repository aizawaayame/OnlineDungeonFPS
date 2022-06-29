using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    public UnityAction<float> onHealed;
    public UnityAction<float, UnityEngine.GameObject> onDamaged;
    public UnityAction onDie;

    #region Const

     const float CRITICAL_HEALTH_RATIO = 0.3f;

    #endregion

    #region Fields

    [SerializeField] float maxHealth;

    #endregion
    
    #region Properties
    public float CriticalHealthRatio { get => CRITICAL_HEALTH_RATIO; }
    public float MaxHealth { get=>maxHealth; set => maxHealth = value; }
    public float CurrentHealth { get; set; }
    
    public bool CanPickup { get => CurrentHealth < MaxHealth; }

    public bool IsCritical
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
            onHealed?.Invoke(trueHealAmount);
        }
    }

    public void TakeDamage(float dmg, UnityEngine.GameObject damageSource)
    {
        Debug.Log($"{CurrentHealth}");
        float healthBefore = CurrentHealth;
        CurrentHealth -= dmg;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);
        float trueDamageAmount = healthBefore - CurrentHealth;
        if (trueDamageAmount > 0f)
        {
            onDamaged?.Invoke(trueDamageAmount, damageSource);
        }
        HandleDeath();
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
            onDie?.Invoke();
        }
    }
    
    #endregion
    
}

