using System.Collections;
using System.Collections.Generic;
using RuntimeHandle;
using UnityEngine;

public class HandlerManager : MonoBehaviour {
    private static HandlerManager _instance = null;
    public static HandlerManager Instance { get { return _instance; } }

    private bool _allowTransformations = true;
    public bool AllowTransformations { get { return _allowTransformations; } }
    public void ResumeTransformations() { _allowTransformations = true; }
    public void PauseTransformations() {
        SetTarget(null);
        _allowTransformations = false;
    }

    [SerializeField] private RuntimeTransformHandle _transformHandler = null;
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

        _transformHandler.axes = HandleAxes.XY;
        SetTarget(null);
        _transformHandler.handleCamera = Camera.main;
        PauseTransformations();
    }

    public void SetTarget(Transform target) {
        if (!_allowTransformations) return;
        _transformHandler.target = target;
        _transformHandler.gameObject.SetActive(target != null);
    }
    public Transform GetTarget() { return _transformHandler.target; }

    public void SwitchMode(HandleType inHandleType) {
        _transformHandler.type = inHandleType;
    }

    private void Update() {
        if (!_allowTransformations) return;

        if (Input.GetKeyDown(KeyCode.Tab)) {
            switch (_transformHandler.type) {
                case HandleType.POSITION:
                    _transformHandler.type = HandleType.SCALE;
                    _transformHandler.axes = HandleAxes.XY;
                    break;
                case HandleType.SCALE:
                    _transformHandler.type = HandleType.ROTATION;
                    _transformHandler.axes = HandleAxes.Z;
                    break;
                case HandleType.ROTATION:
                    _transformHandler.type = HandleType.POSITION;
                    _transformHandler.axes = HandleAxes.XY;
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SetTarget(null);
        }
    }
}
