using System.Collections;
using System.Collections.Generic;
using RuntimeHandle;
using UnityEngine;

public class HandlerManager_Indep : MonoBehaviour {
    private static HandlerManager_Indep _instance = null;
    public static HandlerManager_Indep Instance { get { return _instance; } }

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

        SetTarget(null);
        _transformHandler.handleCamera = Camera.main;
        PauseTransformations();
    }

    public void SetTarget(Transform target) {
        if (!_allowTransformations) return;
        _transformHandler.target = target;

        // Set axes based on the target's scaling axis
        if (target != null)
        {
            Debug.Log("Target Set");
            var scalable = target.GetComponent<Scalable>();
            if (scalable != null)
            {
                _transformHandler.axes = scalable.scalingAxis == Scalable.ScaleAxis.X
                    ? HandleAxes.X
                    : HandleAxes.Y;
            }
        }

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
                    break;
                case HandleType.SCALE:
                    _transformHandler.type = HandleType.POSITION;
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SetTarget(null);
        }
    }
}
