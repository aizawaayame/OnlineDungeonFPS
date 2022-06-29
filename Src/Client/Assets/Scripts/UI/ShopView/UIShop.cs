
using Models;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIShop : MonoBehaviour
{
    public GameObject MenuRoot;
    public WeaponController Shotgun;
    public WeaponController Launcher;
    
    PlayerWeaponController weapons;

    void Start()
    {
        MenuRoot.SetActive(false);
        weapons = User.Instance.CurrentCharacterObject.GetComponent<PlayerWeaponController>();
    }
    void Update()
    {
        if (!MenuRoot.activeSelf && Input.GetMouseButton(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetButtonDown("Pause Menu")
            ||(MenuRoot.activeSelf && Input.GetButtonDown("Cancel")))
        {
            SetShopMenuActivation(!MenuRoot.activeSelf);
        }
        
    }
    
    void SetShopMenuActivation(bool active)
    {
        MenuRoot.SetActive(active);
        if (MenuRoot.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    #region Events

    public void OnBuyShotgun()
    {
        if (User.Instance.CurrentCharacter.Gold < 2000)
        {
            MessageBox.Show("金币不足", "购买失败");
            return;
        }
        if (!weapons.AddWeapon(Shotgun))
        {
            MessageBox.Show("已经拥有该武器", "购买失败");
            return;
        }
        User.Instance.CurrentCharacter.Gold -= 2000;
        weapons.AddWeapon(Shotgun);
        ShopService.Instance.SendBuyWeapon(2000);
        MessageBox.Show("购买成功", "购买成功");
    }
    public void OnBuyLauncher()
    {
        if (User.Instance.CurrentCharacter.Gold < 3000)
        {
            MessageBox.Show("金币不足", "购买失败");
            return;
        }
        if (!weapons.AddWeapon(Launcher))
        {
            MessageBox.Show("已经拥有该武器", "购买失败");
            return;
        }
        User.Instance.CurrentCharacter.Gold -= 3000;
        weapons.AddWeapon(Launcher);
        ShopService.Instance.SendBuyWeapon(3000);
        MessageBox.Show("购买成功", "购买成功");
    }
    public void CloseShopMenu()
    {
        SetShopMenuActivation(false);
    }

    #endregion
}

