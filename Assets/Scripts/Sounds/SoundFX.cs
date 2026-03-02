using UnityEngine;

namespace FG
{
    public class SoundFX : MonoBehaviour
    {
        [HideInInspector] public AudioSource audioSource;

        // ------------
        // UNITY EVENTS
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            audioSource.Play();
        }

        private void OnDisable()
        {
            audioSource.Stop();
        }

        // ------------
        // MAIN METHODS
        public void Init(SoundFXInfo info)
        {
            audioSource.clip = info.audioClip;
            audioSource.volume = info.volume;
            audioSource.pitch = info.GetPitch();
        }
    }
}
