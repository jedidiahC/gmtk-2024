using System;
using UnityEngine;

public enum eTransformType
{
    Scale,
    Rotation,
    Translation
}

[Serializable]
public struct TransformConstraints
{
    public TransformValues OriginalTransform;
    public bool AllowScaling;
    public bool AllowTranslation;
    public bool AllowRotation;
    public bool IsUniformScaling;
    public Vector3 MinScale;
    public Vector3 MaxScale;

    // Each component represents the max value the object can be negatively offset on the related axis.
    // e.g. MinTranslationOffset[0] = 5.0
    // Means the object can only be moved a maximum of -5 units from its original position along its local X axis.
    public Vector3 MinTranslationOffset;
    public Vector3 MaxTranslationOffset;

    public TransformConstraints(TransformValues transformValues)
    {
        OriginalTransform = transformValues;
        AllowScaling = true;
        AllowTranslation = true;
        AllowRotation = true;
        IsUniformScaling = false;
        MinScale = Vector3.one * 0.5f;
        MaxScale = Vector3.one * 5.0f;

        // Each component represents the max value the object can be negatively offset on the related axis.
        // e.g. MinTranslationOffset[0] = 5.0
        // Means the object can only be moved a maximum of -5 units from its original position along its local X axis.
        MinTranslationOffset = Vector3.one * 50.0f;
        MaxTranslationOffset = Vector3.one * 50.0f;
    }
}

public class TransformHandler : MonoBehaviour
{
    private Transform _target = null;
    private ScalableObject _targetScript = null;
    private TransformConstraints _targetConstraints;
    [SerializeField] private LineRenderer _constraintLineRen = null;
    [SerializeField] private LineRenderer _gizmoLineRen = null;

    public void SetTarget(Transform inTarget, TransformConstraints constraints = new TransformConstraints())
    {
        _target = inTarget;
        if (_target)
        {
            _targetScript = _target.GetComponent<ScalableObject>();
            Debug.Assert(_targetScript, String.Format("No ScalableObject.cs on {0}!", name));
        }
        _targetConstraints = constraints;
        SetSprite();
        if (inTarget == null) {
            HideGizmoLines();
            HideConstraints();
        } else DrawConstraints();
    }

    public Transform target { get { return _target; } }
    private eTransformType _transformType = eTransformType.Scale;
    public eTransformType transformType { get { return _transformType; } }
    public TransformConstraints constraints { get { return _targetConstraints; } }

    public void SetTransformType(eTransformType inTransformType)
    {
        _transformType = inTransformType;
        SetSprite();
        HideGizmoLines();
        HideConstraints();
        DrawConstraints();
    }

    private void SetSprite()
    {
        if (_spriteRen == null) return; // Might not be initialised yet.
        if (_target == null)
        {
            _spriteRen.sprite = null;
            return;
        }

        switch (_transformType)
        {
            case eTransformType.Scale:
                _spriteRen.sprite = _scaleSprite;
                break;
            case eTransformType.Rotation:
                _spriteRen.sprite = _rotateSprite;
                break;
            case eTransformType.Translation:
                _spriteRen.sprite = _translateSprite;
                break;
        }
    }

    private SpriteRenderer _spriteRen = null;
    [SerializeField] Sprite _scaleSprite = null;
    [SerializeField] Sprite _rotateSprite = null;
    [SerializeField] Sprite _translateSprite = null;

    public float deltaModifierT = 50.0f;
    public float deltaModifierR = 50.0f;
    public float deltaModifierS = 10.0f;

    void Awake()
    {
        _spriteRen = GetComponent<SpriteRenderer>();
        Debug.Assert(_spriteRen != null, "_spriteRen is not found!");
        Debug.Assert(_scaleSprite != null, "_scaleSprite is not assigned");
        Debug.Assert(_rotateSprite != null, "_rotateSprite is not assigned");
        Debug.Assert(_translateSprite != null, "_translateSprite is not assigned");

        Debug.Assert(_constraintLineRen != null, "_constraintLineRen is not assigned");
        Debug.Assert(_gizmoLineRen != null, "_gizmoLineRen is not assigned");

        SetTarget(null);
    }

