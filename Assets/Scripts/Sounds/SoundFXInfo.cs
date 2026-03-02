using UnityEngine;

namespace FG
{
    [CreateAssetMenu(menuName = "Scriptable Object/Sound/SoundFXInfo")]
    public class SoundFXInfo : ScriptableObject
    {
        [Header("Config")]
        [SerializeField] private string soundName;
        public AudioClip audioClip;
        [Range(0.0f, 1.0f)]
        public float volume = 1.0f;

        [Header("Pooling")]
        public GameObject prefab;

        [Header("Pitch Variation")]
        [Range(0.1f, 1.5f)]
        [SerializeField] private float pitchMin = 0.8f;
        [Range(0.1f, 1.5f)]
        [SerializeField] private float pitchMax = 1.2f;

        // -------
        // GETTERS
        public float GetPitch()
        {
            return Random.Range(pitchMin, pitchMax);
        }
    }
}
