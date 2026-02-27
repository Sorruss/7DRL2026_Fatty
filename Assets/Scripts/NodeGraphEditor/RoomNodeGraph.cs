using UnityEngine;
using System.Collections.Generic;

namespace FG
{
    [CreateAssetMenu(menuName = "Scriptable Object/Dungeon/Room Node Graph")]
    public class RoomNodeGraph : ScriptableObject
    {
        [Header("Nodes Info")]
        public List<RoomNode> nodesList = new();
        public Dictionary<string, RoomNode> nodesDict = new();
        [HideInInspector] public RoomNodeTypeList nodeTypes;

        // ------------
        // UNITY EVENTS
        private void OnValidate()
        {
            RefreshNodesDict();
        }

        // -------------------------
        // NODE LIST & DICT MANAGERS
        public void AddNode(RoomNode node)
        {
            // ADD TO THE LIST
            if (!nodesList.Contains(node))
                nodesList.Add(node);

            // ADD TO THE DICT
            if (!nodesDict.ContainsKey(node.roomID))
                nodesDict.Add(node.roomID, node);
        }

        public void RemoveNode(RoomNode node)
        {
            // REMOVE FROM THE LIST
            if (nodesList.Contains(node))
                nodesList.Remove(node);

            // REMOVE FROM THE DICT
            if (nodesDict.ContainsKey(node.roomID))
                nodesDict.Remove(node.roomID);
        }

        public RoomNode GetNodeByID(string roomID)
        {
            if (nodesDict.ContainsKey(roomID))
                return nodesDict[roomID];
            return null;
        }

        private void RefreshNodesDict()
        {
            foreach (RoomNode node in nodesList)
                AddNode(node);
        }

        public bool HasBossRoomConnected()
        {
            foreach (RoomNode node in nodesList)
            {
                if (!node.roomType.isBossRoom)
                    continue;

                if (node.roomNodeParentIDs.Count > 0)
                    return true;
            }

            return false;
        }

        public int GetCorridorsConnectedCount(RoomNode roomNode)
        {
            int count = 0;
            foreach (string childID in roomNode.roomNodeChildrenIDs)
            {
                RoomNode childNode = GetNodeByID(childID);
                if (childNode.roomType.isCorridor)
                    count++;
            }

            return count;
        }

#if UNITY_EDITOR
        [Header("Connection Line Debug")]
        public RoomNode connectLineStartingNode;
        public Vector2 connectLinePosition;
        public bool isTryingToConnectLine = false;

        // -----------------------
        // CONNECTING LINE METHODS
        public void ConfigureConnectLineStart(RoomNode startingNode, Vector2 linePosition)
        {
            connectLineStartingNode = startingNode;
            connectLinePosition = linePosition;
            isTryingToConnectLine = true;
        }

        public void ClearConnectLine()
        {
            connectLineStartingNode = null;
            connectLinePosition = Vector2.zero;
            isTryingToConnectLine = false;

            GUI.changed = true;
        }

        public void UnselectAllNodes()
        {
            foreach (var node in nodesList)
            {
                node.isSelected = false;
                GUI.changed = true;
            }
        }
#endif
    }
}
