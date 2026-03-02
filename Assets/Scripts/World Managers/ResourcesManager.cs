using UnityEngine;
using UnityEngine.Audio;

namespace FG
{
    public class ResourcesManager : MonoBehaviour
    {
        // ------------------
        // SINGLETON FEATURES
        private static ResourcesManager Instance;
        [HideInInspector] public static ResourcesManager instance
        { 
            get
            {
                if (Instance == null)
                    Instance = Resources.Load<ResourcesManager>("Resources Manager");
                return Instance;
            }
        }

        // ---------
        // RESOURCES
        [Header("Special Prefabs")]
        public GameObject playerPrefab;

        [Header("Resources - Room Graph Editor")]
        public RoomNodeTypeList roomNodeTypes;

        [Header("Material Variables")]
        public string materialOpacityString = "Alpha_Slider";

        [Header("Materials")]
        public Material dimmedMaterial;
        public Material defaultLitMaterial;

        [Header("Shaders")]
        public Shader variableLitShader;

        [Header("SFX Related")]
        public AudioMixerGroup soundsMasterMixer;

        // ------------------------
        // MATERIAL RELATED METHODS
        public void SetMaterialOpacity(ref Material material, float opacity)
        {
            material.SetFloat(materialOpacityString, opacity);
        }
    }
}
