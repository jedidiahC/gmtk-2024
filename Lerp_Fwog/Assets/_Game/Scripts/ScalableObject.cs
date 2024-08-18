using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalableObject : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Color _spriteColor;

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
        _mouseWasDown = true;
    }

    private void OnMouseUp()
    {
        if (_mouseWasDown)
        {
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
    }

    private void OnMouseExit()
    {
        if (!HandlerManager.Instance.AllowTransformations) return;
        _spriteRenderer.color = _spriteColor;
    }
}
