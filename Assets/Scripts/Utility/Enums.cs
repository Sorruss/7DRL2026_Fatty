namespace FG
{
    public enum Orientation     // USED BY DOORWAYS FOR DUNGEON BUILDER
    {
        north,
        east,
        south,
        west,
        none
    }

    public enum GameState       // USED BY GAMEMANAGER
    {
        GAME_STARTED = 1,
        PLAYING_LEVEL,
        ENGAGING_ENEMIES,
        PLAYING_BOSS_LEVEL,
        ENGAGIN_BOSS,
        LEVEL_COMPLETED,
        GAME_WON,
        GAME_PAUSED,
        DUNGEON_OVERVIEW_MAP,
        GAME_RESTART
    }
}
