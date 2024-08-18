using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageLoader : MonoBehaviour
{
    private static StageLoader _instance = null;

    [SerializeField] private int _startSceneBuildIndex = 0;
    [SerializeField] private int _endSceneBuildIndex = 5;
    [SerializeField] private Camera _currentCamera = null;

    private int _currentSceneIndex = 0;


    public static StageLoader GetInstance() { return _instance; }

    [ContextMenu("Load next scene")]
    public void LoadNextScene()
    {
        if (_currentSceneIndex >= _endSceneBuildIndex)
        {
            return;
        }

        _currentSceneIndex++;
        Debug.Log("Loading scene...");

        SceneManager.LoadSceneAsync(_currentSceneIndex, LoadSceneMode.Additive);
    }

    void Awake()
    {
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
        _currentSceneIndex = _startSceneBuildIndex;
        // Do setup code here.
    }
}
