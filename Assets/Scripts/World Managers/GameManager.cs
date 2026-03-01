using UnityEngine;

namespace FG
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [HideInInspector] public PlayerManager player;

        [Header("Config")]
        public GameState gameState;

        [Header("Dungeon Config")]
        public DungeonLevel[] dungeonLevels;
        public int dungeonMaxCorridorsPerNode = 3;
        public int dungeonMaxGraphAttempts = 10;        // HOW MANY GRAPHS ALLOW TO TRY TO BUILD
        public int dungeonMaxBuildAttempts = 1000;      // HOW MANY TIMES ALLOW TO TRY TO BUILD ONE GRAPH

        [Header("Dungeon Debug")]
        public int currentDungeonLevelIndex;
        public Room currentRoom;
        public Room previousRoom;

        // ------------
        // UNITY EVENTS
        private void Start()
        {
            ChangeGameState(GameState.GAME_STARTED);
        }

        // --------------------------
        // GAME STATE RELATED METHODS
        public void ChangeGameState(GameState newValue)
        {
            gameState = newValue;
            switch (gameState)
            {
                case GameState.GAME_STARTED: OnGameStarted(); break;
                case GameState.PLAYING_LEVEL: break;
                case GameState.ENGAGING_ENEMIES: break;
                case GameState.PLAYING_BOSS_LEVEL: break;
                case GameState.ENGAGIN_BOSS: break;
                case GameState.LEVEL_COMPLETED: break;
                case GameState.GAME_WON: break;
                case GameState.GAME_PAUSED: break;
                case GameState.DUNGEON_OVERVIEW_MAP: break;
                case GameState.GAME_RESTART: break;
                default: break;
            }
        }

        private void OnGameStarted()
        {
            LoadDungeonLevel(currentDungeonLevelIndex);
            InstantiatePlayer();
        }

        // STATE HELPERS
        private void LoadDungeonLevel(int levelIndex)
        {
            DungeonBuilder.instance.GenerateDungeon(dungeonLevels[levelIndex]);
        }

        private void InstantiatePlayer()
        {
            // CREATE PLAYER
            GameObject playerInstance = Instantiate(ResourcesManager.instance.playerPrefab);
            player = playerInstance.GetComponent<PlayerManager>();

            // PLACE PLAYER IN THE MIDDLE OF THE CURRENT ROOM
            Vector3 targetPosition = new Vector3(
                (currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2.0f,
                (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2.0f, 0.0f);
            targetPosition = GetClosestSpawnPoint(targetPosition);
            player.transform.position = targetPosition;

            // ADD PLAYER TO CINEMACHINE TARGET GROUP
            CinemachineTargetGroupManager.instance.AddTarget(player.transform);
        }

        // -------
        // HELPERS
        private Vector3 GetClosestSpawnPoint(Vector3 desirablePosition)
        {
            Vector3 closestPosition = new(1000.0f, 1000.0f, 0.0f);
            foreach (var spawnPoint in currentRoom.spawnsPoints)
            {
                Vector3 spawnPointPositionWorld = currentRoom.instantiatedRoom.roomGrid.CellToWorld((Vector3Int)spawnPoint);
                bool isItCloser = Vector3.Distance(closestPosition, desirablePosition) > 
                    Vector3.Distance(spawnPointPositionWorld, desirablePosition);

                if (!isItCloser)
                    continue;

                closestPosition = spawnPointPositionWorld;
            }
            
            return closestPosition;
        }

        public void SetCurrentRoom(Room room)
        {
            previousRoom = currentRoom;
            currentRoom = room;
        }

        // ----------
        // VALIDATION
#if UNITY_EDITOR
        private void OnValidate()
        {
            Helpers.ValidateEnumerableProperty(this, nameof(dungeonLevels), dungeonLevels);
        }
#endif
    }
}
