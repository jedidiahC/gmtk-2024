using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StageLoader : MonoBehaviour
{
    private static StageLoader _instance = null;

    [SerializeField] private int _startSceneBuildIndex = 0;
    [SerializeField] private int _endSceneBuildIndex = 5;
    [SerializeField] private GameCamera _globalCamera = null;

    [SerializeField] private int _currentSceneIndex = 0;
    [SerializeField] private StageManager _activeStage = null;

    private List<TransformValues>[] _savedSolutions;

    public static StageLoader GetInstance() { return _instance; }

    [ContextMenu("Load next scene")]
    public void LoadNextScene()
    {
        if (_currentSceneIndex >= _endSceneBuildIndex)
        {
            return;
        }

        if (_activeStage != null)
        {
            DisableActiveStage();
        }

        _currentSceneIndex++;
        Debug.Log("Loading scene...");

        LoadLevelScene(_currentSceneIndex, LoadSceneMode.Additive);
    }

    public void SetStageActive(StageManager stage)
    {
        if (_activeStage != null)
        {
            DisableActiveStage();
        }

        _activeStage = stage;
        _activeStage.SetIsActive(true);
        UpdateCamera(stage.GetStageCamera());
    }

    private void DisableActiveStage()
    {
        Debug.Log("Disabling " + _currentSceneIndex);
        _savedSolutions[_currentSceneIndex - _startSceneBuildIndex] = _activeStage.GetCurrentSolution();
        _activeStage.Reset();
        _activeStage.SetIsActive(false);
    }

    // We disable the stage's camera and move the global camera to the new position.
    private void UpdateCamera(Camera newStageCamera)
    {
        _globalCamera.SetTargetPosition(newStageCamera.transform.position);
        newStageCamera.gameObject.SetActive(false);
    }

    void Awake()
    {
        _savedSolutions = new List<TransformValues>[_endSceneBuildIndex - _startSceneBuildIndex + 1];
        if (_instance == null)
        {
            _instance = this;
            Setup();
        }
        else if (_instance != null && _instance != this)
        {
            DestroyImmediate(gameObject);
        }
    }

    void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    private void Setup()
    {
        Debug.Assert(_globalCamera != null, "_globalCamera is not assigned!");
        _currentSceneIndex = _startSceneBuildIndex;
        LoadLevelScene(_currentSceneIndex, LoadSceneMode.Additive);
    }



    private void LoadLevelScene(int inSceneIndex, LoadSceneMode inLoadSceneMode) {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        for (int i = 0; i < eventSystems.Length; i++) {
            eventSystems[i].gameObject.SetActive(false);
        }
        // RAYNER: Note, we want to disable all eventSystems before loading the scenes.
        //         This will completely avoid the warnings.

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadSceneAsync(inSceneIndex, inLoadSceneMode);
    }

    void OnSceneLoaded(Scene inNewlyLoadedScene, LoadSceneMode inLoadSceneMode)
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        for (int i = 1; i < eventSystems.Length; i++) {
            // Only enable the first one.
            eventSystems[i].gameObject.SetActive(i == 0);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
