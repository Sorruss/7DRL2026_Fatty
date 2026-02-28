using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FG
{
    [CreateAssetMenu(menuName = "Scriptable Object/Dungeon Builder/Room")]
    public class RoomTemplate : ScriptableObject
    {
        [HideInInspector] public string guid;

        [Header("Config")]
        public GameObject roomPrefab;
        public RoomNodeType roomType;
        public Vector2Int lowerBounds;
        public Vector2Int upperBounds;
        public Vector2Int[] spawnPoints;
        public List<Doorway> doorways;

        [Header("Debug")]
        public GameObject roomPreviousPrefab;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(guid) || roomPrefab != roomPreviousPrefab)
            {
                guid = Guid.NewGuid().ToString();
                roomPreviousPrefab = roomPrefab;
                EditorUtility.SetDirty(this);
            }

            Helpers.ValidateEnumerableProperty(this, nameof(doorways), doorways);
            Helpers.ValidateEnumerableProperty(this, nameof(spawnPoints), spawnPoints);
        }
#endif
    }
}
