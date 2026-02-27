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
    }
}
