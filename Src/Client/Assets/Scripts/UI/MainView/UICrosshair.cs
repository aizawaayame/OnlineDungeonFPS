
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UICrosshair : MonoBehaviour
{

    #region Fields

    [SerializeField] Image crosshairImage;
    [SerializeField] Sprite nullCrosshairSprite;
    [SerializeField] float crosshairUpdateShrpness = 5f;

    PlayerWeaponController weaponsController;
    bool wasPointingAtEnemy;
    RectTransform crosshairRectTransform;
    CrosshairData crosshairDataDefault;
    CrosshairData crosshairDataTarget;
    CrosshairData currentCrosshair;

    #endregion
    
    void Start()
    {
        weaponsController = User.Instance.CurrentCharacterObject.GetComponent<PlayerWeaponController>();
        OnWeaponChanged(weaponsController.GetActiveWeapon());
        weaponsController.onSwitchedToWeapon += OnWeaponChanged;
    }

    void Update()
    {
        UpdateCrosshairPointingAtEnemy(false);
        wasPointingAtEnemy = weaponsController.IsPointingAtEnemy;
    }

    #region Private Methods

    void UpdateCrosshairPointingAtEnemy(bool force)
    {
        if (crosshairDataDefault.CrosshairSprite == null)
            return;

        if ((force || !wasPointingAtEnemy) && weaponsController.IsPointingAtEnemy)
        {
            currentCrosshair = crosshairDataTarget;
            crosshairImage.sprite = currentCrosshair.CrosshairSprite;
            crosshairRectTransform.sizeDelta = currentCrosshair.CrosshairSize * Vector2.one;
        }
        else if ((force || wasPointingAtEnemy) && !weaponsController.IsPointingAtEnemy)
        {
            currentCrosshair = crosshairDataDefault;
            crosshairImage.sprite = currentCrosshair.CrosshairSprite;
            crosshairRectTransform.sizeDelta = currentCrosshair.CrosshairSize * Vector2.one;
        }

        crosshairImage.color = Color.Lerp(crosshairImage.color, currentCrosshair.CrosshairColor,
            Time.deltaTime * crosshairUpdateShrpness);

        crosshairRectTransform.sizeDelta = Mathf.Lerp(crosshairRectTransform.sizeDelta.x,
            currentCrosshair.CrosshairSize,
            Time.deltaTime * crosshairUpdateShrpness) * Vector2.one;
    }

    #endregion

    #region Events
    
    void OnWeaponChanged(WeaponController newWeapon)
    {
        if (newWeapon)
        {
            crosshairImage.enabled = true;
            crosshairDataDefault = newWeapon.crosshairDataDefault;
            crosshairDataTarget = newWeapon.crosshairDataTargetInSight;
            crosshairRectTransform = crosshairImage.GetComponent<RectTransform>();
        }
        else
        {
            if (nullCrosshairSprite)
            {
                crosshairImage.sprite = nullCrosshairSprite;
            }
            else
            {
                crosshairImage.enabled = false;
            }
        }
        UpdateCrosshairPointingAtEnemy(true);
    }
    
    #endregion

}


