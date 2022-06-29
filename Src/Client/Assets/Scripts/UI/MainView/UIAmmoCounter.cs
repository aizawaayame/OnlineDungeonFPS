using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

[RequireComponent(typeof(UIFillBarColorChange))]
public class UIAmmoCounter : MonoBehaviour
{

    #region Fields

    [SerializeField] CanvasGroup CanvasGroup;
    [SerializeField] Image WeaponImage;
    [SerializeField] Image AmmoBackgroundImage;
    [SerializeField] Image AmmoFillImage;
    [SerializeField] TextMeshProUGUI WeaponIndexText;
    [SerializeField] TextMeshProUGUI BulletCounter;
    [SerializeField] RectTransform Reload;
    [SerializeField] float UnselectedOpacity = 0.5f;
    [SerializeField] Vector3 UnselectedScale = Vector3.one * 0.8f;
    [SerializeField] GameObject ControlKeysRoot;
    [SerializeField] UIFillBarColorChange FillBarColorChange;
    [SerializeField] float AmmoFillMovementSharpness = 20f;

    PlayerWeaponController playerWeaponsController;
    WeaponController weapon;
    #endregion

    #region Properties

    public int WeaponCounterIndex { get; set; }

    #endregion
    
    void Awake()
    {
        EventUtil.AddListener<AmmoPickupEvent>(OnAmmoPickup);
    }

    void OnAmmoPickup(AmmoPickupEvent evt)
    {
    }

    public void Initialize(WeaponController weapon, int weaponIndex)
    {
        this.weapon = weapon;
        WeaponCounterIndex = weaponIndex;
        WeaponImage.sprite = weapon.weaponIcon;

        Reload.gameObject.SetActive(false);
        playerWeaponsController = User.Instance.CurrentCharacterObject.GetComponent<PlayerWeaponController>();
        WeaponIndexText.text = (WeaponCounterIndex + 1).ToString();
        FillBarColorChange.Initialize(1f, this.weapon.GetAmmoNeededToShoot());
    }

    void Update()
    {
        float currenFillRatio = weapon.CurrentAmmoRatio;
        AmmoFillImage.fillAmount = Mathf.Lerp(AmmoFillImage.fillAmount, currenFillRatio,
            Time.deltaTime * AmmoFillMovementSharpness);

        bool isActiveWeapon = weapon == playerWeaponsController.GetActiveWeapon();

        CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, isActiveWeapon ? 1f : UnselectedOpacity,
            Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, isActiveWeapon ? Vector3.one : UnselectedScale,
            Time.deltaTime * 10);
        ControlKeysRoot.SetActive(!isActiveWeapon);
        FillBarColorChange.UpdateVisual(currenFillRatio);
    }

    void Destroy()
    {
        EventUtil.RemoveListener<AmmoPickupEvent>(OnAmmoPickup);
    }
}

