using System.Collections.Generic;
using Models;
using UnityEngine;

public class UIWeaponHUD : MonoBehaviour
{

    #region Fields

    public RectTransform AmmoPanel;
    public GameObject AmmoCounterPrefab;
    
    PlayerWeaponController playerWeaponsController;
    List<UIAmmoCounter> ammoCounters = new List<UIAmmoCounter>();
    
    #endregion
    
    void Start()
    {
        playerWeaponsController = User.Instance.CurrentCharacterObject.GetComponent<PlayerWeaponController>();

        WeaponController activeWeapon = playerWeaponsController.GetActiveWeapon();
        if (activeWeapon)
        {
            AddWeapon(activeWeapon, playerWeaponsController.ActiveWeaponIndex);
            ChangeWeapon(activeWeapon);
        }

        playerWeaponsController.onAddedWeapon += AddWeapon;
        playerWeaponsController.onRemovedWeapon += RemoveWeapon;
        playerWeaponsController.onSwitchedToWeapon += ChangeWeapon;
    }

    #region Events

    void AddWeapon(WeaponController newWeapon, int weaponIndex)
    {
        GameObject ammoCounterInstance = Instantiate(AmmoCounterPrefab, AmmoPanel);
        UIAmmoCounter newAmmoCounter = ammoCounterInstance.GetComponent<UIAmmoCounter>();

        newAmmoCounter.Initialize(newWeapon, weaponIndex);

        ammoCounters.Add(newAmmoCounter);
    }

    void RemoveWeapon(WeaponController newWeapon, int weaponIndex)
    {
        int foundCounterIndex = -1;
        for (int i = 0; i < ammoCounters.Count; i++)
        {
            if (ammoCounters[i].WeaponCounterIndex == weaponIndex)
            {
                foundCounterIndex = i;
                Destroy(ammoCounters[i].gameObject);
            }
        }

        if (foundCounterIndex >= 0)
        {
            ammoCounters.RemoveAt(foundCounterIndex);
        }
    }

    void ChangeWeapon(WeaponController weapon)
    {
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(AmmoPanel);
    }

    #endregion

}

