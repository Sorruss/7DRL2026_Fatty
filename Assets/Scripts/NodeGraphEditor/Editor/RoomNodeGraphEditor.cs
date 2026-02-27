using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace FG
{
    public class RoomNodeGraphEditor : EditorWindow
    {
        private GUIStyle roomNodeStyle;
        private GUIStyle roomSelectedNodeStyle;

        private static RoomNodeGraph currentGraphOpen;
        private static RoomNode currentNodeSelected;

        [Header("Config")]
        private int nodeWidth = 160;
        private int nodeHeight = 75;
        private int nodeBorderOffset = 12;
        private int nodePaddingOffset = 25;
        private int connectLineWidth = 4;
        private float connectLineArrowSize = 7.0f;

        [MenuItem("Graph", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
        private static void OpenWindow()
        {
            GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
        }

        // ------------
        // UNITY EVENTS
        // SETTING UP STYLE FOR THE ROOM NODES
        private void OnEnable()
        {
            // ALLOWS FOR MouseMove EVENT
            wantsMouseMove = true;

            // SUBSCRIPTIONS
            Selection.selectionChanged += OnSelectionChanged;

            // NODE STYLE (DEFAULT)
            if (roomNodeStyle == null)
            {
                roomNodeStyle = new GUIStyle();
                roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
                roomNodeStyle.normal.textColor = Color.white;
                roomNodeStyle.border = new RectOffset(nodeBorderOffset, nodeBorderOffset, nodeBorderOffset, nodeBorderOffset);
                roomNodeStyle.padding = new RectOffset(nodePaddingOffset, nodePaddingOffset, nodePaddingOffset, nodePaddingOffset);
            }

            // NODE STYLE (SELECTED)
            if (roomSelectedNodeStyle == null)
            {
                roomSelectedNodeStyle = new GUIStyle();
                roomSelectedNodeStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
                roomSelectedNodeStyle.normal.textColor = Color.white;
                roomSelectedNodeStyle.border = new RectOffset(nodeBorderOffset, nodeBorderOffset, nodeBorderOffset, nodeBorderOffset);
                roomSelectedNodeStyle.padding = new RectOffset(nodePaddingOffset, nodePaddingOffset, nodePaddingOffset, nodePaddingOffset);
            }
        }

        private void OnDisable()
        {
            // SUBSCRIPTIONS
            Selection.selectionChanged -= OnSelectionChanged;
        }

        // WINDOW RENDERER
        private void OnGUI()
        {
            if (currentGraphOpen != null)
            {
                DrawConnectLine();
                ProcessEvents(Event.current);
                DrawDefinedConnectLines();
                DrawRoomNodes();
            }

            if (GUI.changed)
                Repaint();
        }

        // ----------------
        // EVENT PROCESSING
        private void ProcessEvents(Event currentEvent)
        {
            // GET HOVERED OVER NODE IF THERE IS SUCH
            if (currentNodeSelected == null || !currentNodeSelected.isBeingDragged)
                currentNodeSelected = IsMouseOverNode(currentEvent.mousePosition);

            // IF THERE IS SUCH & WE NOT TRYING TO DO CONNECT -> PROCESS ITS EVENTS
            // IF NOT -> PROCESS GRAPH EVENTS
            if (currentNodeSelected == null || currentGraphOpen.isTryingToConnectLine)
                ProcessGraphEvents(currentEvent);
            else
                currentNodeSelected.ProcessEvents(currentEvent);
        }

        private RoomNode IsMouseOverNode(Vector2 mousePos)
        {
            foreach (var node in currentGraphOpen.nodesList)
                if (node.nodeRect.Contains(mousePos))
                    return node;

            return null;
        }

        private void ProcessGraphEvents(Event currentEvent)
        {
            switch (currentEvent.type)
            {
                case EventType.MouseDown: ProcessMouseDownEvent(currentEvent); break;
                case EventType.MouseMove: ProcessMouseMoveEvent(currentEvent); break;
                default: break;
            }
        }

        // EVENT PROCESSING - MOUSE DOWN
        private void ProcessMouseDownEvent(Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                // LEFT MOUSE BUTTON
                if (currentGraphOpen.isTryingToConnectLine)
                {
                    RoomNode overNode = IsMouseOverNode(currentEvent.mousePosition);
                    if (overNode != null)
                    {
                        // FORM A CONNECTION
                        if (currentGraphOpen.connectLineStartingNode.AddChildID(overNode.roomID))
                            overNode.AddParentID(currentGraphOpen.connectLineStartingNode.roomID);
                    }

                    // REMOVE LINE
                    currentGraphOpen.ClearConnectLine();
                }

                currentGraphOpen.UnselectAllNodes();
            }
            if (currentEvent.button == 1)
            {
                // RIGHT MOUSE BUTTON -> CREATE/POPULATE/SHOW CONTEXT MENU
                ShowContextMenu(currentEvent.mousePosition);
            }
        }

        private void ShowContextMenu(Vector2 mousePosition)
        { 
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
            contextMenu.AddItem(new GUIContent("Select All"), false, SelectAllNodes);
            contextMenu.ShowAsContext();
        }

        // EVENT PROCESSING - MOUSE MOVE
        private void ProcessMouseMoveEvent(Event currentEvent)
        {
            if (!currentGraphOpen.isTryingToConnectLine)
                return;

            currentGraphOpen.connectLinePosition += currentEvent.delta;
            GUI.changed = true;
        }

        // -------
        // DRAWING
        private void DrawRoomNodes()
        {
            foreach (var roomNode in currentGraphOpen.nodesList)
            {
                GUIStyle style = roomNodeStyle;
                if (roomNode.isSelected)
                    style = roomSelectedNodeStyle;

                roomNode.Draw(style);
            }

            GUI.changed = true;
        }

        private void DrawConnectLine()
        {
            if (!currentGraphOpen.isTryingToConnectLine)
                return;

            Handles.DrawBezier(currentGraphOpen.connectLineStartingNode.nodeRect.center, currentGraphOpen.connectLinePosition,
                currentGraphOpen.connectLineStartingNode.nodeRect.center, currentGraphOpen.connectLinePosition,
                Color.white, null, connectLineWidth);
        }

        private void DrawDefinedConnectLines()
        {
            foreach (RoomNode parentNode in currentGraphOpen.nodesList)
            {
                foreach (string childNodeID in parentNode.roomNodeChildrenIDs)
                {
                    // TRY TO GET CHILD NODE
                    if (!currentGraphOpen.nodesDict.ContainsKey(childNodeID))
                        continue;
                    RoomNode childNode = currentGraphOpen.nodesDict[childNodeID];

                    // DRAW LINE FROM PARENT NODE TO CHILD NODE
                    Handles.DrawBezier(parentNode.nodeRect.center, childNode.nodeRect.center,
                        parentNode.nodeRect.center, childNode.nodeRect.center,
                        Color.white, null, connectLineWidth);
                    
                    // DRAW LINE'S DIRECTION (ARROW)
                    // 1. GET ARROW HEAD POINT
                    Vector2 direction = childNode.nodeRect.center - parentNode.nodeRect.center;
                    Vector2 midPoint = (childNode.nodeRect.center + parentNode.nodeRect.center) / 2.0f;
                    Vector2 arrowHead = midPoint + direction.normalized * connectLineArrowSize;

                    // 2. GET ARROW UP POINT
                    Vector2 perpendicularDirection = new Vector2(-direction.y, direction.x);
                    Vector2 arrowUp = midPoint + perpendicularDirection.normalized * connectLineArrowSize;

                    // 3. GET ARROW DOWN POINT
                    Vector2 arrowDown = midPoint - perpendicularDirection.normalized * connectLineArrowSize;

                    // 4. DRAW FROM UP TO HEAD & FROM DOWN TO HEAD
                    Handles.DrawBezier(arrowUp, arrowHead, arrowUp, arrowHead, Color.white, null, connectLineWidth);
                    Handles.DrawBezier(arrowDown, arrowHead, arrowDown, arrowHead, Color.white, null, connectLineWidth);

                    GUI.changed = true;
                }
            }
        }

        // --------------
        // NODES CREATORS
        private void CreateRoomNode(object positionObject)
        {
            if (currentGraphOpen.nodesList.Count <= 0)
            {
                RoomNodeType entranceRoomType = ResourcesManager.instance.roomNodeTypes.roomNodeTypes.Find(x => x == x.isEntrance);
                CreateRoomNode(new Vector2(200.0f, 200.0f), entranceRoomType);
            }

            RoomNodeType nullRoomType = ResourcesManager.instance.roomNodeTypes.roomNodeTypes.Find(x => x == x.isNone);
            Vector2 position = (Vector2)positionObject;
            CreateRoomNode(position, nullRoomType);
        }

        private void CreateRoomNode(Vector2 position, RoomNodeType roomNodeType)
        {
            // CREATING & INITIALIZING
            RoomNode roomNode = CreateInstance<RoomNode>();
            roomNode.Initialize(new Rect(position, new Vector2(nodeWidth, nodeHeight)), currentGraphOpen, roomNodeType);
            currentGraphOpen.AddNode(roomNode);

            // SAVING
            AssetDatabase.AddObjectToAsset(roomNode, currentGraphOpen);
            AssetDatabase.SaveAssets();
        }

        // ---------
        // CALLBACKS
        // DOUBLE CLICK ON RoomNodeGraph SCRIPTABLE TO CALL OpenWindow METHOD
        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID)
        {
            RoomNodeGraph roomNodeGraph = EditorUtility.EntityIdToObject(instanceID) as RoomNodeGraph;

            if (roomNodeGraph == null)
                return false;

            OpenWindow();
            currentGraphOpen = roomNodeGraph;

            return true;
        }

        // SELECTION CALLBACKS
        private void OnSelectionChanged()
        {
            RoomNodeGraph graph = Selection.activeObject as RoomNodeGraph;
            if (graph == null)
                return;

            currentGraphOpen = graph;
            GUI.changed = true;
        }

        private void SelectAllNodes()
        {
            foreach (var node in currentGraphOpen.nodesList)
                node.isSelected = true;

            GUI.changed = true;
        }
    }
}
