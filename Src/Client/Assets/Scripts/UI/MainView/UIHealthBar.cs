using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{

    #region Fields

    [SerializeField] Image HealthFillImage;
    HealthController playerHealth;

    #endregion


    void Start()
    {
        PlayerController playerCharacterController = User.Instance.CurrentCharacterObject.GetComponent<PlayerController>();

        playerHealth = playerCharacterController.GetComponent<HealthController>();
    }

    void Update()
    {
        // update health bar value
        HealthFillImage.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;
    }
}

