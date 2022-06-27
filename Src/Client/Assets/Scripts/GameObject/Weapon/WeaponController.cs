using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using Entities;
public enum WeaponShootType
{
    Manual,
    Automatic,
    Charge,
}
[System.Serializable]
public struct CrosshairData
{
    public Sprite CrosshairSprite;
    public int CrosshairSize;
    public Color CrosshairColor;
}

[RequireComponent(typeof(AudioSource))]
public class WeaponController : MonoBehaviour
{
    public UnityAction OnShoot;
    public event Action OnShootProcessed;
    
    #region Fields
    
    [SerializeField] string weaponName;
    [SerializeField] Sprite weaponIcon;
    [SerializeField] public CrosshairData crosshairDataDefault;
    [SerializeField] CrosshairData crosshairDataTargetInSight;
    [SerializeField] GameObject weaponRoot;
    [SerializeField] Transform weaponMuzzle;
    [SerializeField] WeaponShootType shootType;
    [SerializeField] ProjectileBase projectilePrefab;
    [SerializeField] float delayBetweenShots = 0.5f;
    [SerializeField] float projectileSpreadAngle = 0f;
    [SerializeField] int projectilePerShot = 1;
    [SerializeField] float recoilForce = 1;
    [SerializeField] float aimZoomRatio = 1f;
    [SerializeField] Vector3 aimOffset;
    [SerializeField] bool automaticReload = true;
    [SerializeField] int clipSize = 30;
    [SerializeField] float ammoReloadRate = 1f;
    [SerializeField] float ammoReloadDelay = 2f;
    [SerializeField] int maxAmmo = 8;
    [SerializeField] bool automaticReleaseOnCharged;
    [SerializeField] float maxChargeDuration = 2f;
    [SerializeField] float ammoUsedOnStartCharge = 1f;
    [SerializeField] float ammoUsageRateWhileCharging = 1f;
    [SerializeField] GameObject muzzleFlashPrefab;
    
    [SerializeField] bool unparentMuzzleFlash;
    [SerializeField] AudioClip shootSfx;
    [SerializeField] AudioClip changeWeaponSfx;
    [SerializeField] bool useContinuousShootSound = false;
    [SerializeField] AudioClip continuousShootStartSfx;
    [SerializeField] AudioClip continuousShootLoopSfx;
    [SerializeField] AudioClip continuousShootEndSfx;
    
    AudioSource continuousShootAudioSource = null;
    AudioSource shootAudioSource;
    bool wantsToShoot = false;
    float currentAmmo;
    float lastTimeShot = Mathf.NegativeInfinity;
    Vector3 lastMuzzlePosition;
    #endregion

    #region Properties
    public float LastChargeTriggerTimestamp { get; private set; }
    public GameObject Owner { get; set; }
    public GameObject SourcePrefab { get; set; }
    public bool IsCharging { get; private set; }
    public float CurrentAmmoRatio { get; private set; }
    public bool IsWeaponActive { get; private set; }
    public bool IsCooling { get; private set; }
    public float CurrentCharge { get; private set; }
    public bool AutomaticReload { get => automaticReload; }
    public Vector3 AimOffset { get => aimOffset; }
    public float AimZoomRatio { get => aimZoomRatio; }
    public float RecoilForce { get => recoilForce; }
    public Vector3 MuzzleWorldVelocity { get; private set; }
    public float GetAmmoNeededToShoot() =>
        (shootType != WeaponShootType.Charge ? 1f : Mathf.Max(1f, ammoUsedOnStartCharge)) /
        (maxAmmo * projectilePerShot);
    public int GetCurrentAmmo() => Mathf.FloorToInt(currentAmmo);
    public bool IsReloading { get; private set; }
    #endregion
    
