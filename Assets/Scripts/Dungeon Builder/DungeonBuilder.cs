using System.Collections.Generic;
using UnityEngine;

namespace FG
{
    public class DungeonBuilder : SingletonMonoBehaviour<DungeonBuilder>
    {
        private List<RoomNodeType> nodeTypes;
        public Dictionary<string, Room> roomsDict = new();
        private Dictionary<string, RoomTemplate> templatesDict = new();
        public bool dungeonBuildSuccessful = false;

        // ------------
        // UNITY EVENTS
        protected override void Awake()
        {
            base.Awake();

            nodeTypes = ResourcesManager.instance.roomNodeTypes.roomNodeTypes;
            ResourcesManager.instance.SetMaterialOpacity(ref ResourcesManager.instance.dimmedMaterial, 1.0f);
        }

        // ------------
        // MAIN METHODS
        public bool GenerateDungeon(DungeonLevel levelToGenerate)
        {
            // ARRAY TO DICT
            TemplateEnumerable2Dict(levelToGenerate.roomTemplates);

            // WATCHERS
            int builtAttempts = 0;
            dungeonBuildSuccessful = false;

            // MAIN LOOP
            while (!dungeonBuildSuccessful && builtAttempts <= GameManager.instance.dungeonMaxBuildAttempts)
            {
                ++builtAttempts;

                // 1. GET RANDOM NODE GRAPH
                RoomNodeGraph graph = levelToGenerate.roomNodeGraphs[Random.Range(0, levelToGenerate.roomNodeGraphs.Length)];
                int builtAttemptsPerGraph = 0;

                while (!dungeonBuildSuccessful && builtAttemptsPerGraph <= GameManager.instance.dungeonMaxGraphAttempts)
                {
                    ++builtAttemptsPerGraph;

                    // 2. CLEAR DUNGEON
                    ClearDungeon();

                    // 3. TRY TO BUILD A DUNGEON
                    dungeonBuildSuccessful = AttemptToBuildDungeonByGraph(graph);
                }

                if (dungeonBuildSuccessful)
                    InstantiateAllRooms();
            }

            return dungeonBuildSuccessful;
        }

        private bool AttemptToBuildDungeonByGraph(RoomNodeGraph graph)
        {
            // 1. ADDING STARTING ROOM (ENTRANCE) TO THE QUEUE
            Queue<RoomNode> roomNodesQueue = new();
            RoomNode entrance = graph.GetNodeByType(nodeTypes.Find(x => x.isEntrance));
            if (entrance == null)
            {
                Debug.Log("No Entrance Node");
                return false;
            }

            roomNodesQueue.Enqueue(entrance);

            // 2. MAIN LOOP
            bool noRoomOverlaps = true;
            while (noRoomOverlaps && roomNodesQueue.Count > 0)
            {
                // 2.1 GET CURRENT NODE
                // 2.2 ADD NODE'S CHILDREN TO QUEUE
                RoomNode nextNode = roomNodesQueue.Dequeue();
                foreach (RoomNode child in graph.GetNextChildByNode(nextNode))
                    roomNodesQueue.Enqueue(child);

                // 2.3 GET RANDOM TEMPLATE ROOM THAT MATCHES THIS NODE'S ROOM TYPE
                // 2.4 CREATE ROOM CLASS
                // 2.5 ATTEMPT TO PLACE THE ROOM
                if (nextNode.roomType.isEntrance)
                {
                    // IF IT'S AN ENTRANCE -> JUST PLACE IT
                    RoomTemplate template = GetRandomMatchingTemplateByType(nextNode.roomType);
                    Room room = new(nextNode, template);
                    room.isPositioned = true;
                    roomsDict.Add(room.ID, room);
                }
                else
                {
                    // IT'S NOT AN ENTRANCE -> IT HAS PARENT
                    Room parentRoom = roomsDict[nextNode.roomNodeParentIDs[0]];
                    noRoomOverlaps = CanPlaceRoomWithNoOverlaps(nextNode, parentRoom);
                }
            }

            return noRoomOverlaps && roomNodesQueue.Count <= 0;
        }

        private void InstantiateAllRooms()
        {
            foreach (Room room in roomsDict.Values)
            {
                Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x,
                    room.lowerBounds.y - room.templateLowerBounds.y, 0.0f);
                GameObject roomGameObject = Instantiate(room.roomPrefab, roomPosition, Quaternion.identity, transform);
                InstantiatedRoom instantiatedRoom = roomGameObject.GetComponent<InstantiatedRoom>();
                instantiatedRoom.Init(room);
                room.instantiatedRoom = instantiatedRoom;
            }
        }

        // ------------
        // MAIN HELPERS
        private bool CanPlaceRoomWithNoOverlaps(RoomNode child, Room parent)
        {
            bool roomOverlaps = true;

            // TRYING TO PLACE CHILD TO ONE OF PARENT'S DOORWAYS SUCCESSFULLY
            while (roomOverlaps)
            {
                List<Doorway> validDoorways = parent.doorways.FindAll(x => !x.isConnected && !x.isUnavailable);
                if (validDoorways.Count == 0)
                    return false;

                Doorway doorway = validDoorways[Random.Range(0, validDoorways.Count)];

                // GET VALID TEMPLATE TO DOORWAY (TYPE & ORIENTATION)
                RoomTemplate template = null;
                if (child.roomType.isCorridor)
                {
                    switch (doorway.orientation)
                    {
                        case Orientation.north:
                        case Orientation.south:
                            template = GetRandomMatchingTemplateByType(nodeTypes.Find(x => x.isCorridorVertical));
                            break;
                        case Orientation.west:
                        case Orientation.east:
                            template = GetRandomMatchingTemplateByType(nodeTypes.Find(x => x.isCorridorHorizontal));
                            break;
                        case Orientation.none: break;
                        default: break;
                    }
                }
                else
                {
                    template = GetRandomMatchingTemplateByType(child.roomType);
                }

                if (template == null)
                    continue;

                // FIND OUT IF ROOM IS PLACABLE
                Room room = new(child, template);
                if (PlaceTheRoom(room, parent, doorway))
                {
                    roomOverlaps = false;
                    room.isPositioned = true;
                    roomsDict.Add(room.ID, room);
                }
                else
                {
                    roomOverlaps = true;
                }
            }

            return true;
        }

