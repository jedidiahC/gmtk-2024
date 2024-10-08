using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ScalableObject : MonoBehaviour
{
    public UnityEvent OnEnter = new();
    public UnityEvent OnExit = new();
    public UnityEvent OnStartInteract = new();
    public UnityEvent OnStopInteract = new();

    [SerializeField] private TransformConstraints _transformConstraints = new TransformConstraints(new TransformValues());
    [SerializeField] private float _dampingRatio = 0.5f, _angularFrequency = 0.1f, _timeStep = 1.0f;
    [SerializeField] private Vector3 _springPositionCurrent, _springPositionVelocity, _springPositionTarget;
    [SerializeField] private float _springRotationCurrent, _springRotationVelocity, _springRotationTarget;

    private SpriteRenderer _spriteRenderer;
    private Color _spriteColor;
    private bool _isMouseOver = false;
    private Vector3 _originalPosition;
    private float _originalRotation;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _transformConstraints.OriginalTransform = new TransformValues(transform);

        Debug.Assert(_spriteRenderer != null, "_spriteRenderer is not found!");
        Debug.Assert(gameObject.GetComponent<Collider2D>() != null, "Collider2D is not found!");
        Debug.Assert(IsScaleWithinConstraints(), "check the scale bro make sure within the constraints " + gameObject.name);
        Debug.Assert(IsPositionWithinConstraints(), "check the position bro make sure within the constraints " + gameObject.name);

        _spriteColor = _spriteRenderer.color;

        _originalPosition = transform.position;
        _springPositionCurrent = transform.position;
        _springPositionTarget = _springPositionCurrent;

        _originalRotation = transform.localEulerAngles.z;
        _springRotationCurrent = transform.localEulerAngles.z;
        _springRotationTarget = _springRotationCurrent;
    }

    private bool IsScaleWithinConstraints()
    {
        return transform.localScale.x >= _transformConstraints.MinScale.x && transform.localScale.x <= _transformConstraints.MaxScale.x &&
        transform.localScale.y >= _transformConstraints.MinScale.y && transform.localScale.y <= _transformConstraints.MaxScale.y &&
        transform.localScale.z >= _transformConstraints.MinScale.z && transform.localScale.z <= _transformConstraints.MaxScale.z;
    }

    private bool IsPositionWithinConstraints()
    {
        Vector3 originalPosition = _transformConstraints.OriginalTransform.position;
        return transform.position.x >= originalPosition.x - _transformConstraints.MinTranslationOffset.x && transform.position.x <= originalPosition.x + _transformConstraints.MaxTranslationOffset.x &&
        transform.position.y >= originalPosition.y - _transformConstraints.MinTranslationOffset.y && transform.position.y <= originalPosition.y + _transformConstraints.MaxTranslationOffset.y &&
        transform.position.z >= originalPosition.z - _transformConstraints.MinTranslationOffset.z && transform.position.z <= originalPosition.z + _transformConstraints.MaxTranslationOffset.z;
    }

    void Start()
    {
        // RAYNER: Cause this might run BEFORE HandlerMaanager is initialized. So use Start.
        HandlerManager.Instance.OnPauseTransformations.AddListener(ExitSelectedState);
    }

    private void Update()
    {
        if (!IsInEditMode() || !IsCurrentHandlerTarget()) return;
        LerpTransform();
    }

    private void LerpTransform()
    {
        SpringMath.Lerp(ref _springPositionCurrent, ref _springPositionVelocity, _springPositionTarget, _dampingRatio, _angularFrequency, _timeStep);
        transform.position = _springPositionCurrent;

        SpringMath.Lerp(ref _springRotationCurrent, ref _springRotationVelocity, _springRotationTarget, _dampingRatio, _angularFrequency, _timeStep);
        // if (_springRotationCurrent > 360.0f) { _springRotationCurrent -= 360.0f; };
        transform.localEulerAngles = Vector3.forward * _springRotationCurrent;
    }

    void OnDestroy()
    {
        if (HandlerManager.Instance != null) HandlerManager.Instance.OnPauseTransformations.RemoveListener(ExitSelectedState);
    }

    public void ResetSpringValues()
    {
        _springPositionTarget = _originalPosition;
        _springPositionCurrent = _originalPosition;
        _springPositionVelocity = Vector3.zero;

        _springRotationTarget = _originalRotation;
        _springRotationCurrent = _originalRotation;
        _springRotationVelocity = 0.0f;
        // Debug.Log(String.Format("{0} {1} {2} {3}", _springRotationTarget, _springRotationCurrent, _springRotationVelocity, transform.localEulerAngles.z));
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        _springPositionTarget.x = targetPosition.x;
        _springPositionTarget.y = targetPosition.y;
        // _springPositionTarget.z = targetPosition.z;
    }

    public void SetCurrentRotation(float currentRotation)
    {
        _springRotationCurrent = currentRotation;
    }

    public void SetTargetRotation(float targetRotation)
    {
        _springRotationTarget = targetRotation;
    }

    private bool _mouseWasDown = false;

    private void OnMouseDown()
    {
        if (HandlerManager.Instance == null) return;
        if (!HandlerManager.Instance.AllowTransformations) return;

        if (!_mouseWasDown)
        {
            OnStartInteract.Invoke();
        }

        _mouseWasDown = true;
    }

    private void OnMouseUp()
    {
        if (!_mouseWasDown) { return; }

        OnStopInteract.Invoke();
        _mouseWasDown = false;

        if (!IsInEditMode()) { return; }

        if (!IsCurrentHandlerTarget())
        {
            SetSelfAsHandlerTarget();
        }
    }

    private void SetSelfAsHandlerTarget()
    {
        HandlerManager.Instance.SetTarget(transform, _transformConstraints);
    }

    private bool IsInEditMode()
    {
        return HandlerManager.Instance != null && HandlerManager.Instance.AllowTransformations;
    }

    private bool IsCurrentHandlerTarget()
    {
        return HandlerManager.Instance.GetTarget() == transform;
    }

    private void OnMouseEnter()
    {
        if (!IsInEditMode()) return;

        EnterSelectedState();

        if (!_isMouseOver)
        {
            _isMouseOver = true;
            OnEnter.Invoke();
        }
    }

    private void OnMouseExit()
    {
        if (HandlerManager.Instance == null) return;
        if (!HandlerManager.Instance.AllowTransformations) return;
        ExitSelectedState();
    }

    private void EnterSelectedState()
    {
        _spriteRenderer.color = Color.Lerp(_spriteColor, new Color(1.0f, 0.0f, 1.0f), 0.25f);
    }

    private void ExitSelectedState()
    {
        _spriteRenderer.color = _spriteColor;
        if (_isMouseOver)
        {
            _isMouseOver = false;
            OnExit.Invoke();
        }
    }
}
