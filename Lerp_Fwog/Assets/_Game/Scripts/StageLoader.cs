using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        SceneManager.LoadSceneAsync(_currentSceneIndex, LoadSceneMode.Additive);
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
        SceneManager.LoadSceneAsync(_currentSceneIndex, LoadSceneMode.Additive);
    }
}