    private bool InputUp() { return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow); }
    private bool InputDown() { return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow); }
    private bool InputLeft() { return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow); }
    private bool InputRight() { return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow); }

    void Update()
    {
        if (_target != null)
        {
            transform.position = _target.position;
            HideGizmoLines();
            switch (_transformType)
            {
                case eTransformType.Scale:
                    HandleScaleInput();
                    break;
                case eTransformType.Rotation:
                    HandleRotationInput();
                    break;
                case eTransformType.Translation:
                    HandleTranslateInput();
                    break;
            }
        }
    }

        
    private void DrawConstraints() {
        _constraintLineRen.startWidth = 0.1f;
        _constraintLineRen.endWidth = 0.1f;
        switch (_transformType)
        {
            case eTransformType.Scale:
                break;
            case eTransformType.Rotation:
                break;
            case eTransformType.Translation:
                {
                    Vector3 btmLeft = _targetConstraints.OriginalTransform.position - _targetConstraints.MinTranslationOffset;
                    Vector3 topRight = _targetConstraints.OriginalTransform.position + _targetConstraints.MaxTranslationOffset;

                    _constraintLineRen.loop = true;
                    _constraintLineRen.positionCount = 4;
                    _constraintLineRen.SetPositions(new Vector3[] {
                        new Vector3(btmLeft.x, btmLeft.y, 0f),
                        new Vector3(btmLeft.x, topRight.y, 0f),
                        new Vector3(topRight.x, topRight.y, 0f),
                        new Vector3(topRight.x, btmLeft.y, 0f)
                    });

                    break;
                }
        }
    }

    private void HideConstraints() {
        _constraintLineRen.positionCount = 0;
        _constraintLineRen.SetPositions(new Vector3[0]);
    }


    


    private void HideGizmoLines() {
        _gizmoLineRen.positionCount = 0;
        _gizmoLineRen.SetPositions(new Vector3[0]);
    }

    private void HandleScaleInput()
    {
        if (!_targetConstraints.AllowScaling) { return; }

        Vector3 targetLocalScale = _target.localScale;
        float delta = deltaModifierS * Time.deltaTime;

        if (_targetConstraints.IsUniformScaling)
        {
            Vector3 scaleDelta = Vector3.one * delta;
            if (InputUp())
            {
                targetLocalScale += scaleDelta;
            }
            if (InputDown())
            {
                targetLocalScale -= scaleDelta;
            }
            if (InputRight())
            {
                targetLocalScale += scaleDelta;
            }
            if (InputLeft())
            {
                targetLocalScale -= scaleDelta;
            }
        }
        else
        {

            if (InputUp())
            {
                targetLocalScale.y += delta;
            }
            if (InputDown())
            {
                targetLocalScale.y -= delta;
            }
            if (InputRight())
            {
                targetLocalScale.x += delta;
            }
            if (InputLeft())
            {
                targetLocalScale.x -= delta;
            }
        }

        if (targetLocalScale.x < _targetConstraints.MinScale.x || targetLocalScale.x > _targetConstraints.MaxScale.x ||
            targetLocalScale.y < _targetConstraints.MinScale.y || targetLocalScale.y > _targetConstraints.MaxScale.y ||
            targetLocalScale.z < _targetConstraints.MinScale.z || targetLocalScale.z > _targetConstraints.MaxScale.z)
        {
            return;
        }

        _target.localScale = targetLocalScale;
    }


    private RotateFrameData _rotateFrameData = new RotateFrameData();
    private void HandleRotationInput()
    {
        if (!_targetConstraints.AllowRotation) { return; }

        Vector3 targetLocalEuler = _target.localEulerAngles;
        float delta = deltaModifierR * Time.deltaTime;
        if (InputUp())
        {
            targetLocalEuler.z -= delta;
        }
        if (InputDown())
        {
            targetLocalEuler.z += delta;
        }
        if (InputRight())
        {
            targetLocalEuler.z -= delta;
        }
        if (InputLeft())
        {
            targetLocalEuler.z += delta;
        }

        _gizmoLineRen.startWidth = 0.1f;
        _gizmoLineRen.endWidth = 0.1f;
        _gizmoLineRen.loop = false;
        _gizmoLineRen.positionCount = 3;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        if (_rotateFrameData.mouseDowned)
        {
            _gizmoLineRen.SetPosition(2, mouseWorldPosition);
        }
        else
        {
            _rotateFrameData.savedMousePosition = mouseWorldPosition;
            _gizmoLineRen.SetPosition(2, _rotateFrameData.savedMousePosition);
        }
        _gizmoLineRen.SetPosition(1, _target.position);
        _gizmoLineRen.SetPosition(0, _rotateFrameData.savedMousePosition);


        Vector3 v1 = _rotateFrameData.savedMousePosition - _target.position;
        Vector3 v2 = mouseWorldPosition - _target.position;
        v1.z = 0;
        v2.z = 0;
        _rotateFrameData.angleResult = Vector3.SignedAngle(v1, v2, Vector3.forward);
        // if (_angleResult < 0) { _angleResult += 360.0f; }

        if (Input.GetMouseButtonDown(0))
        {
            _rotateFrameData.mouseDowned = true;
            _rotateFrameData.angleRef = _target.eulerAngles.z;
            _rotateFrameData.currentAngle = 0.0f;
        }
        if (Input.GetMouseButton(0))
        {
            _rotateFrameData.targetAngle = _rotateFrameData.angleResult;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _rotateFrameData.mouseDowned = false;
        }

        const float DAMPING_RATIO = 0.5f;
        const float ANGULAR_FREQUENCY = 0.1f;
        const float TIME_STEP = 1.0f;
        SpringMath.Lerp(ref _rotateFrameData.currentAngle, ref _rotateFrameData.velocityAngle, _rotateFrameData.targetAngle, DAMPING_RATIO, ANGULAR_FREQUENCY, TIME_STEP);
        targetLocalEuler = Vector3.forward * (_rotateFrameData.angleRef + _rotateFrameData.currentAngle);

        _target.localEulerAngles = targetLocalEuler;
    }

    private void HandleTranslateInput()
    {
        if (!_targetConstraints.AllowTranslation) { return; }

        Vector3 targetPosition = _target.position;
        float delta = deltaModifierT * Time.deltaTime;

        if (InputUp())
        {
            targetPosition.y += delta;
        }
        if (InputDown())
        {
            targetPosition.y -= delta;
        }
        if (InputRight())
        {
            targetPosition.x += delta;
        }
        if (InputLeft())
        {
            targetPosition.x -= delta;
        }

        if (Input.GetMouseButton(0)) {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
            mousePosWorld.z = 0;
            targetPosition += mousePosWorld - _target.position;
            // Debug.Log("mousePosWorld: " + mousePosWorld + ", deltaPostion: " + (mousePosWorld - _target.position));;
        }

        Vector3 originalPos = _targetConstraints.OriginalTransform.position;

        if (targetPosition.x < originalPos.x - _targetConstraints.MinTranslationOffset.x || targetPosition.x > originalPos.x + _targetConstraints.MaxTranslationOffset.x ||
            targetPosition.y < originalPos.y - _targetConstraints.MinTranslationOffset.y || targetPosition.y > originalPos.y + _targetConstraints.MaxTranslationOffset.y ||
            targetPosition.z < originalPos.z - _targetConstraints.MinTranslationOffset.z || targetPosition.z > originalPos.z + _targetConstraints.MaxTranslationOffset.z)
        {
            return;
        }

        _targetScript.SetTargetPosition(targetPosition);
    }



    struct RotateFrameData
    {
        public bool mouseDowned;
        public Vector3 savedMousePosition;
        public float angleResult;
        public float angleRef;
        public float currentAngle, velocityAngle, targetAngle;
    }
}
