using System.Collections;
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

        [Header("Doors Config")]
        [HideInInspector] public float pixelsPerUnit = 16.0f;
        [HideInInspector] public float tilesSize = 16.0f;

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

        private void OnEnable()
        {
            GameManager.instance.RoomChangeEvent += OnCurrentRoomChanged;
        }

        private void OnDisable()
        {
            GameManager.instance.RoomChangeEvent -= OnCurrentRoomChanged;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (room == GameManager.instance.currentRoom)
                return;

            PlayerManager player = collision.GetComponent<PlayerManager>();
            if (player == null)
                return;

            room.wasVisitedBefore = true;
            GameManager.instance.SetCurrentRoom(room);
        }

        // --------------
        // INITIALIZATION
        public void Init(Room room)
        {
            this.room = room;

            BlockUnusedDoorways();
            PlaceDoors();
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

        private void PlaceDoors()
        {
            if (room.roomType.isCorridorHorizontal || room.roomType.isCorridorVertical)
                return;

            foreach (var doorway in room.doorways)
            {
                if (!doorway.isConnected)
                    continue;

                if (doorway.doorPrefab == null)
                    continue;

                float tileDistance = tilesSize / pixelsPerUnit;
                GameObject door = Instantiate(doorway.doorPrefab, transform);

                if (doorway.orientation == Orientation.north)
                {
                    door.transform.localPosition = 
                        new Vector3(doorway.position.x + tileDistance / 2.0f,
                        doorway.position.y + tileDistance, 0.0f);
                }
                else if (doorway.orientation == Orientation.south)
                {
                    door.transform.localPosition =
                        new Vector3(doorway.position.x + tileDistance / 2.0f,
                        doorway.position.y, 0.0f);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    door.transform.localPosition =
                        new Vector3(doorway.position.x,
                        doorway.position.y + tileDistance * 1.25f, 0.0f);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    door.transform.localPosition =
                        new Vector3(doorway.position.x + tileDistance,
                        doorway.position.y + tileDistance * 1.25f, 0.0f);
                }

                Door doorComponent = door.GetComponent<Door>();
                doorComponent.isBossRoom = room.roomType.isBossRoom;
                if (doorComponent.isBossRoom)
                    doorComponent.LockDoor(true);
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

        // -----------------
        // LIGHTNING CONTROL
        private void LitRoom()
        {
            Material variableMaterial = new(ResourcesManager.instance.variableLitShader);
            StartCoroutine(LitRoomCoroutine(variableMaterial));
        }

        private void LitAllDoors()
        {
            Door[] doors = GetComponentsInChildren<Door>();
            foreach (Door door in doors)
                door.LitDoor();
        }

        // ----------
        // COROUTINES
        private IEnumerator LitRoomCoroutine(Material material)
        {
            groundTilemap.GetComponent<TilemapRenderer>().material = material;
            decoration01Tilemap.GetComponent<TilemapRenderer>().material = material;
            decoration02Tilemap.GetComponent<TilemapRenderer>().material = material;
            frontTilemap.GetComponent<TilemapRenderer>().material = material;
            minimapTilemap.GetComponent<TilemapRenderer>().material = material;

            for (float i = 0.05f; i < 1.0f; i += Time.deltaTime / GameManager.instance.roomFadeInTime)
            {
                material.SetFloat(ResourcesManager.instance.materialOpacityString, i);
                yield return null;
            }

            groundTilemap.GetComponent<TilemapRenderer>().material = ResourcesManager.instance.defaultLitMaterial;
            decoration01Tilemap.GetComponent<TilemapRenderer>().material = ResourcesManager.instance.defaultLitMaterial;
            decoration02Tilemap.GetComponent<TilemapRenderer>().material = ResourcesManager.instance.defaultLitMaterial;
            frontTilemap.GetComponent<TilemapRenderer>().material = ResourcesManager.instance.defaultLitMaterial;
            minimapTilemap.GetComponent<TilemapRenderer>().material = ResourcesManager.instance.defaultLitMaterial;

            yield return null;
        }

        // ---------
        // CALLBACKS
        private void OnCurrentRoomChanged(Room newValue)
        {
            if (room != newValue)
                return;

            if (room.isLit)
                return;

            LitRoom();
            LitAllDoors();

            room.isLit = true;
        }
    }
}
