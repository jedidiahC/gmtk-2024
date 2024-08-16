using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class CheckScript
{
	public static void CheckAll()
	{
		ClearConsole();
		CheckAssetDatabase();

		EditorSceneManager.sceneOpened += CheckLoadingScreenScene;
		EditorMenuItems.OpenScene(EditorMenuItems.SCENE_PATH_MENU);
	}
	
	private static void CheckLoadingScreenScene(Scene inScene, OpenSceneMode inLoadSceneMode)
	{
		EditorSceneManager.sceneOpened -= CheckLoadingScreenScene;
		CheckActiveSceneHiearchy(Constants.SCENE_MENU);

		EditorSceneManager.sceneOpened += CheckDungeonScreen;
		EditorMenuItems.OpenScene(EditorMenuItems.SCENE_PATH_GAME);
	}

	private static void CheckDungeonScreen(Scene inScene, OpenSceneMode inLoadSceneMode)
	{
		EditorSceneManager.sceneOpened -= CheckDungeonScreen;
		CheckActiveSceneHiearchy(Constants.SCENE_GAME);

		Debug.Log("All Checks Completed.");
		EditorUtility.ClearProgressBar();
	}


	#region Asset Database
	private const string ASSET_PATH_PREFIX = "Assets/_Game";
	private static void CheckAssetDatabase()
	{
		string title;
		string info;
		float progress = 0.0f;

		string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

		title = "Checking Asset Database";
		info = "Scanning all assets in database...";
		progress = 0.0f;
		EditorUtility.DisplayCancelableProgressBar(title, info, progress);

		bool completedWithoutErrors = true;
		string errorMsg = string.Empty;

		try
		{
			int numAssetPaths = allAssetPaths.Length;
			for (int iAssetPath = 0; iAssetPath < numAssetPaths; iAssetPath++)
			{
				string assetPath = allAssetPaths[iAssetPath];

				// Only process assets we create, i.e. those under "_Game".
				if (assetPath.StartsWith(ASSET_PATH_PREFIX, StringComparison.Ordinal))
				{
					Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
					if (texture != null) CheckTextureAsset(texture, assetPath);

					GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
					if (gameObject != null)
					{
						CheckGameObjectRecursive(gameObject, assetPath);
						CheckModelAsset(assetPath);
					}
				}

				progress = (float)iAssetPath / (float)numAssetPaths;
				EditorUtility.DisplayCancelableProgressBar(title, info, progress);
			}
		}
		catch (Exception e)
		{
			completedWithoutErrors = false;
			errorMsg = e.Message;
		}

		if (completedWithoutErrors)
		{
			Debug.Log("Asset Database Check completed.");
		}
		else
		{
			Debug.LogError("Check interrupted: " + errorMsg);
		}
		EditorUtility.ClearProgressBar();
	}

	private static void CheckTextureAsset(Texture inTextureAsset, string inAssetPath)
	{
		if (inAssetPath.EndsWith(".otf")) return; // SKIP FONTS.

		bool isSprite = inTextureAsset.name.StartsWith("SPRITE_", StringComparison.Ordinal);
		bool isTexture = inTextureAsset.name.StartsWith("TEX_", StringComparison.Ordinal);

		if (isSprite && !inAssetPath.StartsWith(ASSET_PATH_PREFIX + "/Sprites", StringComparison.Ordinal))
			Debug.LogWarning("Sprite asset found outside of /Sprites folder!\n" + inAssetPath);

		if (isTexture && !inAssetPath.StartsWith(ASSET_PATH_PREFIX + "/Textures", StringComparison.Ordinal))
			Debug.LogWarning("Sprite asset found outside of /Sprites folder!\n" + inAssetPath);

		TextureImporter textureImporter = AssetImporter.GetAtPath(inAssetPath) as TextureImporter;
		Debug.Assert(textureImporter != null, "Unable to obtain textureImporter for " + inAssetPath);

		if (isTexture && textureImporter.mipmapEnabled)
			Debug.LogWarning("Texture asset has 'Generate Mip Maps' checked.\n" + inAssetPath);


		TextureImporterSettings textureImporterSettings = new TextureImporterSettings();
		textureImporter.ReadTextureSettings(textureImporterSettings);
		Debug.Assert(textureImporterSettings != null, "Unable to obtain spriteImporterSettings for " + inAssetPath);

		if (isSprite && textureImporterSettings.spriteGenerateFallbackPhysicsShape)
			Debug.LogWarning("Sprite asset has 'Generate Physics Shape' checked.\n" + inAssetPath);

		if (isSprite && textureImporterSettings.spriteMeshType == SpriteMeshType.FullRect)
			Debug.LogWarning("Sprite asset has Mesh Type of 'Full Rect'\n" + inAssetPath);

		// NOTE: Use this if want to enforce texture compression overrides.
/*
#if UNITY_ANDROID
		TextureImporterPlatformSettings androidTextureImporterSettings = spriteImporter.GetPlatformTextureSettings("Android");
		if (androidTextureImporterSettings.overridden)
		{
			if (androidTextureImporterSettings.format != TextureImporterFormat.ETC2_RGBA8)
			{
				Debug.LogError("Sprite Asset with wrong compression format.\n" + inAssetPath);
			}
		}
		else
		{
			Debug.LogError("Sprite Asset with wrong platform override. Ensure consistency with rest.\n" + inAssetPath);
		}
#elif UNITY_IOS
		TextureImporterPlatformSettings iosTextureImporterSettings = textureImporter.GetPlatformTextureSettings("iPhone");
		if (iosTextureImporterSettings.overridden)
		{
			if (iosTextureImporterSettings.format != TextureImporterFormat.PVRTC_RGBA4)
			{
				Debug.LogError("Sprite Asset with wrong compression format.\n" + inAssetPath);
			}
		}
		else
		{
			Debug.LogError("Sprite Asset with wrong platform override. Ensure consistency with rest.\n" + inAssetPath);
		}
#endif
*/
	}

	private static void CheckModelAsset(string inAssetPath)
	{
		if (inAssetPath.EndsWith(".prefab")) return; // Skip Prefab of the importer models.

		if (inAssetPath.EndsWith(".obj") && !inAssetPath.StartsWith(ASSET_PATH_PREFIX + "/Models", StringComparison.Ordinal))
			Debug.LogWarning("obj asset found outside of /Models folder!\n" + inAssetPath);

		ModelImporter modelImporter = AssetImporter.GetAtPath(inAssetPath) as ModelImporter;
		Debug.Assert(modelImporter != null, "Unable to obtain modelImporter for " + inAssetPath);

		if (modelImporter.importVisibility)
			Debug.LogError("Model setting import visibility is on (turn it off): " + inAssetPath);
		if (modelImporter.importCameras)
			Debug.LogError("Model setting import cameras is on (turn it off): " + inAssetPath);
		if (modelImporter.importLights)
			Debug.LogError("Model setting import lights is on (turn it off): " + inAssetPath);
		if (modelImporter.importAnimation)
			Debug.LogError("Model setting import animation is on (turn it off): " + inAssetPath);
		if (modelImporter.materialImportMode != ModelImporterMaterialImportMode.None)
			Debug.LogError("Model setting import materials is on (turn it off): " + inAssetPath);
	}


	#endregion

	private const string err = "Missing Ref in: [{3}]{0}. Component: {1}, Property: {2}";
	private static void ShowError(string context, GameObject go, string c, string property)
	{
		Debug.LogError(string.Format(err, FullPath(go), c, property, context), go);
	}

	private static void CheckActiveSceneHiearchy(string inSceneName)
	{
		string title;
		string info;
		float progress = 0.0f;

		title = "Checking Scene: " + inSceneName;
		info = "Scanning all objects in hieracrhy";
		progress = 0.0f;
		EditorUtility.DisplayCancelableProgressBar(title, info, progress);

		bool completedWithoutErrors = true;
		string errorMsg = string.Empty;

		try
		{
			GameObject[] hierarchy = SceneManager.GetActiveScene().GetRootGameObjects();
			int numGameObjects = hierarchy.Length;
			for (int iGameObject = 0; iGameObject < numGameObjects; iGameObject++)
			{
				GameObject gameObject = hierarchy[iGameObject];
				CheckGameObjectRecursive(gameObject, "[hierarchy] " + inSceneName);

				progress = (float)iGameObject / (float)numGameObjects;
				EditorUtility.DisplayCancelableProgressBar(title, info, progress);
			}
		}
		catch (Exception e)
		{
			completedWithoutErrors = false;
			errorMsg = e.Message;
		}

		if (completedWithoutErrors)
		{
			Debug.Log("Scene Check " + inSceneName + " completed.");
		}
		else
		{
			Debug.LogError("Check interrupted: " + errorMsg);
		}
		EditorUtility.ClearProgressBar();
	}

	private static void CheckGameObjectRecursive(GameObject inGameObject, string inPath)
	{
		// PROCESS THIS CURRENT GAMEOBJECT
		Component[] components = inGameObject.GetComponents<Component>();
		foreach (Component c in components)
		{
			if (!c)
			{
				Debug.LogError(inPath + ": Missing Component in GO: " + FullPath(inGameObject), inGameObject);
				continue;
			}

			SerializedObject so = new SerializedObject(c);
			var sp = so.GetIterator();

   			// Iterates through all serialized properties.
			while (sp.Next(true))
			{
				if (sp.propertyType == SerializedPropertyType.ObjectReference)
				{
					if (sp.name.StartsWith("m_")
						|| sp.name == "data"
						|| sp.name == "parentLinkedComponent") continue;

					//Debug.Log(sp.name + ", " + sp.propertyType + ", " + sp.objectReferenceValue);
					if (sp.objectReferenceValue == null)
					{
						if (ObjectNames.NicifyVariableName(sp.name) != "Prefab Parent Object")
						{
							ShowError(inPath, inGameObject, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
						}
					}
				}
			}

			if (c.GetType() == typeof(Image))
			{
				Image obj = (Image)c;
				if (obj.sprite != null && obj.sprite.name != "UIMask")
				{
					// Do what checks you want on images.
				}
			}

			if (c.GetType() == typeof(SpriteRenderer))
			{
				SpriteRenderer obj = (SpriteRenderer)c;
				if (obj.sprite != null)
				{
					// Do what checks you want on sprite renderers.
				}
			}

			// MATERIALS -  this bit of code is used to determine all the materials unsed in the game, and to safeguard against new ones being added accidentally. 
			{
				if (c.GetType() == typeof(SpriteRenderer))
				{
					SpriteRenderer obj = (SpriteRenderer)c;
					if (obj.sharedMaterial == null) Debug.LogWarning(inPath + ": " + FullPath(inGameObject) + " contains an incorrect NULL material");
				}
				else if (c.GetType() == typeof(Image))
				{
					Image obj = (Image)c;
					if (obj.material == null) Debug.LogWarning(inPath + ": " + FullPath(inGameObject) + " contains an incorrect NULL material");
				}
				else if (c.GetType() == typeof(Text))
				{
					Text obj = (Text)c;
					if (obj.material == null) Debug.LogWarning(inPath + ": " + FullPath(inGameObject) + " contains an incorrect NULL material");
				}
				else if (c.GetType() == typeof(TextMeshProUGUI))
				{
					TextMeshProUGUI obj = (TextMeshProUGUI)c;
					if (obj.material == null) Debug.LogWarning(inPath + ": " + FullPath(inGameObject) + " contains an incorrect NULL material");
				}
				else if (c.GetType() == typeof(RawImage))
				{
					RawImage obj = (RawImage)c;
					if (obj.material == null) Debug.LogWarning(inPath + ": " + FullPath(inGameObject) + " contains an incorrect NULL material");
				}
			}


			//PHYSICS
			//{
			//	if (c.GetType() == typeof(Rigidbody) || c.GetType() == typeof(Rigidbody2D))
			//	{
			//		Debug.LogError(inPath + ": " + FullPath(inGameObject) + " contains an unnecessary Rigidbody component");
			//	}
			//	// c=animation, bool animate physics
			//	if (c.GetType() == typeof(Animation))
			//	{
			//		Animation animation = (Animation)c;
			//		if (animation.animatePhysics)
			//		{
			//			Debug.LogError(inPath + ": " + FullPath(inGameObject) + " contains an Animation with the animatePhysics box checked. (please uncheck it)");
			//		}
			//	}
			//}


			if (c.GetType().IsSubclassOf(typeof(Renderer)) || c.GetType() == typeof(Renderer))
			{
				if (c.name != "default") // NOTE: Avoid the Model imports.
				{
					Renderer renderer = (Renderer)c;
					if (renderer.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off)
					{
						Debug.LogError(inPath + ": " + FullPath(inGameObject) + " contains a Renderer which casts shadows. (Turn them off)");
					}
					if (renderer.receiveShadows)
					{
						Debug.LogError(inPath + ": " + FullPath(inGameObject) + " contains a Renderer which recieves shadows. (Turn them off)");
					}
					if (renderer.reflectionProbeUsage != UnityEngine.Rendering.ReflectionProbeUsage.Off)
					{
						Debug.LogError(inPath + ": " + FullPath(inGameObject) + " contains a Renderer which uses Reflection Probes. (Turn them off)");
					}
					if (renderer.lightProbeUsage != UnityEngine.Rendering.LightProbeUsage.Off)
					{
						Debug.LogError(inPath + ": " + FullPath(inGameObject) + " contains a Renderer which uses Light Probes. (Turn them off)");
					}
				}
			}

			//BUTTONS
			{
				if (c.GetType().IsSubclassOf(typeof(Button)) || c.GetType() == typeof(Button)) // check if c is a Button
				{
					Button obj = (Button)c;
					if (obj != null)
					{
						// Do whatever checking you want on buttons.
					}
				}
			}
		}


		// RECURSIVE FOR CHILDREN.
		int numChildren = inGameObject.transform.childCount;
		for (int iChild = 0; iChild < numChildren; iChild++)
		{
			GameObject childGO = inGameObject.transform.GetChild(iChild).gameObject;
			CheckGameObjectRecursive(childGO, inPath);
		}
	}

	private static string FullPath(GameObject go)
	{
		return go.transform.parent == null
			? go.name
				: FullPath(go.transform.parent.gameObject) + "/" + go.name;
	}

	private static void ClearConsole()
	{
		// This simply does "LogEntries.Clear()" the long way:
		var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
		var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
		clearMethod.Invoke(null, null);
	}
}
