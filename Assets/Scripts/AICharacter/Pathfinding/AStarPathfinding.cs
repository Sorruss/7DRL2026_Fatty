using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace FG
{
    public static class AStarPathfinding
    {
        public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
        {
            // Adjust positions by lower bounds
            startGridPosition -= (Vector3Int)room.templateLowerBounds; ;
            endGridPosition -= (Vector3Int)room.templateLowerBounds;

            // Create open list and closed hashset
            List<AStarNode> openAStarNodeList = new List<AStarNode>();
            HashSet<AStarNode> closedAStarNodeHashSet = new HashSet<AStarNode>();

            // Create AStarNodeGrid for path finding
            AStarNodeGrid AStarNodeGrid = new AStarNodeGrid(room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

            AStarNode startAStarNode = AStarNodeGrid.GetNode(startGridPosition.x, startGridPosition.y);
            AStarNode targetAStarNode = AStarNodeGrid.GetNode(endGridPosition.x, endGridPosition.y);

            AStarNode endPathAStarNode = FindShortestPath(startAStarNode, targetAStarNode, AStarNodeGrid, openAStarNodeList, closedAStarNodeHashSet, room.instantiatedRoom);

            if (endPathAStarNode != null)
            {
                return CreatePathStack(endPathAStarNode, room);
            }

            return null;
        }

        /// <summary>
        /// Find the shortest path - returns the end AStarNode if a path has been found, else returns null.
        /// </summary>
        private static AStarNode FindShortestPath(AStarNode startAStarNode, AStarNode targetAStarNode, AStarNodeGrid AStarNodeGrid, List<AStarNode> openAStarNodeList, HashSet<AStarNode> closedAStarNodeHashSet, InstantiatedRoom instantiatedRoom)
        {
            // Add start AStarNode to open list
            openAStarNodeList.Add(startAStarNode);

            // Loop through open AStarNode list until empty
            while (openAStarNodeList.Count > 0)
            {
                // Sort List
                openAStarNodeList.Sort();

                // current AStarNode = the AStarNode in the open list with the lowest fCost
                AStarNode currentAStarNode = openAStarNodeList[0];
                openAStarNodeList.RemoveAt(0);

                // if the current AStarNode = target AStarNode then finish
                if (currentAStarNode == targetAStarNode)
                {
                    return currentAStarNode;
                }

                // add current AStarNode to the closed list
                closedAStarNodeHashSet.Add(currentAStarNode);

                // evaluate fcost for each neighbour of the current AStarNode
                EvaluateCurrentAStarNodeNeighbours(currentAStarNode, targetAStarNode, AStarNodeGrid, openAStarNodeList, closedAStarNodeHashSet, instantiatedRoom);
            }

            return null;

        }

        /// <summary>
        ///  Create a Stack<Vector3> containing the movement path 
        /// </summary>
        private static Stack<Vector3> CreatePathStack(AStarNode targetAStarNode, Room room)
        {
            Stack<Vector3> movementPathStack = new Stack<Vector3>();

            AStarNode nextAStarNode = targetAStarNode;

            // Get mid point of cell
            Vector3 cellMidPoint = room.instantiatedRoom.roomGrid.cellSize * 0.5f;
            cellMidPoint.z = 0f;

            while (nextAStarNode != null)
            {
                // Convert grid position to world position
                Vector3 worldPosition = room.instantiatedRoom.roomGrid.CellToWorld(new Vector3Int(nextAStarNode.gridPosition.x + room.templateLowerBounds.x, nextAStarNode.gridPosition.y + room.templateLowerBounds.y, 0));

                // Set the world position to the middle of the grid cell
                worldPosition += cellMidPoint;

                movementPathStack.Push(worldPosition);

                nextAStarNode = nextAStarNode.parent;
            }

            return movementPathStack;
        }

        /// <summary>
        /// Evaluate neighbour AStarNodes
        /// </summary>
        private static void EvaluateCurrentAStarNodeNeighbours(AStarNode currentAStarNode, AStarNode targetAStarNode, AStarNodeGrid AStarNodeGrid, List<AStarNode> openAStarNodeList, HashSet<AStarNode> closedAStarNodeHashSet, InstantiatedRoom instantiatedRoom)
        {
            Vector2Int currentAStarNodeGridPosition = currentAStarNode.gridPosition;

            AStarNode validNeighbourAStarNode;

            // Loop through all directions
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    validNeighbourAStarNode = GetValidAStarNodeNeighbour(currentAStarNodeGridPosition.x + i, currentAStarNodeGridPosition.y + j, AStarNodeGrid, closedAStarNodeHashSet, instantiatedRoom);

                    if (validNeighbourAStarNode != null)
                    {
                        // Calculate new gcost for neighbour
                        int newCostToNeighbour;

                        // Get the movement penalty
                        // Unwalkable paths have a value of 0. Default movement penalty is set in
                        // Settings and applies to other grid squares.
                        int movementPenaltyForGridSpace = instantiatedRoom.movementPenalty[validNeighbourAStarNode.gridPosition.x, validNeighbourAStarNode.gridPosition.y];

                        newCostToNeighbour = currentAStarNode.GValue + GetDistance(currentAStarNode, validNeighbourAStarNode) + movementPenaltyForGridSpace;

                        bool isValidNeighbourAStarNodeInOpenList = openAStarNodeList.Contains(validNeighbourAStarNode);

                        if (newCostToNeighbour < validNeighbourAStarNode.GValue || !isValidNeighbourAStarNodeInOpenList)
                        {
                            validNeighbourAStarNode.GValue = newCostToNeighbour;
                            validNeighbourAStarNode.HValue = GetDistance(validNeighbourAStarNode, targetAStarNode);
                            validNeighbourAStarNode.parent = currentAStarNode;

                            if (!isValidNeighbourAStarNodeInOpenList)
                            {
                                openAStarNodeList.Add(validNeighbourAStarNode);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the distance int between AStarNodeA and AStarNodeB
        /// </summary>
        private static int GetDistance(AStarNode AStarNodeA, AStarNode AStarNodeB)
        {
            int dstX = Mathf.Abs(AStarNodeA.gridPosition.x - AStarNodeB.gridPosition.x);
            int dstY = Mathf.Abs(AStarNodeA.gridPosition.y - AStarNodeB.gridPosition.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);  // 10 used instead of 1, and 14 is a pythagoras approximation SQRT(10*10 + 10*10) - to avoid using floats
            return 14 * dstX + 10 * (dstY - dstX);
        }

        /// <summary>
        /// Evaluate a neighbour AStarNode at neighboutAStarNodeXPosition, neighbourAStarNodeYPosition, using the
        /// specified AStarNodeGrid, closedAStarNodeHashSet, and instantiated room.  Returns null if the AStarNode isn't valid
        /// </summary>
        private static AStarNode GetValidAStarNodeNeighbour(int neighbourAStarNodeXPosition, int neighbourAStarNodeYPosition, AStarNodeGrid AStarNodeGrid, HashSet<AStarNode> closedAStarNodeHashSet, InstantiatedRoom instantiatedRoom)
        {
            // If neighbour AStarNode position is beyond grid then return null
            if (neighbourAStarNodeXPosition >= instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x || neighbourAStarNodeXPosition < 0 || neighbourAStarNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y || neighbourAStarNodeYPosition < 0)
            {
                return null;
            }

            // Get neighbour AStarNode
            AStarNode neighbourAStarNode = AStarNodeGrid.GetNode(neighbourAStarNodeXPosition, neighbourAStarNodeYPosition);

            // check for obstacle at that position
            int movementPenaltyForGridSpace = instantiatedRoom.movementPenalty[neighbourAStarNodeXPosition, neighbourAStarNodeYPosition];

            // if neighbour is an obstacle or neighbour is in the closed list then skip
            if (movementPenaltyForGridSpace == 0 || closedAStarNodeHashSet.Contains(neighbourAStarNode))
            {
                return null;
            }
            else
            {
                return neighbourAStarNode;
            }

        }
    }
}
