public static class GameConstants
{
    // 事件名称
    public static class EventNames
    {
        public const string GAME_PAUSED = "GAME_PAUSED";
        public const string GAME_RESUMED = "GAME_RESUMED";

        public const string SHOW_WORD = "SHOW_WORD";

        public const string SHOW_WORD_LIST_PANEL = "SHOW_WORD_LIST_PANEL";
        public const string SHOW_BGM_PANEL = "SHOW_BGM_LIST_PANEL";
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
        
        public const string BGM_LIST_BUTTON = "Prefabs/BgmButtonPrefab";
        public const string BGM_LIST_PANEL = "Prefabs/BgmListPanel";
        public const string BGM_OPTION_BUTTON = "Prefabs/BgmOptionButton";

        public const string FRAME = "Prefabs/Frame";
    }

    public static class AudioPaths
    {
        public const string BGM1 = "Audios/BGM1";
        public const string BGM2 = "Audios/BGM2";
        public const string BGM3 = "Audios/BGM3";
        public const string BGM4 = "Audios/BGM4";
    }
}