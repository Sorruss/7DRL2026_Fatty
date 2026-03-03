using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;

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

        [Header("Special Tiles")]
        public TileBase[] collisionTiles;
        public TileBase preferrableEnemyPathTile;

        [Header("Resources - Room Graph Editor")]
        public RoomNodeTypeList roomNodeTypes;

        [Header("Material Variables")]
        public string materialOpacityString = "Alpha_Slider";

        [Header("Materials")]
        public Material dimmedMaterial;
        public Material defaultLitMaterial;

        [Header("Shaders")]
        public Shader variableLitShader;

        // ------------------------
        // MATERIAL RELATED METHODS
        public void SetMaterialOpacity(ref Material material, float opacity)
        {
            material.SetFloat(materialOpacityString, opacity);
        }
    }
}
