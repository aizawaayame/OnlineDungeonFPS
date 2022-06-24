using System;
using Common.Data;
using Entities;
using Managers;
using Models;
using TMPro;
using UnityEngine;

namespace GameObjects
{
    public struct Crosshair
    {
        public Sprite crosshairSprite;
        public int crosshairSize;
        public Color crosshairColor;
    }
    
    /// <summary>
    /// Attach to weapon game object.
    /// </summary>
    public class WeaponController : MonoBehaviour
    {

        #region Public Fields
        [Header("Information")]
        public Sprite weaponIcon;

        public Crosshair crosshairDefault;
        public Crosshair crosshairTargetInSight;
        
        [Header("Internal References")]
        public GameObject weaponRoot;
        public Transform weaponMuzzleTransform;
        
        [Header("Shoot Parameters")]
        public ProjectileBase ProjPrefab;
        public float delayBetweenShots = 0.5f;
        public float ProjSpreadAngle = 0f;
        [Tooltip("Amount of bullets per shot")]
        public int ProjPerShot = 1;
        public float manaPerShot = 1;
        [Range(0f,2f)]
        public float recoilForce = 1;
        [Range(0f,1f)]
        public float aimZoomRatio = 1f;
        public Vector3 aimOffset;

        [Header("Ammo Parameters")]
        [Tooltip("Amount of ammo reloaded per second")]
        public float manaRestoreRate = 1f;
        public float manaRestoreDelay = 2f;

        [Header("Charging parameters (charging weapons only)")]
        
        [Tooltip("Trigger a shot when maximum charge is reached")]
        public bool AutomaticReleaseOnCharged;
        
        [Tooltip("Duration to reach maximum charge")]
        public float MaxChargeDuration = 2f;
        [Tooltip("Initial mana used when starting to charge")]
        public float ManaUsedOnStartCharge = 1f;
        [Tooltip("Additional Mana used when charge reaches its maximum")]
        public float ManaUsageAmountWhileCharging = 1f;
        
        public float LastChargeStartTime { get; private set; }
        [Header("Audio & Visual")] 
        public GameObject muzzleFlash;
        #endregion

        #region Public Properties

        public Weapon Weapon { get; set; }

        public string weaponName
        {
            get => DataManager.Instance.WeaponDefines[Weapon.Id].Name;
        }

        public GameObject OwnerGameObject { get; set; }
        public Character OwnerCharacter { get; set; }
        public Transform WeaponCameraTransform { get; set; }
        public GameObject SourcePrefab { get; set; }
        public bool IsCharging { get; private set; }
        public float CurrentAmmoRatio { get; private set; }
        public bool IsWeaponActive { get; private set; }
        public bool IsRestoring { get; private set; }
        public float CurrentCharge { get; private set; }
        public Vector3 MuzzleWorldVelocity { get; private set; }
        #endregion

        #region Private Methods
        
        float CurrentMana { get => User.Instance.MP; set => User.Instance.MP = value; }
        float MaxMana { get => User.Instance.Character.Attributes.MaxMP; }
        float lastTimeShot = Mathf.NegativeInfinity;
        Vector3 lastMuzzlePosition;
        #endregion

        #region Private Methods

        void Awake()
        {
            lastMuzzlePosition = weaponMuzzleTransform.position;
        }

