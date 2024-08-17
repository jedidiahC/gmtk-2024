using System.Collections;
using System.Collections.Generic;
using RuntimeHandle;
using UnityEngine;

public class HandlerManager : MonoBehaviour {
    private static HandlerManager _instance = null;
    public static HandlerManager Instance { get { return _instance; } }

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
        Debug.Assert(_transformHandler.handleCamera != null, "_transformHandler.handleCamera not assigned");
        
        _transformHandler.axes = HandleAxes.XY;
        SetTarget(null);
    }

    public void SetTarget(Transform target) {
        _transformHandler.target = target;
        _transformHandler.gameObject.SetActive(target != null);
    }

    public void SwitchMode(HandleType inHandleType) {
        _transformHandler.type = inHandleType;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
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
