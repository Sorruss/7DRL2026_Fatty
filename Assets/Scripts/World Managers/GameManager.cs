using UnityEngine;

namespace FG
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        [Header("Config")]
        public GameState gameState;

        [Header("Dungeon Config")]
        public DungeonLevel[] dungeonLevels;
        public int dungeonMaxCorridorsPerNode = 3;
        public int dungeonMaxGraphAttempts = 10;        // HOW MANY GRAPHS ALLOW TO TRY TO BUILD
        public int dungeonMaxBuildAttempts = 1000;      // HOW MANY TIMES ALLOW TO TRY TO BUILD ONE GRAPH

        [Header("Dungeon Debug")]
        public int currentDungeonLevelIndex;

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
                case GameState.GAME_STARTED: LoadDungeonLevel(currentDungeonLevelIndex); break;
                case GameState.PLAYING_LEVEL: break;
                case GameState.ENGAGING_ENEMIES: break;
                case GameState.PLAYING_BOSS_LEVEL: break;
                case GameState.ENGAGIN_BOSS: break;
                case GameState.LEVEL_COMPLETED: break;
                case GameState.GAME_WON: break;
                case GameState.GAME_PAUSED: break;
                case GameState.DUNGEON_OVERVIEW_MAP: break;
                case GameState.GAME_RESTART: LoadDungeonLevel(currentDungeonLevelIndex); break;
                default: break;
            }
        }

        private void LoadDungeonLevel(int levelIndex)
        {
            DungeonBuilder.instance.GenerateDungeon(dungeonLevels[levelIndex]);
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