    void Awake()
    {
        currentAmmo = maxAmmo;
        lastMuzzlePosition = weaponMuzzle.position;
        shootAudioSource = GetComponent<AudioSource>();


        if (useContinuousShootSound)
        {
            continuousShootAudioSource = gameObject.AddComponent<AudioSource>();
            continuousShootAudioSource.playOnAwake = false;
            continuousShootAudioSource.clip = continuousShootLoopSfx;
            continuousShootAudioSource.outputAudioMixerGroup =
                AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.WeaponShoot);
            continuousShootAudioSource.loop = true;
        }
        
    }

    void PlaySFX(AudioClip sfx) => AudioUtility.CreateSFX(sfx, transform.position, AudioUtility.AudioGroups.WeaponShoot, 0.0f);


    void Reload()
    {
        IsReloading = false;
    }
    
    void Update()
    {
        UpdateAmmo();
        UpdateCharge();
        UpdateContinuousShootSound();

        if (Time.deltaTime > 0)
        {
            MuzzleWorldVelocity = (weaponMuzzle.position - lastMuzzlePosition) / Time.deltaTime;
            lastMuzzlePosition = weaponMuzzle.position;
        }
    }

    void UpdateAmmo()
    {
        if (AutomaticReload && lastTimeShot + ammoReloadDelay < Time.time && currentAmmo < maxAmmo && !IsCharging)
        {

            currentAmmo += ammoReloadRate * Time.deltaTime;
            currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
            IsCooling = true;
        }
        else
        {
            IsCooling = false;
        }
        if (maxAmmo == Mathf.Infinity)
        {
            CurrentAmmoRatio = 1f;
        }
        else
        {
            CurrentAmmoRatio = currentAmmo / maxAmmo;
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
                if (maxChargeDuration <= 0f)
                {
                    chargeAdded = chargeLeft;
                }
                else
                {
                    chargeAdded = (1f / maxChargeDuration) * Time.deltaTime;
                }
                chargeAdded = Mathf.Clamp(chargeAdded, 0f, chargeLeft);
                float ammoThisChargeWouldRequire = chargeAdded * ammoUsageRateWhileCharging;
                if (ammoThisChargeWouldRequire <= currentAmmo)
                {
                    UseAmmo(ammoThisChargeWouldRequire);
                    CurrentCharge = Mathf.Clamp01(CurrentCharge + chargeAdded);
                }
            }
        }
    }

    void UpdateContinuousShootSound()
    {
        if (useContinuousShootSound)
        {
            if (wantsToShoot && currentAmmo >= 1f)
            {
                if (!continuousShootAudioSource.isPlaying)
                {
                    shootAudioSource.PlayOneShot(shootSfx);
                    shootAudioSource.PlayOneShot(continuousShootStartSfx);
                    continuousShootAudioSource.Play();
                }
            }
            else if (continuousShootAudioSource.isPlaying)
            {
                shootAudioSource.PlayOneShot(continuousShootEndSfx);
                continuousShootAudioSource.Stop();
            }
        }
    }

    public void ShowWeapon(bool show)
    {
        weaponRoot.SetActive(show);

        if (show && changeWeaponSfx)
        {
            shootAudioSource.PlayOneShot(changeWeaponSfx);
        }

        IsWeaponActive = show;
    }

    public void UseAmmo(float amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo - amount, 0f, maxAmmo);
        lastTimeShot = Time.time;
    }

    public bool HandleShootInputs(bool inputDown, bool inputHeld, bool inputUp)
    {
        wantsToShoot = inputDown || inputHeld;
        switch (shootType)
        {
            case WeaponShootType.Manual:
                if (inputDown)
                {
                    return TryShoot();
                }
                return false;
            case WeaponShootType.Automatic:
                if (inputHeld)
                {
                    return TryShoot();
                }
                return false;
            case WeaponShootType.Charge:
                if (inputHeld)
                {
                    TryBeginCharge();
                }
                if (inputUp || (automaticReleaseOnCharged && CurrentCharge >= 1f))
                {
                    return TryReleaseCharge();
                }
                return false;
            default:
                return false;
        }
    }
    
    bool TryShoot()
    {
        if (currentAmmo >= 1f
            && lastTimeShot + delayBetweenShots < Time.time)
        {
            HandleShoot();
            currentAmmo -= 1f;

            return true;
        }
        return false;
    }

    bool TryBeginCharge()
    {
        if (!IsCharging
            && currentAmmo >= ammoUsedOnStartCharge
            && Mathf.FloorToInt((currentAmmo - ammoUsedOnStartCharge) * projectilePerShot) > 0
            && lastTimeShot + delayBetweenShots < Time.time)
        {
            UseAmmo(ammoUsedOnStartCharge);

            LastChargeTriggerTimestamp = Time.time;
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

    void HandleShoot()
    {
        int bulletsPerShotFinal = shootType == WeaponShootType.Charge
            ? Mathf.CeilToInt(CurrentCharge * projectilePerShot)
            : projectilePerShot;
        for (int i = 0; i < bulletsPerShotFinal; i++)
        {
            Vector3 shotDirection = GetShotDirectionWithinSpread(weaponMuzzle);
            ProjectileBase newProjectile = Instantiate(projectilePrefab, weaponMuzzle.position,
                Quaternion.LookRotation(shotDirection));
            newProjectile.Shoot(this);
        }

        if (muzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, weaponMuzzle.position,
                weaponMuzzle.rotation, weaponMuzzle.transform);
            if (unparentMuzzleFlash)
            {
                muzzleFlashInstance.transform.SetParent(null);
            }

            Destroy(muzzleFlashInstance, 2f);
        }
        
        lastTimeShot = Time.time;
        if (shootSfx && !useContinuousShootSound)
        {
            shootAudioSource.PlayOneShot(shootSfx);
        }
        OnShoot?.Invoke();
        OnShootProcessed?.Invoke();
    }

    public Vector3 GetShotDirectionWithinSpread(Transform shootTransform)
    {
        float spreadAngleRatio = projectileSpreadAngle / 180f;
        Vector3 spreadWorldDirection = Vector3.Slerp(shootTransform.forward, UnityEngine.Random.insideUnitSphere,
            spreadAngleRatio);
        return spreadWorldDirection;
    }
}
