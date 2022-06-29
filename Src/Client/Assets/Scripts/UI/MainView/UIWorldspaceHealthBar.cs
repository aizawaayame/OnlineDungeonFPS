
using UnityEngine;
using UnityEngine.UI;

public class UIWorldspaceHealthBar : MonoBehaviour
{
    public HealthController Health;
    public Image HealthBarImage;
    public Transform HealthBarPivot;
    public bool HideFullHealthBar = true;

    void Update()
    {

        HealthBarImage.fillAmount = Health.CurrentHealth / Health.MaxHealth;
        HealthBarPivot.LookAt(Camera.main.transform.position);
        if (HideFullHealthBar)
            HealthBarPivot.gameObject.SetActive(HealthBarImage.fillAmount != 1);
    }
}

