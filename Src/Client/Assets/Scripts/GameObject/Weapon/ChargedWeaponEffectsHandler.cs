using Utilities;
using UnityEngine;

public class ChargedWeaponEffectsHandler : MonoBehaviour
{

    #region Fields

    public GameObject ChargingObject;
    public GameObject SpinningFrame;
    public MinMaxVector3 Scale;
    public GameObject DiskOrbitParticlePrefab;
    public Vector3 Offset;
    public Transform ParentTransform;
    public MinMaxFloat OrbitY;
    public MinMaxVector3 Radius;
    public MinMaxFloat SpinningSpeed;
    public AudioClip ChargeSound;
    public AudioClip LoopChargeWeaponSfx;
    public float FadeLoopDuration = 0.5f;
    public bool UseProceduralPitchOnLoopSfx;
    public float MaxProceduralPitchValue = 2.0f;

    ParticleSystem diskOrbitParticle;
    WeaponController weaponController;
    ParticleSystem.VelocityOverLifetimeModule velocityOverTimeModule;

    AudioSource audioSource;
    AudioSource audioSourceLoop;

    float lastChargeTriggerTimestamp;
    float chargeRatio;
    float endchargeTime;
    #endregion

    #region Properties

    public GameObject ParticleInstance { get; set; }

    #endregion
    
    void Awake()
    {
        lastChargeTriggerTimestamp = 0.0f;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = ChargeSound;
        audioSource.playOnAwake = false;
        
        audioSourceLoop = gameObject.AddComponent<AudioSource>();
        audioSourceLoop.clip = LoopChargeWeaponSfx;
        audioSourceLoop.playOnAwake = false;
        audioSourceLoop.loop = true;

    }

    void SpawnParticleSystem()
    {
        ParticleInstance = Instantiate(DiskOrbitParticlePrefab,
            ParentTransform != null ? ParentTransform : transform);
        ParticleInstance.transform.localPosition += Offset;

        FindReferences();
    }

    public void FindReferences()
    {
        diskOrbitParticle = ParticleInstance.GetComponent<ParticleSystem>();
        weaponController = GetComponent<WeaponController>();
        velocityOverTimeModule = diskOrbitParticle.velocityOverLifetime;
    }

    void Update()
    {
        if (ParticleInstance == null)
            SpawnParticleSystem();

        diskOrbitParticle.gameObject.SetActive(weaponController.IsWeaponActive);
        chargeRatio = weaponController.CurrentCharge;

        ChargingObject.transform.localScale = Scale.GetValueFromRatio(chargeRatio);
        if (SpinningFrame != null)
        {
            SpinningFrame.transform.localRotation *= Quaternion.Euler(0,
                SpinningSpeed.GetValueFromRatio(chargeRatio) * Time.deltaTime, 0);
        }

        velocityOverTimeModule.orbitalY = OrbitY.GetValueFromRatio(chargeRatio);
        diskOrbitParticle.transform.localScale = Radius.GetValueFromRatio(chargeRatio * 1.1f);

        // update sound's volume and pitch 
        if (chargeRatio > 0)
        {
            if (!audioSourceLoop.isPlaying &&
                weaponController.LastChargeTriggerTimestamp > lastChargeTriggerTimestamp)
            {
                lastChargeTriggerTimestamp = weaponController.LastChargeTriggerTimestamp;
                if (!UseProceduralPitchOnLoopSfx)
                {
                    endchargeTime = Time.time + ChargeSound.length;
                    audioSource.Play();
                }

                audioSourceLoop.Play();
            }

            if (!UseProceduralPitchOnLoopSfx)
            {
                float volumeRatio =
                    Mathf.Clamp01((endchargeTime - Time.time - FadeLoopDuration) / FadeLoopDuration);
                audioSource.volume = volumeRatio;
                audioSourceLoop.volume = 1 - volumeRatio;
            }
            else
            {
                audioSourceLoop.pitch = Mathf.Lerp(1.0f, MaxProceduralPitchValue, chargeRatio);
            }
        }
        else
        {
            audioSource.Stop();
            audioSourceLoop.Stop();
        }
    }
}

