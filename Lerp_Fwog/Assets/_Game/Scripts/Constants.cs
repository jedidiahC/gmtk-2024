public static class Constants {
    public const string SCENE_MENU = "SCENE_Menu";
    public const string SCENE_GAME = "SCENE_Game";

    public const string TAG_TARGET_OBJ = "TargetObj";

    public static readonly string[] SCENE_LEVEL_NAMES = {
        "SCENE_Level_1_1",
        "SCENE_Level_1_2",
        "SCENE_Level_1_3",
        "SCENE_Level_1_4",
        "SCENE_Level_1_5",
        "SCENE_Level_1_6",
        "SCENE_Level_1_7",
        "SCENE_Level_1_8",
        "SCENE_Level_1_9",
        "SCENE_Level_1_10",
        "SCENE_Level_1_11",
        "SCENE_Level_1_12",
        "SCENE_Level_Finale"
    };
    public static readonly int NUM_LEVELS = SCENE_LEVEL_NAMES.Length;
    public static int LEVEL_INDEX_FROM_NAME(string inSceneName) {
        for (int i = 0; i < NUM_LEVELS; i++) {
            if (SCENE_LEVEL_NAMES[i] == inSceneName) {
                return i;
            }
        }
        return -1;
    }
}
