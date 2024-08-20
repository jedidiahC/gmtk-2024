using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    public UnityEvent OnNextStage = new();
    public UnityEvent OnStageClear = new();

    [SerializeField] private LevelClearCanvas _levelClearCanvas = null;
    [SerializeField] private GameObject _hudCanvas = null;
    [SerializeField] private List<TargetObject> _targetObjects = null;
    [SerializeField] private List<TargetArea> _targetAreas = null;
    [SerializeField] private List<Rigidbody2D> _dynamics = null;
    [SerializeField] private Camera _camera = null;

    public Camera GetStageCamera() { return _camera; }

    private List<TransformValues> _dynamicTransformVals = null;
    private List<TransformValues> _originalDynamicTransformVals = null;

    private bool _isSimulating = false;
    private bool _isActive = false;
    private int _score = 0;

    public void SetIsActive(bool isActive)
    {
        _isActive = isActive;
    }
    public bool GetIsSimulating() { return _isSimulating; }
    public UnityEvent OnSimulateChange = new();
    private void SetSimulate(bool inIsSimulating) {
        if (inIsSimulating != _isSimulating) {
            _isSimulating = inIsSimulating;
            if (OnSimulateChange != null) OnSimulateChange.Invoke();
        }
    }

    public List<TransformValues> GetCurrentSolution() { return _dynamicTransformVals; }

    public void LoadTransformValues(List<TransformValues> transformValues)
    {
        if (transformValues == null || _dynamics == null || transformValues.Count != _dynamics.Count)
        {
            Debug.LogError("Unable to load transform values!");
            return;
        }

        _dynamicTransformVals = transformValues;
    }

    void Awake()
    {
        Setup();
    }

    void Setup()
    {
        FillObjectsList();

        Debug.Assert(_levelClearCanvas != null, "_levelClearCanvas not assigned");
        Debug.Assert(_hudCanvas != null, "_hudCanvas not assigned");
        Debug.Assert(_targetObjects != null && _targetObjects.Count > 0, "_targetObjects not assigned");
        Debug.Assert(_targetAreas != null && _targetAreas.Count > 0, "_targetAreas not assigned");
        Debug.Assert(_dynamics != null && _dynamics.Count > 0, "_dynamics not assigned");

        _levelClearCanvas.SetNextLevelButton(() => {OnNextStage.Invoke();});

        _dynamicTransformVals = new List<TransformValues>(_dynamics.Count);
        _originalDynamicTransformVals = new List<TransformValues>(_dynamics.Count);
        SetSimulate(false);

        Debug.Assert(_hudCanvas.GetComponent<Hud>() != null, "_hudCanvas does not have a Hud component!");
        _hudCanvas.GetComponent<Hud>().SetStageManager(this);

        RegisterTargetAreas();
        StoreOriginalTransformValues();
        StoreTransformValues();
        Reset();
    }

    private void FillObjectsList()
    {
        string stageName = "";
        if (StageLoader.GetInstance() == null) stageName = gameObject.scene.name;
        else stageName = StageLoader._currentStageName;
        // Target Areas
        foreach (var obj in GameObject.FindGameObjectsWithTag("TargetArea"))
        {
            SceneTag sceneTag = obj.GetComponent<SceneTag>();
            if (sceneTag == null) continue;
            if (string.IsNullOrEmpty(sceneTag.sceneName)) sceneTag.sceneName = gameObject.scene.name;
            if (sceneTag.sceneName != stageName) continue;

            TargetArea objTargetArea = obj.GetComponent<TargetArea>();
            if (objTargetArea == null)
            {
                Debug.LogError("Target Area " + obj.name + " is missing TargetArea component!");
                continue;
            }
            if (!_targetAreas.Contains(objTargetArea)) _targetAreas.Add(objTargetArea);
        }
        // Target objects
        foreach (var obj in GameObject.FindGameObjectsWithTag("TargetObj"))
        {
            SceneTag sceneTag = obj.GetComponent<SceneTag>();
            if (sceneTag == null) continue;
            if (string.IsNullOrEmpty(sceneTag.sceneName)) sceneTag.sceneName = gameObject.scene.name;
            if (sceneTag.sceneName != stageName) continue;

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            TargetObject targetObj = obj.GetComponent<TargetObject>();
            if (rb == null)
            {
                Debug.LogError("Target object " + obj.name + " is missing Rigidbody2D component!");
            }
            if (targetObj == null)
            {
                Debug.LogError("Target object " + obj.name + " is missing TargetObject component!");
            }
            if (!rb || !targetObj) continue;
            if (!_dynamics.Contains(rb)) _dynamics.Add(rb);
            if (!_targetObjects.Contains(targetObj)) _targetObjects.Add(targetObj);
        }

        // Dynamics
        foreach (var obj in GameObject.FindGameObjectsWithTag("DynamicObj"))
        {
            SceneTag sceneTag = obj.GetComponent<SceneTag>();
            if (sceneTag == null) continue;
            if (string.IsNullOrEmpty(sceneTag.sceneName)) sceneTag.sceneName = gameObject.scene.name;
            if (sceneTag.sceneName != stageName) continue;

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Dynamic object " + obj.name + " is missing Rigidbody2D component!");
                continue;
            }
            if (_dynamics.Contains(rb)) continue;
            _dynamics.Add(rb);
        }
    }

    public void CheckStageClear()
    {
        _score = 0;

        for (int i = 0; i < _targetAreas.Count; i++)
        {
            TargetArea curTargetArea = _targetAreas[i];
            if (curTargetArea.TargetType == TargetArea.eTargetType.Optional)
            {
                _score += 100;
            }
            else
            {
                if (curTargetArea.ReachedTarget) {
                    // TODO: Add score based on timer?
                    continue;
                }
                else return;
            }
        }
        _score += 500; // Base level clear score of 500

        CompleteLevel();
    }

    [ContextMenu("Complete level")]
    public void CompleteLevel()
    {
        int levelIndex = Constants.LEVEL_INDEX_FROM_NAME(gameObject.scene.name);
        Debug.Assert(levelIndex >= 0, "Level Index not found!");
        int[] scores = SaveUtils.LoadScores();
        int oldHighscore = scores[levelIndex];
        if (_score > oldHighscore) {
            scores[levelIndex] = _score;
            SaveUtils.SaveScores(scores);
        }

        ShowLevelClearText(oldHighscore);
        OnStageClear.Invoke();
    }

    public void ShowLevelClearText(int inHighscore)
    {
        _levelClearCanvas.gameObject.SetActive(true);
        _levelClearCanvas.SetScore(_score, inHighscore);
    }

    public void ResetUIOnly()
    {
        _levelClearCanvas.gameObject.SetActive(false);
        HandlerManager.Instance.ResumeTransformations();
    }
    public void ToggleHUD(bool inIsVisible) { _hudCanvas.SetActive(inIsVisible); }
    public void Reset()
    {
        SetSimulate(false);
        ResetUIOnly();

        for (int i = 0; i < _dynamics.Count; i++)
        {
            Rigidbody2D curDynamic = _dynamics[i];
            curDynamic.isKinematic = true;
            curDynamic.velocity = Vector2.zero;
            curDynamic.angularVelocity = 0.0f;
            SetTransformToValues(curDynamic.transform, _dynamicTransformVals[i]);
            curDynamic.gameObject.SetActive(true);
        }

        for (int i = 0; i < _targetObjects.Count; i++)
        {
            _targetObjects[i].Reset();
        }

        for (int i = 0; i < _targetAreas.Count; i++)
        {
            _targetAreas[i].ResetTarget();
        }

        _score = 0;
    }

    public void ResetStage()
    {
        SetSimulate(false);
        ResetUIOnly();

        for (int i = 0; i < _dynamics.Count; i++)
        {
            Rigidbody2D curDynamic = _dynamics[i];
            curDynamic.isKinematic = true;
            curDynamic.velocity = Vector2.zero;
            curDynamic.angularVelocity = 0.0f;
            SetTransformToValues(curDynamic.transform, _originalDynamicTransformVals[i]);
            curDynamic.gameObject.SetActive(true);
        }

        for (int i = 0; i < _targetObjects.Count; i++)
        {
            _targetObjects[i].Reset();
        }

        for (int i = 0; i < _targetAreas.Count; i++)
        {
            _targetAreas[i].ResetTarget();
        }

    }

    public void ResumePhysics()
    {
        if (_isSimulating) return;
        SetSimulate(true);
        HandlerManager.Instance.PauseTransformations();
        StoreTransformValues();

        for (int i = 0; i < _dynamics.Count; i++)
        {
            _dynamics[i].isKinematic = false;
        }
    }

    private void RegisterTargetAreas()
    {
        foreach (var targetArea in _targetAreas)
        {
            targetArea.OnGoalReached.AddListener(CheckStageClear);
        }
    }

    private void StoreTransformValues()
    {
        _dynamicTransformVals.Clear();
        for (int i = 0; i < _dynamics.Count; i++)
        {
            _dynamicTransformVals.Add(new TransformValues(_dynamics[i].transform));
        }
    }

    private void StoreOriginalTransformValues()
    {
        _originalDynamicTransformVals.Clear();
        for (int i = 0; i < _dynamics.Count; i++)
        {
            _originalDynamicTransformVals.Add(new TransformValues(_dynamics[i].transform));
        }
    }

    private void Update()
    {
        if (_isActive && Input.GetKeyDown(KeyCode.Space))
        {
            if (!_isSimulating) ResumePhysics();
            else Reset();
        }
    }

    public void Play()
    {
        if (_isActive && !_isSimulating)
        {
            ResumePhysics();
        }
        else
        {
            Reset();
        }
    }

    private void SetTransformToValues(Transform transform, TransformValues values)
    {
        transform.position = values.position;
        transform.rotation = values.rotation;
        transform.localScale = values.scale;
    }
}

public struct TransformValues
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public TransformValues(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.lossyScale;
    }
}
