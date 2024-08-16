using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class EditorMenuItems
{
    public const string MENU_ITEM_PREFIX = "FWOG/";
    public const string SCENE_PATH_MENU = "Assets/_Game/Scenes/" + Constants.SCENE_MENU + ".unity";
    public const string SCENE_PATH_GAME = "Assets/_Game/Scenes/" + Constants.SCENE_GAME + ".unity";

    [MenuItem(MENU_ITEM_PREFIX + "Open Menu Scene", false, 1)]
    private static void OpenLoadingScreenScene() { OpenScene(SCENE_PATH_MENU); }

    [MenuItem(MENU_ITEM_PREFIX + "Open Game Scene", false, 2)]
    private static void OpenDungeonScene() { OpenScene(SCENE_PATH_GAME); }

	[MenuItem(MENU_ITEM_PREFIX + "Build FWOG DEBUG", false, 101)]
	private static void BuildFWOGDebug() { FWOGBuildScript.BuildFWOG(eBuildScheme.DEBUG); }

    [MenuItem(MENU_ITEM_PREFIX + "Build FWOG RELEASE", false, 110)]
    private static void BuildFWOGRelease() { FWOGBuildScript.BuildFWOG(eBuildScheme.RELEASE); }


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