        private bool PlaceTheRoom(Room roomToPlace, Room parentRoom, Doorway parentDoorway)
        {
            // GET OPPOSITE DOORWAY
            Doorway oppositeDoorway = GetOppositeDoorway(parentDoorway, roomToPlace.doorways);
            if (oppositeDoorway == null)
            {
                parentDoorway.isUnavailable = true;
                return false;
            }

            // CALCULATING ROOM'S LOWER & UPPER BOUNDS
            Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + parentDoorway.position - parentRoom.templateLowerBounds;
            Vector2Int adjustment = Vector2Int.zero;

            switch (oppositeDoorway.orientation)
            {
                case Orientation.north: adjustment.y = -1; break;
                case Orientation.south: adjustment.y = 1; break;
                case Orientation.east: adjustment.x = -1; break;
                case Orientation.west: adjustment.x = 1; break;
                default: break;
            }

            roomToPlace.lowerBounds = parentDoorwayPosition + adjustment + roomToPlace.templateLowerBounds - oppositeDoorway.position;
            roomToPlace.upperBounds = roomToPlace.lowerBounds + roomToPlace.templateUpperBounds - roomToPlace.templateLowerBounds;

            Room overlappingRoom = CheckForOverlappingRoom(roomToPlace);
            if (overlappingRoom == null)
            {
                parentDoorway.isConnected = true;
                parentDoorway.isUnavailable = true;

                oppositeDoorway.isConnected = true;
                oppositeDoorway.isUnavailable = true;

                return true;
            }

            parentDoorway.isUnavailable = true;
            return false;
        }

        private Room CheckForOverlappingRoom(Room roomToCheck)
        {
            foreach (var room in roomsDict.Values)
            {
                if (!room.isPositioned || room.ID == roomToCheck.ID)
                    continue;

                if (!AreRoomsOverlapping(roomToCheck, room))
                    continue;

                return room;
            }

            return null;
        }

        private bool AreRoomsOverlapping(Room room1, Room room2)
        {
            bool isOverlappingX = Helpers.IsInBounds(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);
            bool isOverlappingY = Helpers.IsInBounds(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

            return isOverlappingX && isOverlappingY;
        }

        private Doorway GetOppositeDoorway(Doorway doorwayToMatch, List<Doorway> doorways)
        {
            foreach (Doorway doorway in doorways)
            {
                // NORTH -> SOUTH
                if (doorwayToMatch.orientation == Orientation.south && doorway.orientation == Orientation.north)
                    return doorway;

                // SOUTH -> NORTH
                if (doorwayToMatch.orientation == Orientation.north && doorway.orientation == Orientation.south)
                    return doorway;

                // WEST -> EAST
                if (doorwayToMatch.orientation == Orientation.west && doorway.orientation == Orientation.east)
                    return doorway;

                // EAST -> WEST
                if (doorwayToMatch.orientation == Orientation.east && doorway.orientation == Orientation.west)
                    return doorway;
            }

            return null;
        }

        private void ClearDungeon()
        {
            foreach (Room room in roomsDict.Values)
            {
                if (room.instantiatedRoom == null)
                    continue;

                Destroy(room.instantiatedRoom.gameObject);
            }

            roomsDict.Clear();
        }

        // -------------
        // SUPPLEMENTARY
        private void TemplateEnumerable2Dict(RoomTemplate[] templatesList)
        {
            templatesDict.Clear();
            foreach (RoomTemplate template in templatesList)
            {
                if (!templatesDict.ContainsKey(template.ID))
                    templatesDict.Add(template.ID, template);
                else
                    Debug.Log($"Duplicate room template in level {GameManager.instance.currentDungeonLevelIndex + 1}");
            }
        }

        private RoomTemplate GetRandomMatchingTemplateByType(RoomNodeType roomType)
        {
            // 1. GETTING ALL THE MATCHING TEMPLATES
            List<RoomTemplate> matchingTemplates = new();
            foreach (RoomTemplate roomTemplate in templatesDict.Values)
            {
                if (roomTemplate.roomNodeType != roomType)
                    continue;

                matchingTemplates.Add(roomTemplate);
            }

            if (matchingTemplates.Count == 0)
                return null;

            // 2. GETTING RANDOM ONE AMONG THEM
            return matchingTemplates[Random.Range(0, matchingTemplates.Count)];
        }

        private Room GetRoomByID(string ID)
        {
            if (roomsDict.ContainsKey(ID))
                return roomsDict[ID];

            return null;
        }

        private RoomTemplate GetTemplateByID(string ID)
        {
            if (templatesDict.ContainsKey(ID))
                return templatesDict[ID];

            return null;
        }
    }
}
