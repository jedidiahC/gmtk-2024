using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles setup and teardown for stage when first loaded from the combined scene.
public class StageGameSetup : MonoBehaviour
{
    [SerializeField] private StageManager _stage;
    private StageLoader _stageLoader;

    private void Awake()
    {
        Debug.Assert(_stage != null, "_stage is not assigned!");
    }

    private void Start()
    {
        _stageLoader = StageLoader.GetInstance();

        // If not stage loader, assume developer testing individual scene.
        if (_stageLoader == null)
        {
            Debug.Log("Solo stage...");
            return;
        }

        Setup();
    }

    void Setup()
    {
        Debug.Log("Setting up stage in game world...");
        _stageLoader.SetStageActive(_stage);
        _stage.OnStageClear.AddListener(OnStageClear);
    }

    void OnStageClear()
    {
        _stageLoader.LoadNextScene();
    }
}