        void Update()
        {
            RestoreMana();
            UpdateCharge();
            if (Time.deltaTime > 0)
            {
                MuzzleWorldVelocity = (weaponMuzzleTransform.position - lastMuzzlePosition) / Time.deltaTime;
                lastMuzzlePosition = weaponMuzzleTransform.position;
            }
        }
        void RestoreMana( )
        {
            if (lastTimeShot + manaRestoreDelay < Time.time && CurrentMana < MaxMana && !IsCharging)
            {
                CurrentMana = Mathf.Clamp(CurrentMana + manaRestoreRate * Time.deltaTime, 0, MaxMana);
                IsRestoring = true;
            }
            else
            {
                IsRestoring = false;
            }
        }
        void UpdateCharge()
        {
            if (IsCharging)
            {
                if (CurrentCharge < 1f)
                {
                    float chargeLeft = 1f - CurrentCharge;
                    float chargeAdded = 0f;
                    if (MaxChargeDuration <= 0f)
                    {
                        chargeAdded = chargeLeft;
                    }
                    else
                    {
                        chargeAdded = (1f / MaxChargeDuration) * Time.deltaTime;
                    }
                    chargeAdded = Mathf.Clamp(chargeAdded, 0f, chargeLeft);

                    float manaThisChargeWouldRequire = chargeAdded * ManaUsageAmountWhileCharging;
                    if (manaThisChargeWouldRequire <= CurrentMana )
                    {
                        UseMana(manaThisChargeWouldRequire);
                        CurrentCharge = Mathf.Clamp01(CurrentCharge + chargeAdded);
                    }
                }
            }
        }
        void UseMana(float mana)
        {
            CurrentMana = Mathf.Clamp(CurrentMana - mana, 0f, MaxMana);
            lastTimeShot = Time.time;
        }
        void HandleShoot()
        {
            float projPerShotFinal = Weapon.Type == WeaponType.Charge
                ? CurrentCharge * ProjPerShot
                : ProjPerShot;
            
            // spawn all bullets with random direction
            for (int i = 0; i < projPerShotFinal; i++)
            {
                Vector3 shotDirection = GetShotDirectionWithinSpread(weaponMuzzleTransform);
                ProjectileBase newProj = Instantiate(ProjPrefab, weaponMuzzleTransform.position, Quaternion.LookRotation(shotDirection));
                newProj.Shoot(this);
            }
            
            // muzzle flash
            if (muzzleFlash != null)
            {
                GameObject muzzleFlashInstance = Instantiate(muzzleFlash,weaponMuzzleTransform.position,
                    weaponMuzzleTransform.rotation,weaponMuzzleTransform.transform);
                Destroy(muzzleFlashInstance, 2f);
            }

            lastTimeShot = Time.time;
        }

        bool TryShoot()
        {
            if (CurrentMana >= manaPerShot
                && lastTimeShot + delayBetweenShots < Time.time)    
            {
                HandleShoot();
                CurrentMana -= manaPerShot;
                return true;
            }
            return false;
        }
        bool TryBeginCharge()
        {
            if (!IsCharging
                && CurrentMana >= ManaUsedOnStartCharge
                && lastTimeShot + delayBetweenShots < Time.time)
            {
                UseMana(ManaUsageAmountWhileCharging);
                LastChargeStartTime = Time.time;
                IsCharging = true;
                return true;
            }
            return false;
        }

        bool TryReleaseCharge()
        {
            if (IsCharging)
            {
                HandleShoot();
                CurrentCharge = 0f;
                IsCharging = false;
                return true;
            }
            return false;
        }
        Vector3 GetShotDirectionWithinSpread(Transform transform)
        {
            float spreadAngleRatio = ProjSpreadAngle / 180f;
            Vector3 spreadWorldDirection = Vector3.Slerp(transform.forward, UnityEngine.Random.insideUnitCircle, spreadAngleRatio);
            return spreadWorldDirection;
        }

        #endregion
        
        #region Public Methods

        public void SetActive(bool active)
        {
            weaponRoot.SetActive(active);
            IsWeaponActive = active;
        }

        public bool HandleShootInputs(bool inputDown, bool inputHeld, bool inputUp)
        {
            switch (Weapon.Type)
            {
                case WeaponType.Manual:
                    if (inputDown)
                    {
                        return TryShoot();
                    }
                    return false;
                case WeaponType.Automatic:
                    if (inputHeld)
                    {
                        return TryShoot();
                    }
                    return false;
                case WeaponType.Charge:
                    if (inputHeld)
                    {
                        TryBeginCharge();
                    }
                    if (inputUp || (AutomaticReleaseOnCharged && CurrentCharge >= 1f))
                    {
                        return TryReleaseCharge();
                    }
                    return false;
                default:
                    return false;
            }
        }
        #endregion
    }
}
