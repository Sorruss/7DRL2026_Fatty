using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FG
{
    [CreateAssetMenu(menuName = "Scriptable Object/Dungeon Builder/Room")]
    public class RoomTemplate : ScriptableObject
    {
        public string guid;

        [Header("Config")]
        public GameObject prefab;
        public RoomNodeType roomNodeType;
        public Vector2Int lowerBounds;
        public Vector2Int upperBounds;
        [SerializeField] public List<Doorway> doorwayList;
        public Vector2Int[] spawnPositionArray;

        [Header("Debug")]
        public GameObject previousPrefab;

        public List<Doorway> GetDoorwayList()
        {
            return doorwayList;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (guid == "" || previousPrefab != prefab)
            {
                guid = GUID.Generate().ToString();
                previousPrefab = prefab;
                EditorUtility.SetDirty(this);
            }

            Helpers.ValidateEnumerableProperty(this, nameof(doorwayList), doorwayList);
            Helpers.ValidateEnumerableProperty(this, nameof(spawnPositionArray), spawnPositionArray);
        }
#endif
    }
}