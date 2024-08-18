using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class StageManager : MonoBehaviour
{
    public UnityEvent OnStageClear = new();

    [SerializeField] private TextMeshProUGUI _levelClearText = null;
    [SerializeField] private List<TargetArea> _targetAreas = null;
    [SerializeField] private List<Rigidbody2D> _dynamics = null;
    [SerializeField] private Camera _camera = null;

    public Camera GetStageCamera() { return _camera; }

    private List<TransformValues> _dynamicTransformVals = null;

    private bool _isSimulating = false;
    private bool _isActive = false;

    void Awake()
    {
        Setup();
    }

    void Setup()
    {
        Debug.Assert(_levelClearText != null, "_levelClearText not assigned");
        Debug.Assert(_targetAreas != null && _targetAreas.Count > 0, "_targetAreas not assigned");
        Debug.Assert(_dynamics != null && _dynamics.Count > 0, "_dynamics not assigned");
        _dynamicTransformVals = new List<TransformValues>(_dynamics.Count);
        _isSimulating = false;

        RegisterTargetAreas();
        StoreTransformValues();
        Reset();
    }

    public void CheckStageClear()
    {
        for (int i = 0; i < _targetAreas.Count; i++)
        {
            if (_targetAreas[i].ReachedTarget) continue;
            else return;
        }

        CompleteLevel();
    }

    [ContextMenu("Complete level")]
    public void CompleteLevel()
    {
        ShowLevelClearText();
        OnStageClear.Invoke();
    }

    public void ShowLevelClearText()
    {
        _levelClearText.enabled = true;
    }

    public void Reset()
    {
        _isSimulating = false;
        _levelClearText.enabled = false;

        for (int i = 0; i < _dynamics.Count; i++)
        {
            Rigidbody2D curDynamic = _dynamics[i];
            curDynamic.isKinematic = true;
            curDynamic.velocity = Vector2.zero;
            curDynamic.angularVelocity = 0.0f;
            SetTransformToValues(curDynamic.transform, _dynamicTransformVals[i]);
            curDynamic.gameObject.SetActive(true);
        }

        for (int i = 0; i < _targetAreas.Count; i++)
        {
            _targetAreas[i].ResetTarget();
        }

        HandlerManager.Instance.ResumeTransformations();
    }

    public void ResumePhysics()
    {
        if (_isSimulating) return;
        _isSimulating = true;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_isSimulating) ResumePhysics();
            else Reset();
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
