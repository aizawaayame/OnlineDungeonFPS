using UnityEngine;
using System.Collections.Generic;
using Utilities;


public class OverheatBehavior : MonoBehaviour
{

    #region Internal Structure

    [System.Serializable]
    public struct RendererIndexData
    {
        public Renderer Renderer;
        public int MaterialIndex;

        public RendererIndexData(Renderer renderer, int index)
        {
            this.Renderer = renderer;
            this.MaterialIndex = index;
        }
    }

    #endregion

    #region Fields
    
    public ParticleSystem SteamVfx;
    public float SteamVfxEmissionRateMax = 8f;
    public Gradient OverheatGradient;
    public Material OverheatingMaterial;
    public AudioClip CoolingCellsSound;
    public AnimationCurve AmmoToVolumeRatioCurve;
    
    WeaponController weapon;
    AudioSource audioSource;
    List<RendererIndexData> overheatingRenderersData;
    MaterialPropertyBlock overheatMaterialPropertyBlock;
    float lastAmmoRatio;
    ParticleSystem.EmissionModule steamVfxEmissionModule;

    #endregion
    
    void Awake()
    {
        var emissionModule = SteamVfx.emission;
        emissionModule.rateOverTimeMultiplier = 0f;

        overheatingRenderersData = new List<RendererIndexData>();
        foreach (var renderer in GetComponentsInChildren<Renderer>(true))
        {
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                if (renderer.sharedMaterials[i] == OverheatingMaterial)
                    overheatingRenderersData.Add(new RendererIndexData(renderer, i));
            }
        }

        overheatMaterialPropertyBlock = new MaterialPropertyBlock();
        steamVfxEmissionModule = SteamVfx.emission;

        weapon = GetComponent<WeaponController>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = CoolingCellsSound;
    }

    void Update()
    {
        float currentAmmoRatio = weapon.CurrentAmmoRatio;
        if (currentAmmoRatio != lastAmmoRatio)
        {
            overheatMaterialPropertyBlock.SetColor("_EmissionColor",
                OverheatGradient.Evaluate(1f - currentAmmoRatio));

            foreach (var data in overheatingRenderersData)
            {
                data.Renderer.SetPropertyBlock(overheatMaterialPropertyBlock, data.MaterialIndex);
            }

            steamVfxEmissionModule.rateOverTimeMultiplier = SteamVfxEmissionRateMax * (1f - currentAmmoRatio);
        }

        if (CoolingCellsSound)
        {
            if (!audioSource.isPlaying
                && currentAmmoRatio != 1
                && weapon.IsWeaponActive
                && weapon.IsCooling)
            {
                audioSource.Play();
            }
            else if (audioSource.isPlaying
                     && (currentAmmoRatio == 1 || !weapon.IsWeaponActive || !weapon.IsCooling))
            {
                audioSource.Stop();
                return;
            }

            audioSource.volume = AmmoToVolumeRatioCurve.Evaluate(1 - currentAmmoRatio);
        }

        lastAmmoRatio = currentAmmoRatio;
    }
}
