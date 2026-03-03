using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FG
{
    public class AStarTest : SingletonMonoBehaviour<AStarTest>
    {
        private InstantiatedRoom instantiatedRoom;
        private Grid grid;
        private Tilemap frontTilemap;
        private Tilemap pathTilemap;
        private Vector3Int startGridPosition;
        private Vector3Int endGridPosition;
        private TileBase startPathTile;
        private TileBase finishPathTile;

        private Vector3Int noValue = new Vector3Int(9999, 9999, 9999);
        private Stack<Vector3> pathStack;

        private void OnEnable()
        {
            StartCoroutine(WaitAndSubscribe());
        }

        private void OnDisable()
        {
            GameManager.instance.RoomChangeEvent -= StaticEventHandler_OnRoomChanged;
        }

        private void Start()
        {
            startPathTile = ResourcesManager.instance.preferrableEnemyPathTile;
            finishPathTile = ResourcesManager.instance.collisionTiles[0];
        }

        private void StaticEventHandler_OnRoomChanged(Room room)
        {
            pathStack = null;
            instantiatedRoom = room.instantiatedRoom;
            frontTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front").GetComponent<Tilemap>();
            grid = instantiatedRoom.transform.GetComponentInChildren<Grid>();
            startGridPosition = noValue;
            endGridPosition = noValue;

            SetUpPathTilemap();
        }

        /// <summary>
        /// Use a clone of the front tilemap for the path tilemap.  If not created then create one, else use the exisitng one.
        /// </summary>
        private void SetUpPathTilemap()
        {
            Transform tilemapCloneTransform = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)");

            // If the front tilemap hasn't been cloned then clone it
            if (tilemapCloneTransform == null)
            {
                pathTilemap = Instantiate(frontTilemap, grid.transform);
                pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
                pathTilemap.GetComponent<TilemapRenderer>().material = ResourcesManager.instance.defaultLitMaterial;
                pathTilemap.gameObject.tag = "Untagged";
            }
            // else use it
            else
            {
                pathTilemap = instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)").GetComponent<Tilemap>();
                pathTilemap.ClearAllTiles();
            }
        }

        /// <summary>
        /// Set the start position and the start tile on the front tilemap
        /// </summary>
        public void SetStartPosition()
        {
            ClearPath();

            if (startGridPosition == noValue)
            {
                startGridPosition = grid.WorldToCell(PlayerInputManager.instance.MousePosition);

                if (!IsPositionWithinBounds(startGridPosition))
                {
                    startGridPosition = noValue;
                    return;
                }

                pathTilemap.SetTile(startGridPosition, startPathTile);
            }
            else
            {
                pathTilemap.SetTile(startGridPosition, null);
                startGridPosition = noValue;
            }
        }

        /// <summary>
        /// Set the end position and the end tile on the front tilemap
        /// </summary>
        public void SetEndPosition()
        {
            ClearPath();

            if (endGridPosition == noValue)
            {
                endGridPosition = grid.WorldToCell(PlayerInputManager.instance.MousePosition);

                if (!IsPositionWithinBounds(endGridPosition))
                {
                    endGridPosition = noValue;
                    return;
                }

                pathTilemap.SetTile(endGridPosition, finishPathTile);
            }
            else
            {
                pathTilemap.SetTile(endGridPosition, null);
                endGridPosition = noValue;
            }
        }

        /// <summary>
        /// Check if the position is within the lower and upper bounds of the room
        /// </summary>
        private bool IsPositionWithinBounds(Vector3Int position)
        {
            // If  position is beyond grid then return false
            if (position.x < instantiatedRoom.room.templateLowerBounds.x || position.x > instantiatedRoom.room.templateUpperBounds.x
                || position.y < instantiatedRoom.room.templateLowerBounds.y || position.y > instantiatedRoom.room.templateUpperBounds.y)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Clear the path and reset the start and finish positions
        /// </summary>
        private void ClearPath()
        {
            // Clear Path
            if (pathStack == null) return;

            foreach (Vector3 worldPosition in pathStack)
            {
                pathTilemap.SetTile(grid.WorldToCell(worldPosition), null);
            }

            pathStack = null;

            //Clear Start and Finish Squares
            endGridPosition = noValue;
            startGridPosition = noValue;
        }

        /// <summary>
        /// Build and display the AStar path between the start and finish positions
        /// </summary>
        public void DisplayPath()
        {
            if (startGridPosition == noValue || endGridPosition == noValue) return;

            pathStack = AStarPathfinding.BuildPath(instantiatedRoom.room, startGridPosition, endGridPosition);

            if (pathStack == null) return;

            foreach (Vector3 worldPosition in pathStack)
            {
                pathTilemap.SetTile(grid.WorldToCell(worldPosition), startPathTile);
            }
        }

        private IEnumerator WaitAndSubscribe()
        {
            while (GameManager.instance == null)
                yield return null;

            GameManager.instance.RoomChangeEvent += StaticEventHandler_OnRoomChanged;
            yield return null;
        }
    }
}
