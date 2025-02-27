public static class GameConstants
{
    // 事件名称
    public static class EventNames
    {
        public const string GAME_PAUSED = "GAME_PAUSED";
        public const string GAME_RESUMED = "GAME_RESUMED";

        public const string SHOW_WORD = "SHOW_WORD";

        public const string SHOW_WORD_LIST_PANEL = "SHOW_WORD_LIST_PANEL";
    }

    // 存档相关
    public static class SaveKeys
    {
        public const string PLAYER_DATA = "PlayerData";
        public const string GAME_SETTINGS = "GameSettings";
    }

    // UI路径
    public static class UIPaths
    {
        public const string MAIN_MENU = "Prefabs/MainMenu";
        public const string LOADING_PANEL = "Prefabs/LoadingPanel";
        public const string PAUSE_MENU = "Prefabs/PauseMenu";

        public const string WORD_LIST_BUTTON = "Prefabs/WordListButton";
        public const string WORD_LIST_PANEL = "Prefabs/WordListPanel";
    }
}