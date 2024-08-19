using System.Collections;
using System.Collections.Generic;
using RuntimeHandle;
using UnityEngine;
using UnityEngine.Events;

public class HandlerManager : MonoBehaviour {
    private static HandlerManager _instance = null;
    public static HandlerManager Instance { get { return _instance; } }

    private bool _allowTransformations = true;
    public bool AllowTransformations { get { return _allowTransformations; } }
    public void ResumeTransformations() { _allowTransformations = true; }
    public void PauseTransformations() {
        SetTarget(null);
        _allowTransformations = false;
        if (OnPauseTransformations != null) OnPauseTransformations.Invoke();
    }
    public UnityEvent OnPauseTransformations = new();

    [SerializeField] private TransformHandler _transformHandler = null;
    void Awake() {
        if (_instance == null) {
            _instance = this;
            Setup();
        } else if (_instance != this) DestroyImmediate(gameObject);
    }

    void OnDestroy() {
        if (_instance == this) _instance = null;
    }
    
    void Setup() {
        Debug.Assert(_transformHandler != null, "_transformHandler not assigned");
        SetTarget(null);
        PauseTransformations();
    }

    public void SetTarget(Transform inTarget) {
        if (!_allowTransformations) return;
        _transformHandler.SetTarget(inTarget);
    }
    public Transform GetTarget() { return _transformHandler.target; }

    public void SwitchMode(eTransformType inTransformType) {
        _transformHandler.SetTransformType(inTransformType);
    }

    private void Update() {
        if (!_allowTransformations) return;

        if (Input.GetKeyDown(KeyCode.Q)) SetTarget(null);
        else if (Input.GetKeyDown(KeyCode.W)) SwitchMode(eTransformType.Translation);
        else if (Input.GetKeyDown(KeyCode.E)) SwitchMode(eTransformType.Scale);
        else if (Input.GetKeyDown(KeyCode.R)) SwitchMode(eTransformType.Rotation);
    }
}
