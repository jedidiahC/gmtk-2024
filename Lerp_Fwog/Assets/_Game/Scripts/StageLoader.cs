using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class StageLoader : MonoBehaviour
{
    private static StageLoader _instance = null;

    [SerializeField] private GameCamera _globalCamera = null;
    [SerializeField] private int _currentSceneIndex = 0;
    [SerializeField] private StageManager _activeStage = null;

    private List<TransformValues>[] _savedSolutions;

    public static StageLoader GetInstance() { return _instance; }

    void Awake() {
        if (_instance == null) {
            _instance = this;
            Setup();
        }
        else if (_instance != this) DestroyImmediate(gameObject);
    }
    void OnDestroy() { if (_instance == this) _instance = null; }

    private void Setup() {
        _savedSolutions = new List<TransformValues>[Constants.NUM_LEVELS];
        Debug.Assert(_globalCamera != null, "_globalCamera is not assigned!");
        _currentSceneIndex = 0;
        LoadLevelScene(_currentSceneIndex, LoadSceneMode.Additive);
    }

    [ContextMenu("Load next scene")]
    public void LoadNextScene() {
        if (_currentSceneIndex >= Constants.NUM_LEVELS - 1) {
            // TODO: You can insert complete all levels here.
            return;
        }
        if (_activeStage != null) DisableActiveStage();

        _currentSceneIndex++;
        Debug.Log("Loading scene: " + Constants.SCENE_LEVEL_NAMES[_currentSceneIndex]);
        LoadLevelScene(_currentSceneIndex, LoadSceneMode.Additive);
    }

    public void SetStageActive(StageManager stage) {
        if (_activeStage != null) DisableActiveStage();

        _activeStage = stage;
        _activeStage.SetIsActive(true);
        UpdateCamera(stage.GetStageCamera());
    }

    public StageManager GetActiveStage() { return _activeStage; }

    private void DisableActiveStage() {
        Debug.Log("Disabling Scene: " + Constants.SCENE_LEVEL_NAMES[_currentSceneIndex]);
        _savedSolutions[_currentSceneIndex] = _activeStage.GetCurrentSolution();
        // RAYNER: Note - Hacky way to reset the UI immediately, but leave the objects there for resetting offscreen.
        _activeStage.ResetUIOnly();
        StartCoroutine(DelayedReset(_activeStage));
        IEnumerator DelayedReset(StageManager inStageToReset) {
            yield return new WaitForSeconds(1.0f);
            inStageToReset.Reset();
            Debug.Log("RAN RESET on " + inStageToReset.gameObject.scene.name);
        }
        _activeStage.SetIsActive(false);
    }

    // We disable the stage's camera and move the global camera to the new position.
    private void UpdateCamera(Camera newStageCamera) {
        _globalCamera.SetTargetPosition(newStageCamera.transform.position);
        newStageCamera.gameObject.SetActive(false);
    }





    private void LoadLevelScene(int inSceneIndex, LoadSceneMode inLoadSceneMode) {
        // RAYNER: Note, we want to disable all eventSystems before loading the scenes.
        //         This will completely avoid the warnings.
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        for (int i = 0; i < eventSystems.Length; i++) {
            eventSystems[i].gameObject.SetActive(false);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        string sceneName = Constants.SCENE_LEVEL_NAMES[inSceneIndex];
        SceneManager.LoadSceneAsync(sceneName, inLoadSceneMode);
    }

    void OnSceneLoaded(Scene inNewlyLoadedScene, LoadSceneMode inLoadSceneMode) {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        for (int i = 1; i < eventSystems.Length; i++) {
            // Only enable the first one.
            eventSystems[i].gameObject.SetActive(i == 0);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
