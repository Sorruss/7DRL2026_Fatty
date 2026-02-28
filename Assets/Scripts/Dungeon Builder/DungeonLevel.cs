using UnityEngine;

namespace FG
{
    [CreateAssetMenu(menuName = "Scriptable Object/Dungeon Builder/Level")]
    public class DungeonLevel : ScriptableObject
    {
        [Header("Config")]
        public string levelName;
        public RoomTemplate[] roomTemplates;
        public RoomNodeGraph[] roomNodeGraphs;

#if UNITY_EDITOR
        private void OnValidate()
        {
            // VALIDATION
            Helpers.ValidateStringProperty(this, nameof(levelName), levelName);
            if (!Helpers.ValidateEnumerableProperty(this, nameof(roomTemplates), roomTemplates))
                return;
            if (!Helpers.ValidateEnumerableProperty(this, nameof(roomNodeGraphs), roomNodeGraphs))
                return;

            // CHECK IF SPECIAL ROOMS ARE IN THE LIST
            #region code
            bool hasHorizontalCorridor = false;
            bool hasVerticalCorridor = false;
            bool hasEntrance = false;

            foreach (var roomTemplate in roomTemplates)
            {
                if (roomTemplate.roomNodeType.isCorridorHorizontal)
                    hasHorizontalCorridor = true;

                if (roomTemplate.roomNodeType.isCorridorVertical)
                    hasVerticalCorridor = true;

                if (roomTemplate.roomNodeType.isEntrance)
                    hasEntrance = true;
            }

            if (!hasHorizontalCorridor)
            {
                Debug.Log("No horizontal corridor specified. Can't do dungeon building stuff.");
                return;
            }
            else if (!hasVerticalCorridor)
            {
                Debug.Log("No vertical corridor specified. Can't do dungeon building stuff.");
                return;
            }
            else if (!hasEntrance)
            {
                Debug.Log("No entrance specified. Can't do dungeon building stuff.");
                return;
            }
            #endregion code

            // CHECK IF ALL THE ROOM NODE TYPE SPECIFIED IN GRAPH ARE IN THE TEMPLATES
            #region code
            foreach (var graph in roomNodeGraphs)
            {
                if (graph == null)
                    continue;

                foreach (var node in graph.nodesList)
                {
                    if (node == null)
                        continue;

                    // WE ALERADY CHECKED THOSE
                    if (node.roomType.isEntrance || node.roomType.isCorridor || node.roomType.isCorridorVertical
                        || node.roomType.isCorridorVertical || node.roomType.isNone)
                        continue;

                    bool roomTypeMatch = false;
                    foreach (var roomTemplate in roomTemplates)
                    {
                        if (roomTemplate == null)
                            continue;

                        if (roomTemplate.roomNodeType == node.roomType)
                        {
                            roomTypeMatch = true;
                            break;
                        }
                    }

                    if (!roomTypeMatch)
                    {
                        Debug.Log($"In '{this.name}' no room template '{node.roomType.roomName} found for graph '{graph.name}''");
                    }
                }
            }
            #endregion code
        }
#endif
    }
}
