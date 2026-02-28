using UnityEngine;

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
        [Header("Resources - Room Graph Editor")]
        public RoomNodeTypeList roomNodeTypes;

        [Header("Material Variables")]
        [SerializeField] private string materialOpacityString = "Alpha_Slider";

        [Header("Materials")]
        public Material dimmedMaterial;

        // ------------------------
        // MATERIAL RELATED METHODS
        public void SetMaterialOpacity(ref Material material, float opacity)
        {
            material.SetFloat(materialOpacityString, opacity);
        }
    }
}
