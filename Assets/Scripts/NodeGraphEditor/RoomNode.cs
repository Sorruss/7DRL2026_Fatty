using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FG
{
    // THIS ONE IS NOT CREATED OUTSIDE OF THE ALGORITHM
    public class RoomNode : ScriptableObject
    {
        [Header("Post Init Values")]
        private RoomNodeTypeList roomNodeTypes;

        [Header("Config")]
        public string roomID;
        public RoomNodeType roomType;
        public RoomNodeGraph roomNodeGraph;

        [Header("Relations")]
        public List<string> roomNodeParentIDs = new();
        public List<string> roomNodeChildrenIDs = new();

#if UNITY_EDITOR
        [Header("Post Init Values")]
        public Rect nodeRect;

        [Header("Action Flags")]
        public bool isSelected = false;
        public bool isBeingDragged = false;

        // ----------------------
        // INITIALIZATION METHODS
        public void Initialize(Rect nodeRect, RoomNodeGraph nodeGraph, RoomNodeType nodeType)
        {
            this.nodeRect = nodeRect;
            name = "Room Node";
            roomNodeTypes = ResourcesManager.instance.roomNodeTypes;

            roomID = Guid.NewGuid().ToString();
            roomType = nodeType;
            roomNodeGraph = nodeGraph;
        }

        // ---------------------
        // SUPPLEMENTARY METHODS
        private string[] GetRoomNodeTypeNames()
        {
            string[] nodeTypeNames = new string[roomNodeTypes.roomNodeTypes.Count];
            for (int i = 0; i < nodeTypeNames.Length; ++i)
            {
                if (!roomNodeTypes.roomNodeTypes[i].isCreatableByUser)
                    continue;

                nodeTypeNames[i] = roomNodeTypes.roomNodeTypes[i].roomName;
            }

            return nodeTypeNames;
        }

        private void ShowContextMenu(Vector2 mousePosition)
        {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Create Connection"), false, OnCreateConnection, mousePosition);
            contextMenu.ShowAsContext();
        }

        private void OnCreateConnection(object mousePosition)
        {
            roomNodeGraph.ConfigureConnectLineStart(this, (Vector2)mousePosition);
        }

        // ------------
        // TICK METHODS
        public void Draw(GUIStyle style)
        {
            // START
            GUILayout.BeginArea(nodeRect, style);
            EditorGUI.BeginChangeCheck();
            
            // CONTENT
            if (roomNodeParentIDs.Count == 0 && !roomType.isEntrance)
            {
                int currentTypeIndex = roomNodeTypes.roomNodeTypes.FindIndex(x => x == roomType);
                int newTypeIndex = EditorGUILayout.Popup("", currentTypeIndex, GetRoomNodeTypeNames());
                roomType = roomNodeTypes.roomNodeTypes[newTypeIndex];
            }
            else
            {
                EditorGUILayout.LabelField(roomType.roomName);
            }

            // STOP
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(this);
            GUILayout.EndArea();
        }
        
        public void ProcessEvents(Event currentEvent)
        {
            switch (currentEvent.type)
            {
                case EventType.MouseDown: ProcessMouseDownEvent(currentEvent); break;
                case EventType.MouseUp: ProcessMouseUpEvent(currentEvent); break;
                case EventType.MouseDrag: ProcessMouseDragEvent(currentEvent); break;
                default: break;
            }
        }

        // ------------------------
        // EVENT PROCESSING - MOUSE
        private void ProcessMouseDownEvent(Event currentEvent)
        {
            // 0 = LEFT MOUSE BUTTON, 1 = RIGHT MOUSE BUTTON
            if (currentEvent.button == 0)
                ProcessLeftMouseDownEvent(currentEvent);
            if (currentEvent.button == 1)
                ProcessRightMouseDownEvent(currentEvent);
        }

        private void ProcessMouseUpEvent(Event currentEvent)
        {
            // 0 = LEFT MOUSE BUTTON, 1 = RIGHT MOUSE BUTTON
            if (currentEvent.button == 1)
                return;

            if (isBeingDragged)
            {
                isBeingDragged = false;
                isSelected = false;
            }
        }

        private void ProcessMouseDragEvent(Event currentEvent)
        {
            if (!isSelected)
                return;

            isBeingDragged = true;
            nodeRect.position += currentEvent.delta;
            EditorUtility.SetDirty(this);
            GUI.changed = true;
        }

        // -----------------------------
        // EVENT PROCESSING - LEFT MOUSE
        private void ProcessLeftMouseDownEvent(Event currentEvent)
        {
            Selection.activeObject = this;
            isSelected = !isSelected;
        }

        // ------------------------------
        // EVENT PROCESSING - RIGHT MOUSE
        private void ProcessRightMouseDownEvent(Event currentEvent)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
#endif
        // -----------------
        // RELATION MANAGERS
        public void AddParentID(string ID)
        {
            roomNodeParentIDs.Add(ID);
        }

        public bool AddChildID(string ID)
        {
            // THAT'S ME
            if (ID == roomID)
                return false;

            // THAT'S MY PARENT
            if (roomNodeParentIDs.Contains(ID))
                return false;

            // ALREADY HAVE THIS CHILD
            if (roomNodeChildrenIDs.Contains(ID))
                return false;

            RoomNode childNode = roomNodeGraph.GetNodeByID(ID);

            // THIS IS A CORRIDOR
            if (roomType.isCorridor && childNode.roomType.isCorridor)
                return false;

            // THIS IS A NULL ROOM
            if (roomType.isNone)
                return false;

            // CHILD IS A NULL ROOM
            if (childNode.roomType.isNone)
                return false;

            // CAN'T ADD NON-CORRIDOR TO NON-CORRIDOR
            if (!roomType.isCorridor && !childNode.roomType.isCorridor)
                return false;

            // CHILD IS A ENTRANCE
            if (childNode.roomType.isEntrance)
                return false;

            if (childNode.roomType.isBossRoom && roomNodeGraph.HasBossRoomConnected())
                return false;

            // TOO MANY CORRIDORS
            if (childNode.roomType.isCorridor && roomNodeGraph.GetCorridorsConnectedCount(this) >= 3)
                return false;

            roomNodeChildrenIDs.Add(ID);
            return true;
        }
    }
}
