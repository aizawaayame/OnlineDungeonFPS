
using Models;
using UnityEngine;
using Utilities;

public class UINotificationHUD : MonoBehaviour
{

    #region Fields

    [SerializeField] RectTransform NotificationPanel;
    [SerializeField] GameObject NotificationPrefab;

    #endregion


    void Start()
    {
        PlayerWeaponController playerWeaponsManager = User.Instance.CurrentCharacterObject.GetComponent<PlayerWeaponController>();
        playerWeaponsManager.onAddedWeapon += OnPickupWeapon;

        EventUtil.AddListener<ObjectiveUpdateEvent>(OnObjectiveUpdateEvent);
    }

    void OnObjectiveUpdateEvent(ObjectiveUpdateEvent evt)
    {
        if (!string.IsNullOrEmpty(evt.NotificationText))
            CreateNotification(evt.NotificationText);
    }

    void OnPickupWeapon(WeaponController weaponController, int index)
    {
        if (index != 0)
            CreateNotification("Picked up weapon : " + weaponController.weaponName);
    }

    void OnUnlockJetpack(bool unlock)
    {
        CreateNotification("Jetpack unlocked");
    }

    public void CreateNotification(string text)
    {
        GameObject notificationInstance = Instantiate(NotificationPrefab, NotificationPanel);
        notificationInstance.transform.SetSiblingIndex(0);

        UINotificationToast toast = notificationInstance.GetComponent<UINotificationToast>();
        if (toast)
        {
            toast.Initialize(text);
        }
    }

    void OnDestroy()
    {
        EventUtil.RemoveListener<ObjectiveUpdateEvent>(OnObjectiveUpdateEvent);
    }
}

