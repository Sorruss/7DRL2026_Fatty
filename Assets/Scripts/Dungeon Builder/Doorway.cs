using UnityEngine;

namespace FG
{
    [System.Serializable]
    public class Doorway
    {
        [Header("Config")]
        public Vector2Int position;
        public Orientation orientation;
        public GameObject doorPrefab;

        [Header("Copying Config")]
        public Vector2Int doorwayStartCopyPosition;
        public int doorwayCopyTileWidth;
        public int doorwayCopyTileHeight;

        [Header("Flags")]
        public bool isConnected = false;
        public bool isUnavailable = false;
    }
}
