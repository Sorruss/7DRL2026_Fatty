using System.Collections.Generic;
using UnityEngine;

namespace FG
{
    public class Room
    {
        [Header("Config")]
        public string ID;
        public GameObject roomPrefab;
        public RoomNodeType roomType;
        public Vector2Int lowerBounds;
        public Vector2Int upperBounds;
        public InstantiatedRoom instantiatedRoom;

        [Header("Relations")]
        public List<string> parentsID = new();
        public List<string> childrenID = new();

        [Header("Template")]
        public string templateID;
        public Vector2Int templateLowerBounds;
        public Vector2Int templateUpperBounds;
        public Vector2Int spawnsPoints;
        public List<Doorway> doorways;

        [Header("Flags")]
        public bool isLit = false;
        public bool isPositioned = false;
        public bool wasVisitedBefore = false;
        public bool isClearedOfEnemies = false;
    }
}
