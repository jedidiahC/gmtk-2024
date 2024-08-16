using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeighingScalesManager : MonoBehaviour {
    private static WeighingScalesManager _instance = null;
    public static WeighingScalesManager Instance { get { return _instance; } }

    [SerializeField] private TextMeshProUGUI _levelClearText = null;
    [SerializeField] private Rigidbody2D _launchObjectRB = null;
    [SerializeField] private Rigidbody2D _dropObjectRB = null;
    [SerializeField] private Rigidbody2D _plankRB = null;

    private TransformValues _launchObjTransformVals = new TransformValues();
    private TransformValues _dropObjTransformVals = new TransformValues();
    private TransformValues _plankTransformVals = new TransformValues();

    void Awake() {
        if (_instance == null) {
            _instance = this;
            Setup();
        } else if (_instance != this) DestroyImmediate(gameObject);
    }
    private void OnDestroy() { if (_instance == this) _instance = null; }

    void Setup() {
        Debug.Assert( _levelClearText != null, "_levelClearText not assigned");
        Debug.Assert( _launchObjectRB != null, "_launchObjectRB not assigned");
        Debug.Assert( _dropObjectRB != null, "_dropObjectRB not assigned");
        Debug.Assert(_plankRB != null, "_plankRB not assigned");

        _levelClearText.enabled = false;
        _launchObjectRB.isKinematic = true;
        _dropObjectRB.isKinematic = true;
        _plankRB.isKinematic = true;

        _launchObjTransformVals.position = _launchObjectRB.transform.position;
        _launchObjTransformVals.rotation = _launchObjectRB.transform.rotation;
        _launchObjTransformVals.scale = _launchObjectRB.transform.lossyScale;

        _dropObjTransformVals.position = _dropObjectRB.transform.position;
        _dropObjTransformVals.rotation = _dropObjectRB.transform.rotation;
        _dropObjTransformVals.scale = _dropObjectRB.transform.lossyScale;

        _plankTransformVals.position = _plankRB.transform.position;
        _plankTransformVals.rotation = _plankRB.transform.rotation;
        _plankTransformVals.scale = _plankRB.transform.lossyScale;
    }

    public void ShowLevelClearText() {
        _levelClearText.enabled = true;
    }

    public void Reset() {
        _levelClearText.enabled = false;

        _launchObjectRB.isKinematic = true;
        _launchObjectRB.velocity = Vector2.zero;
        _launchObjectRB.angularVelocity = 0.0f;
        _dropObjectRB.isKinematic = true;
        _dropObjectRB.velocity = Vector2.zero;
        _dropObjectRB.angularVelocity = 0.0f;
        _plankRB.isKinematic = true;
        _plankRB.velocity = Vector2.zero;
        _plankRB.angularVelocity = 0.0f;

        _launchObjectRB.transform.position = _launchObjTransformVals.position;
        _launchObjectRB.transform.rotation = _launchObjTransformVals.rotation;
        _launchObjectRB.transform.localScale = _launchObjTransformVals.scale;

        _dropObjectRB.transform.position = _dropObjTransformVals.position;
        _dropObjectRB.transform.rotation = _dropObjTransformVals.rotation;
        _dropObjectRB.transform.localScale = _dropObjTransformVals.scale;

        _plankRB.transform.position = _plankTransformVals.position;
        _plankRB.transform.rotation = _plankTransformVals.rotation;
        _plankRB.transform.localScale = _plankTransformVals.scale;
    }

    public void ResumePhysics() {
        _launchObjectRB.isKinematic = false;
        _dropObjectRB.isKinematic = false;
        _plankRB.isKinematic = false;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) ResumePhysics();
        if (Input.GetKeyDown(KeyCode.R)) Reset();

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _dropObjectRB.transform.localScale *= 1.1f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            _dropObjectRB.transform.localScale *= 0.9f;
        }
    }
}

public struct TransformValues {
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}
