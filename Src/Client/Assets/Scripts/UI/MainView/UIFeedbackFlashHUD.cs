
using Managers;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIFeedbackFlashHUD : MonoBehaviour
{

    #region Fields

    [SerializeField]  Image FlashImage;
    [SerializeField]  CanvasGroup FlashCanvasGroup;
    [SerializeField]  CanvasGroup VignetteCanvasGroup;
    [SerializeField]  Color DamageFlashColor;
    [SerializeField]  float DamageFlashDuration;
    [SerializeField]  float DamageFlashMaxAlpha = 1f;
    [SerializeField]  float CriticaHealthVignetteMaxAlpha = .8f;
    [SerializeField]  float PulsatingVignetteFrequency = 4f;
    [SerializeField]  Color HealFlashColor;
    [SerializeField]  float HealFlashDuration;
    [SerializeField]  float HealFlashMaxAlpha = 1f;

    bool flashActive;
    float lastTimeFlashStarted = Mathf.NegativeInfinity;
    HealthController playerHealth;


    #endregion


    void Start()
    {
        // Subscribe to player damage events
        PlayerController playerController = User.Instance.CurrentCharacterObject.GetComponent<PlayerController>();

        playerHealth = playerController.GetComponent<HealthController>();
        
        playerHealth.onDamaged += OnTakeDamage;
        playerHealth.onHealed += OnHealed;
    }

    void Update()
    {
        if (playerHealth.IsCritical)
        {
            VignetteCanvasGroup.gameObject.SetActive(true);
            float vignetteAlpha =
                (1 - (playerHealth.CurrentHealth / playerHealth.MaxHealth /
                      playerHealth.CriticalHealthRatio)) * CriticaHealthVignetteMaxAlpha;

            if (GameFlowManager.Instance.GameIsEnding)
                VignetteCanvasGroup.alpha = vignetteAlpha;
            else
                VignetteCanvasGroup.alpha =
                    ((Mathf.Sin(Time.time * PulsatingVignetteFrequency) / 2) + 0.5f) * vignetteAlpha;
        }
        else
        {
            VignetteCanvasGroup.gameObject.SetActive(false);
        }


        if (flashActive)
        {
            float normalizedTimeSinceDamage = (Time.time - lastTimeFlashStarted) / DamageFlashDuration;

            if (normalizedTimeSinceDamage < 1f)
            {
                float flashAmount = DamageFlashMaxAlpha * (1f - normalizedTimeSinceDamage);
                FlashCanvasGroup.alpha = flashAmount;
            }
            else
            {
                FlashCanvasGroup.gameObject.SetActive(false);
                flashActive = false;
            }
        }
    }

    void ResetFlash()
    {
        lastTimeFlashStarted = Time.time;
        flashActive = true;
        FlashCanvasGroup.alpha = 0f;
        FlashCanvasGroup.gameObject.SetActive(true);
    }

    void OnTakeDamage(float dmg, GameObject damageSource)
    {
        ResetFlash();
        FlashImage.color = DamageFlashColor;
    }

    void OnHealed(float amount)
    {
        ResetFlash();
        FlashImage.color = HealFlashColor;
    }
}

