using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace FG
{
    public class RoomNodeGraphEditor : EditorWindow
    {
        // NODE STYLES
        private GUIStyle roomNodeStyle;
        private GUIStyle roomSelectedNodeStyle;

        // NEEDED VARIABLES
        private static RoomNodeGraph currentGraphOpen;
        private RoomNode currentNodeSelected;

        [Header("Grid Config")]
        private int bigGridSize = 120;
        private int smallGridSize = 30;
        private Vector2 graphOffset;
        private Vector2 graphDrag;

        [Header("Node Config")]
        private int nodeWidth = 160;
        private int nodeHeight = 75;
        private int nodeBorderOffset = 12;
        private int nodePaddingOffset = 25;

        [Header("Connect Line Config")]
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
                DrawGrid(smallGridSize, 0.2f, Color.gray);
                DrawGrid(bigGridSize, 0.3f, Color.gray);
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
            // FIND_OUT: IDK WHY
            graphDrag = Vector2.zero;

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
                case EventType.MouseDrag: ProcessMouseDragEvent(currentEvent); break;
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
            else if (currentEvent.button == 1)
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
            contextMenu.AddSeparator("");
            contextMenu.AddItem(new GUIContent("Delete Selected Connections"), false, DeleteSelectedConnections);
            contextMenu.AddItem(new GUIContent("Delete Selected Nodes"), false, DeleteSelectedNodes);
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

        // EVENT PROCESSING - MOUSE DRAG
        private void ProcessMouseDragEvent(Event currentEvent)
        {
            if (currentEvent.button == 2)
            {
                // MIDDLE MOUSE BUTTON -> CANVAS DRAG
                // 1. DRAG THE GRAPH (ALL THE NODES)
                foreach (RoomNode node in currentGraphOpen.nodesList)
                    node.Drag(currentEvent.delta);

                // 2. DRAG THE GRID
                graphDrag = currentEvent.delta;

                // END
                GUI.changed = true;
            }
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

        private void DrawGrid(int gridSize, float gridOpacity, Color gridColor)
        {
            // SET COLOR
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            // CALCULATE LINES COUNT
            int horizontalLinesCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
            int verticalLinesCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

            // FIND_OUT:
            graphOffset += graphDrag * 0.5f;
            Vector3 gridOffset = new(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

            // DRAW HORIZONTAL LINES
            for (int i = 0; i < horizontalLinesCount; ++i)
            {
                Handles.DrawLine(
                    new Vector3(gridSize * i, -gridSize, 0.0f) + gridOffset, 
                    new Vector3(gridSize * i, position.height + gridSize, 0.0f) + gridOffset);
            }

            // DRAW VERTICAL LINES
            for (int i = 0; i < verticalLinesCount; ++i)
            {
                Handles.DrawLine(
                    new Vector3(-gridSize, gridSize * i, 0.0f) + gridOffset,
                    new Vector3(position.width + gridSize, gridSize * i, 0.0f) + gridOffset);
            }

            // RESET COLOR
            Handles.color = Color.white;
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

            RoomNodeType corridorRoomType = ResourcesManager.instance.roomNodeTypes.roomNodeTypes.Find(x => x == x.isCorridor);
            Vector2 position = (Vector2)positionObject;
            CreateRoomNode(position, corridorRoomType);
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

        // DELETION
        private void DeleteSelectedConnections()
        {
            foreach (RoomNode node in currentGraphOpen.nodesList)
            {
                if (node.isSelected)
                    node.OnDeleteConnections();
            }
        }

        private void DeleteSelectedNodes()
        {
            for (int i = currentGraphOpen.nodesList.Count - 1; i >= 0; --i)
            {
                RoomNode node = currentGraphOpen.nodesList[i];
                if (node.isSelected && !node.roomType.isEntrance)
                    node.OnDeleteNode();
            }
        }
    }
}
