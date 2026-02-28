using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace FG
{
    [CreateAssetMenu(menuName = "Scriptable Object/Dungeon Editor/Room Node Graph")]
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

        private void RefreshNodesDict()
        {
            foreach (RoomNode node in nodesList)
                AddNode(node);
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

        // GETTERS
        public RoomNode GetNodeByID(string nodeID)
        {
            if (nodesDict.ContainsKey(nodeID))
                return nodesDict[nodeID];
            return null;
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

        public RoomNode GetNodeByType(RoomNodeType roomNodeType)
        {
            foreach (var node in nodesList)
            {
                if (node.roomType != roomNodeType)
                    continue;

                return node;
            }

            return null;
        }

        public IEnumerable<RoomNode> GetNextChildByNodeID(RoomNode parentNode)
        {
            foreach (string childID in parentNode.roomNodeChildrenIDs)
                yield return GetNodeByID(childID);
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
