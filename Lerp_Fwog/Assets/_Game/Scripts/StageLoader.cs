using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class StageLoader : MonoBehaviour
{
    private static StageLoader _instance = null;
    public static string _currentStageName = null;

    [SerializeField] private GameCamera _globalCamera = null;
    [SerializeField] private int _currentSceneIndex = 0;
    public static int LevelIndexFromMenu = -1;
    [SerializeField] private StageManager _activeStage = null;
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClipGroup _whooshSound = null;

    private List<TransformValues>[] _savedSolutions;

    public static StageLoader GetInstance() { return _instance; }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Setup();
        }
        else if (_instance != this) DestroyImmediate(gameObject);
    }
    void OnDestroy() { if (_instance == this) _instance = null; }

    private void Setup()
    {
        Debug.Assert(_audioSource != null, "_audioSource is not assigned!");
        Debug.Assert(_whooshSound != null, "_whooshSound is not assigned!");

        _savedSolutions = new List<TransformValues>[Constants.NUM_LEVELS];

        Debug.Assert(_globalCamera != null, "_globalCamera is not assigned!");

        _currentSceneIndex = 0;
        if (LevelIndexFromMenu > -1) _currentSceneIndex = LevelIndexFromMenu;
        // RAYNER TODO: Set the Camera to that position for this initial load.
        LoadLevelScene(_currentSceneIndex, LoadSceneMode.Additive);
    }

    [ContextMenu("Load next scene")]
    public void LoadNextScene()
    {
        if (_currentSceneIndex >= Constants.NUM_LEVELS - 1)
        {
            // TODO: You can insert complete all levels here.
            return;
        }
        if (_activeStage != null) UnloadActiveStage();

        _currentSceneIndex++;
        Debug.Log("Loading scene: " + Constants.SCENE_LEVEL_NAMES[_currentSceneIndex]);
        LoadLevelScene(_currentSceneIndex, LoadSceneMode.Additive);
    }

    public void SetStageActive(StageManager stage)
    {
        _activeStage = stage;
        _activeStage.SetIsActive(true);
        _activeStage.ToggleHUD(true);
        UpdateCamera(stage.GetStageCamera());
    }

    public StageManager GetActiveStage() { return _activeStage; }

    private void UnloadActiveStage()
    {
        Debug.Log("Disabling Scene: " + Constants.SCENE_LEVEL_NAMES[_currentSceneIndex]);
        _savedSolutions[_currentSceneIndex] = _activeStage.GetCurrentSolution();
        // RAYNER: Note - Hacky way to reset the UI immediately, but leave the objects there for resetting offscreen.
        _activeStage.ResetUIOnly();
        _activeStage.ToggleHUD(false);
        StartCoroutine(DelayedReset(_activeStage));
        IEnumerator DelayedReset(StageManager inStageToReset)
        {
            yield return new WaitForSeconds(2.0f);
            SceneManager.UnloadSceneAsync(inStageToReset.gameObject.scene.name);
            Debug.Log("Unloaded Stage: " + inStageToReset.gameObject.scene.name);
        }
        _activeStage.SetIsActive(false);
    }

    // We disable the stage's camera and move the global camera to the new position.
    private void UpdateCamera(Camera newStageCamera)
    {
        _globalCamera.LerpToCameraProperties(newStageCamera);
        newStageCamera.gameObject.SetActive(false);
    }

    private void LoadLevelScene(int inSceneIndex, LoadSceneMode inLoadSceneMode)
    {
        // RAYNER: Note, we want to disable all eventSystems before loading the scenes.
        //         This will completely avoid the warnings.
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        for (int i = 0; i < eventSystems.Length; i++)
        {
            eventSystems[i].gameObject.SetActive(false);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        string sceneName = Constants.SCENE_LEVEL_NAMES[inSceneIndex];
        SceneManager.LoadSceneAsync(sceneName, inLoadSceneMode);
        _currentStageName = sceneName;
    }

    void OnSceneLoaded(Scene inNewlyLoadedScene, LoadSceneMode inLoadSceneMode)
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        for (int i = 1; i < eventSystems.Length; i++)
        {
            // Only enable the first one.
            eventSystems[i].gameObject.SetActive(i == 0);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        _whooshSound.PlayOneShot(_audioSource);
    }
}
