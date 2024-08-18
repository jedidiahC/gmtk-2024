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

    private SpriteRenderer _spriteRenderer;
    private Color _spriteColor;

    private bool _isMouseOver = false;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(_spriteRenderer != null, "_spriteRenderer is not found!");
        Debug.Assert(gameObject.GetComponent<Collider2D>() != null, "Collider2D is not found!");
        _spriteColor = _spriteRenderer.color;
    }

    private Vector3 _lastMousePos;
    private bool _mouseWasDown = false;
    private void OnMouseDown()
    {
        _lastMousePos = Input.mousePosition;

        if (!_mouseWasDown)
        {
            OnStartInteract.Invoke();
        }

        _mouseWasDown = true;
    }

    private void OnMouseUp()
    {
        if (_mouseWasDown)
        {
            OnStopInteract.Invoke();
            _mouseWasDown = false;
            if (!HandlerManager.Instance.AllowTransformations) return;

            if (HandlerManager.Instance.GetTarget() == transform)
            {
                const float TOLERANCE = 1.0f;
                if (Vector3.Distance(_lastMousePos, Input.mousePosition) < TOLERANCE)
                {
                    HandlerManager.Instance.SetTarget(null);
                }
            }
            else HandlerManager.Instance.SetTarget(transform);
        }
    }

    private void OnMouseEnter()
    {
        if (!HandlerManager.Instance.AllowTransformations) return;
        _spriteRenderer.color = Color.magenta;

        if (!_isMouseOver)
        {
            _isMouseOver = true;
            OnEnter.Invoke();
        }
    }

    private void OnMouseExit()
    {
        if (!HandlerManager.Instance.AllowTransformations) return;
        _spriteRenderer.color = _spriteColor;

        if (_isMouseOver)
        {
            _isMouseOver = false;
            OnExit.Invoke();
        }
    }
}
