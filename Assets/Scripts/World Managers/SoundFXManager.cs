using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace FG
{
    public class SoundFXManager : SingletonMonoBehaviour<SoundFXManager>
    {
        [Header("Mixer Config")]
        [SerializeField] private AudioMixerGroup soundsMasterMixer;

        [Header("Volume Config")]
        [SerializeField] private float soundsMasterVolume = 8.0f;
        [SerializeField] private string soundsMasterVolumeParam = "soundsVolume";

        [Header("Sound Effects")]
        public SoundFXInfo doorOpenSoundFX;

        // ------------
        // UNITY EVENTS
        protected override void Awake()
        {
            base.Awake();

            SetSoundsMasterVolume(soundsMasterVolume);
        }

        // ------------
        // MAIN METHODS
        public void PlaySoundFX(SoundFXInfo info)
        {
            SoundFX soundFX = (SoundFX)PoolManager.instance.ReuseObject(info.prefab, Vector3.zero, Quaternion.identity);
            soundFX.Init(info);
            soundFX.gameObject.SetActive(true);
            StartCoroutine(StopSoundFXCoroutine(soundFX));
        }

        // ----------
        // COROUTINES
        private IEnumerator StopSoundFXCoroutine(SoundFX soundFX)
        {
            yield return new WaitForSeconds(soundFX.audioSource.clip.length);
            soundFX.gameObject.SetActive(false);
            yield return null;
        }

        // --------------
        // VOLUME RELATED
        private void SetSoundsMasterVolume(float volume)
        {
            float muteDecibels = -80.0f;
            
            if (volume <= 0.0f)
            {
                soundsMasterMixer.audioMixer.SetFloat(soundsMasterVolumeParam, muteDecibels);
                return;
            }

            float volumeDecibels = Helpers.Linear2Decibels(volume);
            soundsMasterMixer.audioMixer.SetFloat(soundsMasterVolumeParam, volumeDecibels);
        }
    }
}
