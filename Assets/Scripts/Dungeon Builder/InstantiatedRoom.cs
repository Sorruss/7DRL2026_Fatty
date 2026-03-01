using UnityEngine;
using UnityEngine.Tilemaps;

namespace FG
{
    public class InstantiatedRoom : MonoBehaviour
    {
        private BoxCollider2D roomCollider;

        [Header("Room Config")]
        [HideInInspector] public Room room;
        [HideInInspector] public Grid roomGrid;
        [HideInInspector] public Bounds roomColliderBounds;

        [Header("Tilemaps")]
        [HideInInspector] public Tilemap groundTilemap;
        [HideInInspector] public Tilemap decoration01Tilemap;
        [HideInInspector] public Tilemap decoration02Tilemap;
        [HideInInspector] public Tilemap frontTilemap;
        [HideInInspector] public Tilemap collisionTilemap;
        [HideInInspector] public Tilemap minimapTilemap;

        // ------------
        // UNITY EVENTS
        private void Awake()
        {
            // ROOM CONFIG
            roomGrid = GetComponentInChildren<Grid>();
            roomCollider = GetComponent<BoxCollider2D>();
            roomColliderBounds = roomCollider.bounds;

            // TILEMAPS
            Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
            foreach (Tilemap tilemap in tilemaps)
            {
                switch (tilemap.tag)
                {
                    case "groundTilemap": groundTilemap = tilemap; break;
                    case "decoration1Tilemap": decoration01Tilemap = tilemap; break;
                    case "decoration2Tilemap": decoration02Tilemap = tilemap; break;
                    case "frontTilemap": frontTilemap = tilemap; break;
                    case "collisionTilemap": collisionTilemap = tilemap; break;
                    case "minimapTilemap": minimapTilemap = tilemap; break;
                    default: break;
                }
            }

            collisionTilemap.GetComponent<TilemapRenderer>().enabled = false;
        }

        // --------------
        // INITIALIZATION
        public void Init(Room room)
        {
            this.room = room;

            BlockUnusedDoorways();
        }

        private void BlockUnusedDoorways()
        {
            foreach (var doorway in room.doorways)
            {
                if (doorway.isConnected)
                    continue;

                BlockDoorwayWithTilemap(groundTilemap, doorway);
                BlockDoorwayWithTilemap(decoration01Tilemap, doorway);
                BlockDoorwayWithTilemap(decoration02Tilemap, doorway);
                BlockDoorwayWithTilemap(frontTilemap, doorway);
                BlockDoorwayWithTilemap(collisionTilemap, doorway);
                BlockDoorwayWithTilemap(minimapTilemap, doorway);
            }
        }

        // -------------
        // TILE BLOCKERS
        private void BlockDoorwayWithTilemap(Tilemap tilemap, Doorway doorway)
        {
            switch (doorway.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    BlockDoorwayHorizontally(tilemap, doorway);
                    break;
                case Orientation.west:
                case Orientation.east:
                    BlockDoorwayVertically(tilemap, doorway);
                    break;
                case Orientation.none: break;
                default: break;
            }
        }

        private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
        {
            Vector2Int startPosition = doorway.doorwayStartCopyPosition;
            for (int x = 0; x < doorway.doorwayCopyTileWidth; ++x)
            {
                for (int y = 0; y < doorway.doorwayCopyTileHeight; ++y)
                {
                    Vector3Int oldPos = new(startPosition.x + x, startPosition.y - y, 0);
                    Vector3Int newPos = new(startPosition.x + x + 1, startPosition.y - y, 0);
                    Matrix4x4 transformationMat = tilemap.GetTransformMatrix(oldPos);

                    tilemap.SetTile(newPos, tilemap.GetTile(oldPos));
                    tilemap.SetTransformMatrix(newPos, transformationMat);
                }
            }
        }

        private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
        {
            Vector2Int startPosition = doorway.doorwayStartCopyPosition;
            for (int y = 0; y < doorway.doorwayCopyTileHeight; ++y)
            {
                for (int x = 0; x < doorway.doorwayCopyTileWidth; ++x)
                {
                    Vector3Int oldPos = new(startPosition.x + x, startPosition.y - y, 0);
                    Vector3Int newPos = new(startPosition.x + x, startPosition.y - y - 1, 0);
                    Matrix4x4 transformationMat = tilemap.GetTransformMatrix(oldPos);

                    tilemap.SetTile(newPos, tilemap.GetTile(oldPos));
                    tilemap.SetTransformMatrix(newPos, transformationMat);
                }
            }
        }
    }
}
