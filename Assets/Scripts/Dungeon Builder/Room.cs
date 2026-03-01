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
        public string parentID;
        public List<string> childrenID = new();

        [Header("Template")]
        public string templateID;
        public Vector2Int templateLowerBounds;
        public Vector2Int templateUpperBounds;
        public Vector2Int[] spawnsPoints;
        public List<Doorway> doorways = new();

        [Header("Flags")]
        public bool isLit = false;
        public bool isPositioned = false;
        public bool wasVisitedBefore = false;
        public bool isClearedOfEnemies = false;

        public Room(RoomNode roomNode, RoomTemplate template)
        {
            // COPYING
            ID = roomNode.roomID;
            roomType = roomNode.roomType;
            roomPrefab = template.prefab;
            lowerBounds = template.lowerBounds;
            upperBounds = template.upperBounds;

            if (!roomType.isEntrance)
                parentID = roomNode.roomNodeParentIDs[0];
            Helpers.CopyListTo(ref roomNode.roomNodeChildrenIDs, ref childrenID);

            templateID = template.ID;
            templateLowerBounds = template.lowerBounds;
            templateUpperBounds = template.upperBounds;
            spawnsPoints = template.spawnPositionArray;
            Helpers.CopyListTo(ref template.doorwayList, ref doorways);

            // FLAGS
            if (roomType.isEntrance)
                wasVisitedBefore = true;
        }
    }
}
