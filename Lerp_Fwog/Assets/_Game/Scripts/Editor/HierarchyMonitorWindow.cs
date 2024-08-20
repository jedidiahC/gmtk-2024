using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class HierarchyMonitorWindow : EditorWindow
{
    [MenuItem("Window/Hierarchy Monitor")]
    static void CreateWindow()
    {
        EditorWindow.GetWindow<HierarchyMonitorWindow>();
    }

    void OnHierarchyChange()
    {
        var addedObjects = Resources.FindObjectsOfTypeAll<SceneTag>()
                                    .Where(x => x.isAdded < 2);

        foreach (var item in addedObjects)
        {
            GameObject assetRoot = item.gameObject;
            string assetPath = AssetDatabase.GetAssetPath(assetRoot);
            //if (item.isAdded == 0) early setup

            if (item.isAdded == 1)
            {

                // do setup here,
                // will happen just after user releases mouse
                item.sceneName = EditorSceneManager.GetActiveScene().name;
            }

            // finish with this:
            item.isAdded++;
        }
    }
}