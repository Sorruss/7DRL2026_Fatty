using UnityEngine;

namespace FG
{
    [System.Serializable]
    public class Doorway
    {
        [Header("Config")]
        public Orientation orientation;         // WHERE DOOR LEADS
        public Vector2Int position;             // DOOR'S POSITINO
        public GameObject doorwayPrefab;        // DOOR'S PREFAB

        [Header("Copying Config")]
        public Vector2Int startCopyPosition;    // UPPER-LEFT POSITION OF TILES WE WANT TO COPY
        public int copyTileWidth;               // WIDTH OF AREA OF TILES WE WANT TO COPY
        public int copyTileHeight;              // WIDTH OF AREA OF TILES WE WANT TO COPY

        [Header("Flags")]
        public bool isConnected = false;        // IS DOOR ALREADY CONNECTED TO ROOM
        public bool isAvailable = true;         // IS DOOR UNABLE TO CONNECT
    }
}
