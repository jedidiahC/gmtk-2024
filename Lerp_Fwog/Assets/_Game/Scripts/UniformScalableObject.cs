using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UniformScalableObject : MonoBehaviour
{
    public UnityEvent<float> onScale = new();

    [SerializeField] private float _scaleSpeedMultiplier = 1.0f;
    [SerializeField] private float _minScaleFactor = 0.5f;
    [SerializeField] private float _maxScaleFactor = 5.0f;

    private SpriteRenderer _spriteRenderer;
    private Color _spriteColor;
    private Vector3 _originalScale;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(_spriteRenderer != null, "_spriteRenderer is not found!");
        Debug.Assert(gameObject.GetComponent<Collider2D>() != null, "Collider2D is not found!");
        _spriteColor = _spriteRenderer.color;
    }

    private void Start()
    {
        _originalScale = transform.localScale;
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

    private void OnMouseOver()
    {
        ScaleDelta(Input.mouseScrollDelta.y * _scaleSpeedMultiplier);
    }

    public void ScaleDelta(float dScale)
    {
        Vector3 newScale = transform.localScale + Vector3.one * dScale;
        float newScaleFactor = GetScaleFactor(newScale);
        Debug.Log(newScaleFactor);
        if (newScaleFactor < _minScaleFactor || newScaleFactor > _maxScaleFactor)
        {
            return;
        }
        transform.localScale = newScale;
        onScale.Invoke(GetCurrScaleFactor());
    }

    private float GetCurrScaleFactor()
    {
        return GetScaleFactor(transform.localScale);
    }

    private float GetScaleFactor(Vector3 scale)
    {
        return scale.x / _originalScale.x;
    }
}
