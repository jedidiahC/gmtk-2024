using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Threading.Tasks;


public enum eBuildScheme { RELEASE, DEBUG };

public class FWOGBuildScript
{

#if UNITY_STANDALONE
	private static BuildTargetGroup buildTargetGroup = BuildTargetGroup.Standalone;
#if UNITY_STANDALONE_OSX
    private static BuildTarget buildTarget = BuildTarget.StandaloneOSX;
#elif UNITY_STANDALONE_WIN
#if UNITY_64
	private static BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
#else
	private static BuildTarget buildTarget = BuildTarget.StandaloneWindows;
#endif
#endif

#elif UNITY_WEBGL
	private static BuildTargetGroup buildTargetGroup = BuildTargetGroup.WebGL;
	private static BuildTarget buildTarget = BuildTarget.WebGL;
#endif

    public static void BuildFWOG(eBuildScheme inBuildScheme, bool inAllowAppendXCodeProj = false) {
        try {
            #region Scripting defines
            string scriptingDefines = "FWOG_" + inBuildScheme.ToString() + ";";
            switch (inBuildScheme) {
                // NOTE: Add any additional plugins related scripting defines needed here.
                case eBuildScheme.RELEASE:
                {
                    //scriptingDefines += "";
                    break;
                }
                case eBuildScheme.DEBUG:
                {
                    //scriptingDefines += "";
                    break;
                }
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, scriptingDefines);
            #endregion

            #region PlayerSettings
            PlayerSettings.companyName = "LERP_FWOG";
            PlayerSettings.productName = "Lerp Fwog";
            PlayerSettings.SetApplicationIdentifier(buildTargetGroup, "com.fwog.lerp");
            PlayerSettings.bundleVersion = VersionClass.BUNDLE_VERSION;
			PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
			#endregion

			// NOTE: Set unity scenes to be included in the build.
			string[] scenes = new string[]
            {
                EditorMenuItems.SCENE_PATH_MENU, // NOTE: Has to be first so when building this is the default scene loaded.
                EditorMenuItems.SCENE_PATH_GAME,
            };

			string outputFileName;
            string buildDirectory;
            BuildOptions buildOptions = BuildOptions.ShowBuiltPlayer;
#if UNITY_EDITOR_OSX
			string projectRootPath = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
			buildDirectory = Path.Combine(projectRootPath, "build/");
#elif UNITY_EDITOR_WIN
			string projectRootPath = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
        	buildDirectory = Path.Combine(projectRootPath, "build\\");
#endif
            Debug.Log("Build Directory Path: " + buildDirectory);

#if UNITY_STANDALONE_OSX
            outputFileName = "FWOG_" + inBuildScheme.ToString() + "_v" + VersionClass.BUNDLE_VERSION + ".app";
#elif UNITY_STANDALONE_WIN
            outputFileName = "FWOG_" + inBuildScheme.ToString() + "_v" + VersionClass.BUNDLE_VERSION + ".exe";
#elif UNITY_WEBGL
            outputFileName = "FWOG_" + inBuildScheme.ToString() + "_v" + VersionClass.BUNDLE_VERSION + "_WebGL";
#endif
            Debug.Log("Output File Name: " + outputFileName);

            if (!Directory.Exists(buildDirectory)) Directory.CreateDirectory(buildDirectory);
            if (inBuildScheme != eBuildScheme.RELEASE) { // NOTE: Respect the settings in the Build Settings, only if it's not a release build.
                if (EditorUserBuildSettings.development) {
                    buildOptions |= BuildOptions.Development;
                    if (EditorUserBuildSettings.connectProfiler) buildOptions |= BuildOptions.ConnectWithProfiler;
                    if (EditorUserBuildSettings.allowDebugging) buildOptions |= BuildOptions.AllowDebugging;
                }
            }

            string buildPath = Path.Combine(buildDirectory, outputFileName);
            // The actual build happens here!
			UnityEditor.Build.Reporting.BuildReport buildReport = BuildPipeline.BuildPlayer(scenes, buildPath, buildTarget, buildOptions);

			if (buildReport.summary.totalErrors < 1) {
                Debug.Log("FWOG " + inBuildScheme.ToString() + " build completed successfully.");
            }
            else {
				string buildStepsMessages = "";
				for (int iStep = 0; iStep < buildReport.steps.Length; iStep++)
				{
					UnityEditor.Build.Reporting.BuildStepMessage[] stepMsgs = buildReport.steps[iStep].messages;
					for (int iMsg = 0; iMsg < stepMsgs.Length; iMsg++)
					{
						buildStepsMessages += stepMsgs[iMsg].content + "\n";
					}
					buildStepsMessages += "\n";
				}
				EditorUtility.DisplayDialog("Oops!", "Error encountered while building: " + buildReport.summary.ToString(), "ok");
            }
        }
        catch (Exception e) {
            EditorUtility.DisplayDialog("Ooops!", "Error encountered while building: " + e, "ok");
        }
    }

    private static void ResetScriptingDefineSymbols(BuildTargetGroup targetGroup) {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Empty);
        // NOTE: SaveAssets and Refresh are needed else the scripting define symbols don't update player settings.
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        string scriptingDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        Debug.Log("Scripting define symbols resetted: " + scriptingDefines);
    }

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject) {
        Debug.Log("Running build post-process...");
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
        ResetScriptingDefineSymbols(buildTargetGroup);
    }
}
