using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class EditorMenuItems
{
    public const string BUILD_AND_RUN_KEY = "EDITOR_BUILD_AND_RUN_KEY";
    public const string MENU_ITEM_PREFIX = "FWOG/";
    public const string SCENE_PATH_MENU = "Assets/_Game/Scenes/" + Constants.SCENE_MENU + ".unity";
    public const string SCENE_PATH_GAME = "Assets/_Game/Scenes/" + Constants.SCENE_GAME + ".unity";

    [MenuItem(MENU_ITEM_PREFIX + "Open Menu Scene", false, 1)]
    private static void OpenLoadingScreenScene() { OpenScene(SCENE_PATH_MENU); }

    [MenuItem(MENU_ITEM_PREFIX + "Open Game Scene", false, 2)]
    private static void OpenDungeonScene() { OpenScene(SCENE_PATH_GAME); }

	[MenuItem(MENU_ITEM_PREFIX + "Build FWOG DEBUG", false, 101)]
	private static void BuildFWOGDebug() { FWOGBuildScript.BuildFWOG(eBuildScheme.DEBUG, EditorPrefs.GetBool(BUILD_AND_RUN_KEY)); }

    [MenuItem(MENU_ITEM_PREFIX + "Build FWOG RELEASE", false, 110)]
    private static void BuildFWOGRelease() { FWOGBuildScript.BuildFWOG(eBuildScheme.RELEASE, false); }


    [MenuItem(MENU_ITEM_PREFIX + "CHECK ALL", false, 201)]
    private static void CheckAll() { CheckScript.CheckAll(); }





    public static void OpenScene(string inPath, OpenSceneMode inSceneMode = OpenSceneMode.Single)
    {
        // TODO: Check if the path exits.
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(inPath, inSceneMode);
        }
    }
}

[InitializeOnLoad] // NOTE: Needed to call the static constructor.
public static class EditorMenuCheckmarkItems
{
    private static bool _buildAndRun;
    private const string BUILD_AND_RUN_CHECKMARK = EditorMenuItems.MENU_ITEM_PREFIX + "Build and Run";

    static EditorMenuCheckmarkItems()
    {
        if (!EditorPrefs.HasKey(EditorMenuItems.BUILD_AND_RUN_KEY)) {
            EditorPrefs.SetBool(EditorMenuItems.BUILD_AND_RUN_KEY, false);
        }
        _buildAndRun = EditorPrefs.GetBool(EditorMenuItems.BUILD_AND_RUN_KEY);


        /// Delaying until first editor tick so that the menu
        /// will be populated before setting check state, and
        /// re-apply correct action
        EditorApplication.delayCall += () => {
            ToggleBuildAndRun(_buildAndRun);
        };
    }

    [MenuItem(BUILD_AND_RUN_CHECKMARK, false, 121)]
    private static void ToggleAction() {
        ToggleBuildAndRun(!_buildAndRun);
    }

    public static void ToggleBuildAndRun(bool inBuildAndRun) {
        _buildAndRun = inBuildAndRun;
        EditorPrefs.SetBool(EditorMenuItems.BUILD_AND_RUN_KEY, inBuildAndRun);
        UnityEditor.Menu.SetChecked(BUILD_AND_RUN_CHECKMARK, inBuildAndRun);
    }
}
