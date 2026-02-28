using System.Collections.Generic;
using UnityEngine;

namespace FG
{
    public class DungeonBuilder : SingletonMonoBehaviour<DungeonBuilder>
    {
        [Header("Debug")]
        public Dictionary<string, Room> roomsDict = new();

        private Dictionary<string, RoomTemplate> templatesDict = new();
        private bool dungeonBuildSuccess = false;
        private List<RoomNodeType> nodeTypes;

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
            TemplateEnumerable2Dict(ref levelToGenerate.roomTemplates);

            // WATCHERS
            int builtAttempts = 0;
            dungeonBuildSuccess = false;

            // MAIN LOOP
            while (!dungeonBuildSuccess && builtAttempts <= GameManager.instance.dungeonMaxBuildAttempts)
            {
                ++builtAttempts;

                // 1. GET RANDOM NODE GRAPH
                RoomNodeGraph graph = levelToGenerate.roomNodeGraphs[Random.Range(0, levelToGenerate.roomNodeGraphs.Length)];
                int builtAttemptsPerGraph = 0;

                while (builtAttemptsPerGraph <= GameManager.instance.dungeonMaxGraphAttempts)
                {
                    ++builtAttemptsPerGraph;

                    // 2. CLEAR DUNGEON
                    ClearDungeon();

                    // 3. TRY TO BUILD A DUNGEON
                    AttemptToBuildDungeonByGraph(ref graph);

                    // 4. SEE IF IT SUCCEEDED
                    if (dungeonBuildSuccess)
                    {
                        InstantiateAllRooms();
                        return dungeonBuildSuccess;
                    }
                }
            }

            return dungeonBuildSuccess;
        }

        private void AttemptToBuildDungeonByGraph(ref RoomNodeGraph graph)
        {

        }

        private void InstantiateAllRooms()
        {

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
            templatesDict.Clear();
        }

        // -------------
        // SUPPLEMENTARY
        private void TemplateEnumerable2Dict(ref RoomTemplate[] templatesList)
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
    }
}
