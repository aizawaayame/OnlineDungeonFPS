﻿using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Audio;
using AudioManager = Managers.AudioManager;

namespace Utilities
{
    public static class AudioUtility
    {
        static AudioManager audioManager;

        public enum AudioGroups
        {
            DamageTick,
            Impact,
            EnemyDetection,
            Pickup,
            WeaponShoot,
            WeaponOverheat,
            WeaponChargeBuildup,
            WeaponChargeLoop,
            HUDVictory,
            HUDObjective,
            EnemyAttack
        }

        public static void CreateSFX(AudioClip clip, Vector3 position, AudioGroups audioGroup, float spatialBlend,
            float rolloffDistanceMin = 1f)
        {
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.transform.position = position;
            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = spatialBlend;
            source.minDistance = rolloffDistanceMin;
            source.Play();

            source.outputAudioMixerGroup = GetAudioGroup(audioGroup);

            TimedSelfDestruct timedSelfDestruct = impactSfxInstance.AddComponent<TimedSelfDestruct>();
            timedSelfDestruct.LifeTime = clip.length;
        }

        public static AudioMixerGroup GetAudioGroup(AudioGroups group)
        {
            if (audioManager == null)
                audioManager = GameObject.FindObjectOfType<AudioManager>();

            var groups = audioManager.FindMatchingGroups(group.ToString());

            if (groups.Length > 0)
                return groups[0];

            Debug.LogWarning("Didn't find audio group for " + group.ToString());
            return null;
        }

        public static void SetMasterVolume(float value)
        {
            if (audioManager == null)
                audioManager = GameObject.FindObjectOfType<AudioManager>();

            if (value <= 0)
                value = 0.001f;
            float valueInDb = Mathf.Log10(value) * 20;

            audioManager.SetFloat("MasterVolume", valueInDb);
        }

        public static float GetMasterVolume()
        {
            if (audioManager == null)
                audioManager = GameObject.FindObjectOfType<AudioManager>();

            audioManager.GetFloat("MasterVolume", out var valueInDb);
            return Mathf.Pow(10f, valueInDb / 20.0f);
        }
    }
}
