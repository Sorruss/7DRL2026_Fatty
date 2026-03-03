using System.Collections.Generic;
using UnityEngine;

namespace FG
{
    public static class AStarPathfinding
    {
        // MAIN METHOD
        public static Stack<Vector3> BuildPath(Room room, Vector3Int gridStart, Vector3Int gridEnd)
        {
            // ADJUST POSITIONS
            gridStart -= (Vector3Int)room.templateLowerBounds;
            gridEnd -= (Vector3Int)room.templateLowerBounds;

            // OPEN & CLOSED LISTS
            List<AStarNode> openList = new();
            HashSet<AStarNode> closedList = new();

            // GRID NODES
            int roomX = room.templateUpperBounds.x - room.templateLowerBounds.x + 1;
            int roomY = room.templateUpperBounds.y - room.templateLowerBounds.y + 1;
            AStarNodeGrid grid = new(roomX, roomY);

            // START & DESTINATION NODES
            AStarNode startNode = grid.GetNode(gridStart.x, gridStart.y);
            AStarNode destinationNode = grid.GetNode(gridEnd.x, gridEnd.y);

            // RUN THE ALGORITHM
            AStarNode pathNode = FindBestPathNode(startNode, destinationNode, grid, openList, closedList, room.instantiatedRoom);
            if (pathNode == null)
                return null;

            return CreatePathStack(pathNode, room);
        }

        // FIND BEST PATH METHOD
        private static AStarNode FindBestPathNode(
            AStarNode start, 
            AStarNode end, 
            AStarNodeGrid grid, 
            List<AStarNode> openList, 
            HashSet<AStarNode> closedList, 
            InstantiatedRoom instantiatedRoom)
        {
            // 1. ADD START TO OPEN LIST
            openList.Add(start);

            // 2. WHILE WE HAVE NODES IN OPEN LIST
            while (openList.Count > 0)
            {
                // 3. GET NODE WITH LOWEST FVALUE
                openList.Sort();
                AStarNode node = openList[0];

                // 4. SEE IF ITS THE NEEDED NODE
                if (node == end)
                    return node;

                // 5. MOVE NODE FROM OPEN TO CLOSED LIST
                openList.RemoveAt(0);
                closedList.Add(node);

                // 6. ITERATE THROUGH EVERY NEIGHBOUR OR NODE
                AStarNode neighbour;
                Vector2Int nodePosition = node.gridPosition;
                for (int x = -1; x <= 1; ++x)
                {
                    for (int y = -1; y <= 1; ++y)
                    {
                        // 6.1 IT'S THIS NODE
                        if (x == 0 && y == 0)
                            continue;

                        neighbour = grid.GetNode(nodePosition.x + x, nodePosition.y + y);

                        // 6.2 NEIGHBOUR IS IN CLOSED LIST
                        if (closedList.Contains(neighbour))
                            continue;

                        // 6.3 NEIGHBOUR IS AN OBSTACLE OR OUT OF BOUNDS
                        // IS OUT OF BOUNDS
                        if (!CheckIfValidNeighbour(neighbour, instantiatedRoom))
                        {
                            closedList.Add(neighbour);
                            continue;
                        }

                        // 6.4 CALCULATE GVALUE FOR NEIGHBOUR
                        int movementPenalty = instantiatedRoom.movementPenalty[neighbour.gridPosition.x, neighbour.gridPosition.y];
                        int newGValue = node.GValue + GetDistance(neighbour, node);
                        newGValue += movementPenalty;

                        // 6.5 IF NEIGHBOUR NOT IN THE OPEN LIST
                        // OR IT IS BUT FVALUE IS LOWER
                        bool condition01 = openList.Contains(neighbour);
                        bool condition02 = condition01 && neighbour.FValue > newGValue;
                        if (!condition01 && !condition02)
                            continue;

                        // 6.6 SAVE VALUES FOR NEIGHBOUR
                        neighbour.GValue = newGValue;
                        neighbour.HValue = GetDistance(neighbour, end);

                        // 6.7 SET NEIGHBOUR'S PARENT & ADD IT TO OPENLIST
                        neighbour.parent = node;
                        if (condition01)
                            continue;

                        openList.Add(neighbour);
                    }
                }
            }

            return null;
        }

        // BUILD PATH BASED ON BEST PATH
        private static Stack<Vector3> CreatePathStack(AStarNode pathNode, Room room)
        {
            Stack<Vector3> movementStack = new();
            AStarNode nextNode = pathNode;

            Vector3 cellMiddle = room.instantiatedRoom.roomGrid.cellSize * 0.5f;
            cellMiddle.z = 0.0f;

            while (nextNode != null)
            {
                Vector3Int cellPos = new(
                    nextNode.gridPosition.x + room.templateLowerBounds.x,
                    nextNode.gridPosition.y + room.templateLowerBounds.y, 0);
                Vector3 cellWorld = room.instantiatedRoom.roomGrid.CellToWorld(cellPos);
                cellWorld += cellMiddle;

                movementStack.Push(cellWorld);
                nextNode = nextNode.parent;
            }
            
            return movementStack;
        }

        // -------
        // HELPERS
        private static bool CheckIfValidNeighbour(AStarNode node, InstantiatedRoom instantiatedRoom)
        {
            // IF OUT OF BOUNDS OF THE ROOM
            bool outOfBoundsX = node.gridPosition.x >= instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x;
            bool outOfBoundsY = node.gridPosition.y >= instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y;

            if (outOfBoundsX || outOfBoundsY || node.gridPosition.x < 0 || node.gridPosition.y < 0)
                return false;

            // IS AN OBSTACLE
            int movementPenalty = instantiatedRoom.movementPenalty[node.gridPosition.x, node.gridPosition.y];
            if (movementPenalty == 0)
                return false;

            return true;
        }

        private static int GetDistance(AStarNode node1, AStarNode node2)
        {
            int distanceX = Mathf.Abs(node1.gridPosition.x - node2.gridPosition.x);
            int distanceY = Mathf.Abs(node1.gridPosition.y - node2.gridPosition.y);

            if (distanceX > distanceY)
                return 14 * distanceY + 10 * (distanceX - distanceY);
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }
}
