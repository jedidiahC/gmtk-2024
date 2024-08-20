using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;


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

    public static void BuildFWOG(eBuildScheme inBuildScheme, bool run = false) {
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
            PlayerSettings.productName = "one small leap for frog";
            PlayerSettings.SetApplicationIdentifier(buildTargetGroup, "com.fwog.lerp");
            PlayerSettings.bundleVersion = VersionClass.BUNDLE_VERSION;
			PlayerSettings.SetScriptingBackend(buildTargetGroup, ScriptingImplementation.IL2CPP);
            #if UNITY_WEBGL
            // TODO: Find a way to set the WebGL Code Optimization to
            //       Runtime Speed for release and Shorter Build Time for debug
            if (inBuildScheme == eBuildScheme.RELEASE) {
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            } else {
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            }
            PlayerSettings.defaultWebScreenWidth = 1920;
            PlayerSettings.defaultWebScreenHeight = 1080;
            #endif
			#endregion

            List<String> sceneList = new List<String>();
            sceneList.Add(EditorMenuItems.SCENE_PATH_MENU); // NOTE: Has to be first so when building this is the default scene loaded.
            sceneList.Add(EditorMenuItems.SCENE_PATH_GAME);
            for (int i = 0; i < Constants.NUM_LEVELS; i++) {
                string scenePath = EditorMenuItems.SCENE_PATH_PREFIX
                                + Constants.SCENE_LEVEL_NAMES[i]
                                + EditorMenuItems.SCENE_PATH_POSTFIX;
                Debug.Log("Adding scene: " + scenePath);
                sceneList.Add(scenePath);
            }

			// NOTE: Set unity scenes to be included in the build.
			string[] scenes = sceneList.ToArray();

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
            outputFileName = "one small leap for frog";

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
                if (run) {
#if UNITY_STANDALONE
                    System.Diagnostics.Process.Start(buildPath);
#elif UNITY_WEBGL
                    string runInstructionStr = "You need to run the server locally...\n" +
                    "Open Terminal/Powershell and run the following command:\n\n" +
                    #if UNITY_EDITOR_OSX
                    "python3 -m http.server --directory \"" + buildPath + "\"\n\n" +
                    #elif UNITY_EDITOR_WIN
                    "python -m http.server --directory \"" + buildPath + "\"\n\n" +
                    "OR\n\n"+
                    "py -m http.server --directory \"" + buildPath + "\"\n\n" +
                    #endif
                    "Then with the server running, go to your web browser and enter\n" +
                    "localhost:8000\n";

                    EditorUtility.DisplayDialog("Run Instructions", runInstructionStr, "Got it!");
#endif
                }
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


//     private static void StartLocalServer(string buildPath)
//     {
// #if UNITY_EDITOR_OSX
//         // On macOS, use Python's HTTP server (Python 3.x)
//         string serverCommand = $"python3 -m http.server --directory \"{buildPath}\"";
// #elif UNITY_EDITOR_WIN
//         // On other platforms, use the default Python command
//         string serverCommand = $"python -m http.server --directory \"{buildPath}\"";
// #endif

//         System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo
//         {
//             FileName = "/bin/zsh",
//             Arguments = $"-c \"{serverCommand}\"",
//             WorkingDirectory = buildPath,
//             RedirectStandardOutput = true,
//             RedirectStandardError = true,
//             UseShellExecute = false,
//             CreateNoWindow = true // This does nothing?!
//         };

//         try {
//             using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(processInfo)) {
//                 if (process == null) {
//                     Debug.LogError("failed to start the local server.");
//                     return;
//                 }

//                 // Read output and error streams for debugging
//                 string output = process.StandardOutput.ReadToEnd();
//                 string error = process.StandardError.ReadToEnd();

//                 // This will hang Unity!!! Needs some other way to keep this process running...
//                 process.WaitForExit();

//                 Debug.Log("Server output: " + output);
//                 if (!string.IsNullOrEmpty(error))
//                 {
//                     Debug.LogError("Server error: " + error);
//                 }
//             }
//         }
//         catch (System.Exception e)
//         {
//             UnityEngine.Debug.LogError($"Error starting local server: {e.Message}");
//         }

//         // Opens the browser. This works.
//         string url = "http://localhost:8000"; // Default port for Python HTTP server
//         System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
//         {
//             FileName = url,
//             UseShellExecute = true
//         });
//     }
}
