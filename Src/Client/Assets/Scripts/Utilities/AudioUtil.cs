using Managers;
using UnityEngine;
using UnityEngine.Audio;
namespace Utilities
{
    public static class AudioUtil
    {
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
            var groups = AudioManager.Instance.FindMatchingGroups(group.ToString());

            if (groups.Length > 0)
                return groups[0];

            Debug.LogWarning("Didn't find audio group for " + group.ToString());
            return null;
        }

        public static void SetMasterVolume(float value)
        {
            if (value <= 0)
                value = 0.001f;
            float valueInDb = Mathf.Log10(value) * 20;

            AudioManager.Instance.SetFloat("MasterVolume", valueInDb);
        }

        public static float GetMasterVolume()
        {
            AudioManager.Instance.GetFloat("MasterVolume", out var valueInDb);
            return Mathf.Pow(10f, valueInDb / 20.0f);
        }
    }
}
