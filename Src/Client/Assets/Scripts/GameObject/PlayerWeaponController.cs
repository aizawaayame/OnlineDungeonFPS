using System.Collections.Generic;
using Managers;
using Models;
using Protocol.Message;
using Services;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeaponController : MonoBehaviour
{
    enum WeaponSwitchState
    {
        Up,
        Down,
        PutDownPrevious,
        PutUpNew,
    }
    
    public UnityAction<WeaponController> OnSwitchedToWeapon;
    public UnityAction<WeaponController, int> OnAddedWeapon;
    public UnityAction<WeaponController, int> OnRemovedWeapon;

    #region Const
    
    const float BOB_FREQUENCY = 10f;
    const float BOB_SHARPNESS = 10f;
    const  float DEFAULT_BOB_AMOUNT = 0.05f;
    const  float AIMING_BOB_AMOUNT = 0.02f;
    const float RECOIL_SHARPNESS = 50f;
    const float MAX_RECOIL_DISTANCE = 0.5f;
    const float RECOIL_RESTITUTION_SHARPNESS = 10f;
    const float AimingAnimationSpeed = 10f;
    const float DEFAULT_FOV = 60f;
    const float WEAPON_FOV_MULTIPLIER = 1f;
    const float WEAPON_SWITCH_DELAY = 1f;

    #endregion

    #region Fields
    //for test
    [SerializeField] List<WeaponController> startingWeapons = new List<WeaponController>();
    [SerializeField] Camera weaponCamera;
    [SerializeField] Transform weaponParentSocket;
    [SerializeField] Transform defaultWeaponPosition;
    [SerializeField] Transform aimingWeaponPosition;
    [SerializeField] Transform downWeaponPosition;
    [SerializeField] LayerMask fpsWeaponLayer ;
    WeaponController[] weaponSlots = new WeaponController[9]; // 9 available weapon slots
    PlayerController playerController;
    float weaponBobFactor;
    Vector3 lastCharacterPosition;
    Vector3 weaponMainLocalPosition;
    Vector3 weaponBobLocalPosition;
    Vector3 weaponRecoilLocalPosition;
    Vector3 accumulatedRecoil;
    float timeStartedWeaponSwitch;
    WeaponSwitchState weaponSwitchState;
    int weaponSwitchNewWeaponIndex;
    #endregion
    
    public Camera WeaponCamera { get => weaponCamera; }
    public bool IsAiming { get; private set; }
    public bool IsPointingAtEnemy { get; private set; }
    public int ActiveWeaponIndex { get; private set; }





    void Start()
    {
        ActiveWeaponIndex = -1;
        weaponSwitchState = WeaponSwitchState.Down;
        
        playerController = GetComponent<PlayerController>();

        SetFov(DEFAULT_FOV);

        OnSwitchedToWeapon += OnWeaponSwitched;

        // Add starting weapons
        foreach (var weapon in startingWeapons)
        {
            AddWeapon(weapon);
        }
        SwitchWeapon(true);
    }

    void Update()
    {
        WeaponController activeWeapon = GetActiveWeapon();
        if (activeWeapon != null && activeWeapon.IsReloading)
            return;
        if (activeWeapon != null && weaponSwitchState == WeaponSwitchState.Up)
        {
            if (!activeWeapon.AutomaticReload && PlayerInputManager.Instance.GetReloadButtonDown() && activeWeapon.CurrentAmmoRatio < 1.0f)
            {
                IsAiming = false;
                return;
            }

            IsAiming = PlayerInputManager.Instance.GetAimInputHeld();
            bool hasFired = activeWeapon.HandleShootInputs(
                PlayerInputManager.Instance.GetFireInputDown(),
                PlayerInputManager.Instance.GetFireInputHeld(),
                PlayerInputManager.Instance.GetFireInputReleased());
            
            if (hasFired)
            {
                accumulatedRecoil += Vector3.back * activeWeapon.RecoilForce;
                accumulatedRecoil = Vector3.ClampMagnitude(accumulatedRecoil, MAX_RECOIL_DISTANCE);
            }
        }
        if (!IsAiming &&
            (activeWeapon == null || !activeWeapon.IsCharging) &&
            (weaponSwitchState == WeaponSwitchState.Up || weaponSwitchState == WeaponSwitchState.Down))
        {
            int switchWeaponInput = PlayerInputManager.Instance.GetSwitchWeaponInput();
            if (switchWeaponInput != 0)
            {
                bool switchUp = switchWeaponInput > 0;
                SwitchWeapon(switchUp);
            }
            else
            {
                switchWeaponInput = PlayerInputManager.Instance.GetSelectWeaponInput();
                if (switchWeaponInput != 0)
                {
                    if (GetWeaponAtSlotIndex(switchWeaponInput - 1) != null)
                        SwitchToWeaponIndex(switchWeaponInput - 1);
                }
            }
        }
        IsPointingAtEnemy = false;
        if (activeWeapon)
        {
            if (Physics.Raycast(weaponCamera.transform.position, weaponCamera.transform.forward, out RaycastHit hit,
                1000, -1, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.GetComponentInParent<HealthController>() != null)
                {
                    IsPointingAtEnemy = true;
                }
            }
        }
    }

    
    void LateUpdate()
    {
        UpdateWeaponAiming();
        UpdateWeaponBob();
        UpdateWeaponRecoil();
        UpdateWeaponSwitching();

        // Set final weapon socket position based on all the combined animation influences
        weaponParentSocket.localPosition =
            weaponMainLocalPosition + weaponBobLocalPosition + weaponRecoilLocalPosition;
    }
    
    public void SetFov(float fov)
    {
        playerController.MainCamera.fieldOfView = fov;
        weaponCamera.fieldOfView = fov * WEAPON_FOV_MULTIPLIER;
    }
    
    public void SwitchWeapon(bool ascendingOrder)
    {
        int newWeaponIndex = -1;
        int closestSlotDistance = weaponSlots.Length;
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (i != ActiveWeaponIndex && GetWeaponAtSlotIndex(i) != null)
            {
                int distanceToActiveIndex = GetDistanceBetweenWeaponSlots(ActiveWeaponIndex, i, ascendingOrder);

                if (distanceToActiveIndex < closestSlotDistance)
                {
                    closestSlotDistance = distanceToActiveIndex;
                    newWeaponIndex = i;
                }
            }
        }

        // Handle switching to the new weapon index
        SwitchToWeaponIndex(newWeaponIndex);
    }
    
    public void SwitchToWeaponIndex(int newWeaponIndex, bool force = false)
    {
        if (force || (newWeaponIndex != ActiveWeaponIndex && newWeaponIndex >= 0))
        {
            weaponSwitchNewWeaponIndex = newWeaponIndex;
            timeStartedWeaponSwitch = Time.time;
            if (GetActiveWeapon() == null)
            {
                weaponMainLocalPosition = downWeaponPosition.localPosition;
                weaponSwitchState = WeaponSwitchState.PutUpNew;
                ActiveWeaponIndex = weaponSwitchNewWeaponIndex;

                WeaponController newWeapon = GetWeaponAtSlotIndex(weaponSwitchNewWeaponIndex);
                if (OnSwitchedToWeapon != null)
                {
                    OnSwitchedToWeapon.Invoke(newWeapon);
                }
            }
            else
            {
                weaponSwitchState = WeaponSwitchState.PutDownPrevious;
            }
        }
    }

    public WeaponController HasWeapon(WeaponController weaponPrefab)
    {
        for (var index = 0; index < weaponSlots.Length; index++)
        {
            var w = weaponSlots[index];
            if (w != null && w.SourcePrefab == weaponPrefab.gameObject)
            {
                return w;
            }
        }
        return null;
    }
    
    void UpdateWeaponAiming()
    {
        if (weaponSwitchState == WeaponSwitchState.Up)
        {
            WeaponController activeWeapon = GetActiveWeapon();
            if (IsAiming && activeWeapon)
            {
                weaponMainLocalPosition = Vector3.Lerp(weaponMainLocalPosition,
                    aimingWeaponPosition.localPosition + activeWeapon.AimOffset,
                    AimingAnimationSpeed * Time.deltaTime);
                SetFov(Mathf.Lerp(playerController.MainCamera.fieldOfView,
                    activeWeapon.AimZoomRatio * DEFAULT_FOV, AimingAnimationSpeed * Time.deltaTime));
            }
            else
            {
                weaponMainLocalPosition = Vector3.Lerp(weaponMainLocalPosition,
                    defaultWeaponPosition.localPosition, AimingAnimationSpeed * Time.deltaTime);
                SetFov(Mathf.Lerp(playerController.MainCamera.fieldOfView, DEFAULT_FOV,
                    AimingAnimationSpeed * Time.deltaTime));
            }
        }
    }
    
    void UpdateWeaponBob()
    {
        if (Time.deltaTime > 0f)
        {
            Vector3 playerCharacterVelocity =
                (playerController.transform.position - lastCharacterPosition) / Time.deltaTime;
            float characterMovementFactor = 0f;
            if (playerController.IsGrounded)
            {
                characterMovementFactor =
                    Mathf.Clamp01(playerCharacterVelocity.magnitude /
                                  (playerController.MaxSpeedOnGound *
                                   playerController.SprintSpeedModifier));
            }

            weaponBobFactor =
                Mathf.Lerp(weaponBobFactor, characterMovementFactor, BOB_SHARPNESS * Time.deltaTime);
            float bobAmount = IsAiming ? AIMING_BOB_AMOUNT : DEFAULT_BOB_AMOUNT;
            float frequency = BOB_FREQUENCY;
            float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * weaponBobFactor;
            float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount *
                              weaponBobFactor;
            weaponBobLocalPosition.x = hBobValue;
            weaponBobLocalPosition.y = Mathf.Abs(vBobValue);

            lastCharacterPosition = playerController.transform.position;
        }
    }

    void UpdateWeaponRecoil()
    {
        if (weaponRecoilLocalPosition.z >= accumulatedRecoil.z * 0.99f)
        {
            weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, accumulatedRecoil,
                RECOIL_SHARPNESS * Time.deltaTime);
        }
        else
        {
            weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, Vector3.zero,
                RECOIL_RESTITUTION_SHARPNESS * Time.deltaTime);
            accumulatedRecoil = weaponRecoilLocalPosition;
        }
    }
    
    void UpdateWeaponSwitching()
    {
        float switchingTimeFactor = 0f;
        if (WEAPON_SWITCH_DELAY == 0f)
        {
            switchingTimeFactor = 1f;
        }
        else
        {
            switchingTimeFactor = Mathf.Clamp01((Time.time - timeStartedWeaponSwitch) / WEAPON_SWITCH_DELAY);
        }
        if (switchingTimeFactor >= 1f)
        {
            if (weaponSwitchState == WeaponSwitchState.PutDownPrevious)
            {
                WeaponController oldWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                if (oldWeapon != null)
                {
                    oldWeapon.ShowWeapon(false);
                }

                ActiveWeaponIndex = weaponSwitchNewWeaponIndex;
                switchingTimeFactor = 0f;
                WeaponController newWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                if (OnSwitchedToWeapon != null)
                {
                    OnSwitchedToWeapon.Invoke(newWeapon);
                }

                if (newWeapon)
                {
                    timeStartedWeaponSwitch = Time.time;
                    weaponSwitchState = WeaponSwitchState.PutUpNew;
                }
                else
                {
                    weaponSwitchState = WeaponSwitchState.Down;
                }
            }
            else if (weaponSwitchState == WeaponSwitchState.PutUpNew)
            {
                weaponSwitchState = WeaponSwitchState.Up;
            }
        }
        if (weaponSwitchState == WeaponSwitchState.PutDownPrevious)
        {
            weaponMainLocalPosition = Vector3.Lerp(defaultWeaponPosition.localPosition,
                downWeaponPosition.localPosition, switchingTimeFactor);
        }
        else if (weaponSwitchState == WeaponSwitchState.PutUpNew)
        {
            weaponMainLocalPosition = Vector3.Lerp(downWeaponPosition.localPosition,
                defaultWeaponPosition.localPosition, switchingTimeFactor);
        }
    }
    
    public bool AddWeapon(WeaponController weaponPrefab)
    {
        if (HasWeapon(weaponPrefab) != null)
        {
            return false;
        }
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == null)
            {
                WeaponController weaponInstance = Instantiate(weaponPrefab, weaponParentSocket);
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.localRotation = Quaternion.identity;
                weaponInstance.Owner = gameObject;
                weaponInstance.SourcePrefab = weaponPrefab.gameObject;
                weaponInstance.ShowWeapon(false);
                int layerIndex = Mathf.RoundToInt(Mathf.Log(fpsWeaponLayer.value, 2));
                foreach (Transform t in weaponInstance.gameObject.GetComponentsInChildren<Transform>(true))
                {
                    t.gameObject.layer = layerIndex;
                }

                weaponSlots[i] = weaponInstance;

                if (OnAddedWeapon != null)
                {
                    OnAddedWeapon.Invoke(weaponInstance, i);
                }

                return true;
            }
        }
        if (GetActiveWeapon() == null)
        {
            SwitchWeapon(true);
        }

        return false;
    }

    public bool RemoveWeapon(WeaponController weaponInstance)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] == weaponInstance)
            {
                weaponSlots[i] = null;

                if (OnRemovedWeapon != null)
                {
                    OnRemovedWeapon.Invoke(weaponInstance, i);
                }

                Destroy(weaponInstance.gameObject);
                if (i == ActiveWeaponIndex)
                {
                    SwitchWeapon(true);
                }
                return true;
            }
        }

        return false;
    }

    public WeaponController GetActiveWeapon()
    {
        return GetWeaponAtSlotIndex(ActiveWeaponIndex);
    }

    public WeaponController GetWeaponAtSlotIndex(int index)
    {
        if (index >= 0 &&
            index < weaponSlots.Length)
        {
            return weaponSlots[index];
        }
        return null;
    }
    int GetDistanceBetweenWeaponSlots(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
    {
        int distanceBetweenSlots = 0;

        if (ascendingOrder)
        {
            distanceBetweenSlots = toSlotIndex - fromSlotIndex;
        }
        else
        {
            distanceBetweenSlots = -1 * (toSlotIndex - fromSlotIndex);
        }

        if (distanceBetweenSlots < 0)
        {
            distanceBetweenSlots = weaponSlots.Length + distanceBetweenSlots;
        }

        return distanceBetweenSlots;
    }

    void OnWeaponSwitched(WeaponController newWeapon)
    {
        if (newWeapon != null)
        {
            newWeapon.ShowWeapon(true);
        }
    }
    
}

