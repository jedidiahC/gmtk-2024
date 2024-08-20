public static class Constants {
    public const string SCENE_MENU = "SCENE_Menu";
    public const string SCENE_GAME = "SCENE_Game";

    public const string TAG_TARGET_OBJ = "TargetObj";

    public static readonly string[] SCENE_LEVEL_NAMES = {
        "SCENE_Level_1_1",
        "SCENE_Level_1_2",
        //"SCENE_Level_ManipulationBasics",
        //"SCENE_Level_ShooterBasics",
        //"SCENE_Level_GravityBasics"
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
