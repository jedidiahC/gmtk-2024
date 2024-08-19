using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTransformType {
    Scale,
    Rotation,
    Translation
}

public class TransformHandler : MonoBehaviour {
    private Transform _target = null;
    public void SetTarget(Transform inTarget) {
        _target = inTarget;
        SetSprite();
    }
    public Transform target { get { return _target; } }
    private eTransformType _transformType = eTransformType.Scale;
    public eTransformType transformType { get { return _transformType; } }
    public void SetTransformType(eTransformType inTransformType) {
        _transformType = inTransformType;
        SetSprite();
    }
    private void SetSprite() {
        if (_spriteRen == null) return; // Might not be initialised yet.
        if (_target == null) {
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

    void Awake() {
        _spriteRen = GetComponent<SpriteRenderer>();
        Debug.Assert(_spriteRen != null, "_spriteRen is not found!");
        Debug.Assert(_scaleSprite != null, "_scaleSprite is not assigned");
        Debug.Assert(_rotateSprite != null, "_rotateSprite is not assigned");
        Debug.Assert(_translateSprite != null, "_translateSprite is not assigned");

        SetTarget(null);
    }

    void Update() {
        if (_target != null) {
            transform.position = _target.position;

            switch (_transformType) {
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

    private void HandleScaleInput() {
        Vector3 targetLocalScale = _target.localScale;
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            targetLocalScale.y += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            targetLocalScale.y -= 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            targetLocalScale.x += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            targetLocalScale.x -= 0.1f;
        }
        _target.localScale = targetLocalScale;
    }

    private void HandleRotationInput() {
        Vector3 targetLocalEuler = _target.localEulerAngles;
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            targetLocalEuler.z -= 10.0f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            targetLocalEuler.z += 10.0f;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            targetLocalEuler.z -= 10.0f;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            targetLocalEuler.z += 10.0f;
        }
        _target.localEulerAngles = targetLocalEuler;
    }

    private void HandleTranslateInput() {
        Vector3 targetPosition = _target.position;
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            targetPosition.y += 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            targetPosition.y -= 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            targetPosition.x += 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            targetPosition.x -= 1.0f;
        }
        _target.position = targetPosition;
    }
}
