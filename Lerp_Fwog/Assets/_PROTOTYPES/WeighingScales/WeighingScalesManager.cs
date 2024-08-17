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

        StoreTransfromValues();
        Reset();
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

        SetTransformToValues(_launchObjectRB.transform, _launchObjTransformVals);
        SetTransformToValues(_dropObjectRB.transform, _dropObjTransformVals);
        SetTransformToValues(_plankRB.transform, _plankTransformVals);

        HandlerManager.Instance.ResumeTransformations();
    }

    public void ResumePhysics() {
        HandlerManager.Instance.PauseTransformations();
        StoreTransfromValues();

        _launchObjectRB.isKinematic = false;
        _dropObjectRB.isKinematic = false;
        _plankRB.isKinematic = false;
    }

    private void StoreTransfromValues() {
        _launchObjTransformVals = new TransformValues(_launchObjectRB.transform);
        _dropObjTransformVals = new TransformValues(_dropObjectRB.transform);
        _plankTransformVals = new TransformValues(_plankRB.transform);
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

    private void SetTransformToValues(Transform transform, TransformValues values) {
        transform.position = values.position;
        transform.rotation = values.rotation;
        transform.localScale = values.scale;
    }
}

public struct TransformValues {
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public TransformValues(Transform transform) {
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.lossyScale;
    }
}
