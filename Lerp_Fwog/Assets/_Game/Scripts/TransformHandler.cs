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
    private TransformConstraints _targetConstraints;

    public void SetTarget(Transform inTarget, TransformConstraints constraints = new TransformConstraints())
    {
        _target = inTarget;
        _targetConstraints = constraints;
        SetSprite();
    }

    public Transform target { get { return _target; } }
    private eTransformType _transformType = eTransformType.Scale;
    public eTransformType transformType { get { return _transformType; } }
    public TransformConstraints constraints { get { return _targetConstraints; } }

    public void SetTransformType(eTransformType inTransformType)
    {
        _transformType = inTransformType;
        SetSprite();
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

    void Awake()
    {
        _spriteRen = GetComponent<SpriteRenderer>();
        Debug.Assert(_spriteRen != null, "_spriteRen is not found!");
        Debug.Assert(_scaleSprite != null, "_scaleSprite is not assigned");
        Debug.Assert(_rotateSprite != null, "_rotateSprite is not assigned");
        Debug.Assert(_translateSprite != null, "_translateSprite is not assigned");

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

    private void HandleScaleInput()
    {
        if (!_targetConstraints.AllowScaling) { return; }

        Vector3 targetLocalScale = _target.localScale;
        float delta = 1.5f * Time.deltaTime;

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


    private void HandleRotationInput()
    {
        if (!_targetConstraints.AllowRotation) { return; }

        Vector3 targetLocalEuler = _target.localEulerAngles;
        float delta = 36.0f * Time.deltaTime;
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

        _target.localEulerAngles = targetLocalEuler;
    }

    private void HandleTranslateInput()
    {
        if (!_targetConstraints.AllowTranslation) { return; }

        Vector3 targetPosition = _target.position;
        float delta = 1.5f * Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
        {
            targetPosition.y += delta;
        }
        if (Input.GetKey(KeyCode.S))
        {
            targetPosition.y -= delta;
        }
        if (Input.GetKey(KeyCode.D))
        {
            targetPosition.x += delta;
        }
        if (Input.GetKey(KeyCode.A))
        {
            targetPosition.x -= delta;
        }

        Vector3 originalPos = _targetConstraints.OriginalTransform.position;

        if (targetPosition.x < originalPos.x - _targetConstraints.MinTranslationOffset.x || targetPosition.x > originalPos.x + _targetConstraints.MaxTranslationOffset.x ||
            targetPosition.y < originalPos.y - _targetConstraints.MinTranslationOffset.y || targetPosition.y > originalPos.y + _targetConstraints.MaxTranslationOffset.y ||
            targetPosition.z < originalPos.z - _targetConstraints.MinTranslationOffset.z || targetPosition.z > originalPos.z + _targetConstraints.MaxTranslationOffset.z)
        {
            return;
        }

        _target.position = targetPosition;
    }
}
