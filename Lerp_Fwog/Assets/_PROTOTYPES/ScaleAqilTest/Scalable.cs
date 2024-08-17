using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scalable : MonoBehaviour {
    public enum ScaleAxis { X, Y }  
    [SerializeField] public ScaleAxis scalingAxis = ScaleAxis.X;  

    private SpriteRenderer _spriteRenderer;
    private Color _spriteColor;

    void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(_spriteRenderer != null, "_spriteRenderer is not found!");
        Debug.Assert(gameObject.GetComponent<Collider2D>() != null, "Collider2D is not found!");
        _spriteColor = _spriteRenderer.color;
    }

    private Vector3 _lastMousePos; 
    private bool _mouseWasDown = false;

    private void OnMouseDown() {
        Debug.Log("Mouse Down");
        _lastMousePos = Input.mousePosition;
        _mouseWasDown = true;
    }

    private void OnMouseUp() {
        if (_mouseWasDown) {
            _mouseWasDown = false;
            if (!HandlerManager.Instance.AllowTransformations) return;

            if (HandlerManager.Instance.GetTarget() == transform) {
                const float TOLERANCE = 1.0f;
                if (Vector3.Distance(_lastMousePos, Input.mousePosition) < TOLERANCE) {
                    Debug.Log("Setting target to null");
                    HandlerManager.Instance.SetTarget(null);
                }
            }
            else 
            {   
                Debug.Log("Setting target to " + gameObject.name);
                HandlerManager.Instance.SetTarget(transform);
            }
        }
    }

    private void OnMouseEnter() {

        Debug.Log("Mouse Entered");

        if (!HandlerManager.Instance.AllowTransformations) return;
        _spriteRenderer.color = Color.magenta;
    }

    private void OnMouseExit() {

        
        Debug.Log("Mouse Exited");

        if (!HandlerManager.Instance.AllowTransformations) return;
        _spriteRenderer.color = _spriteColor;
    }

    private void Update()
    {
        if (HandlerManager.Instance.GetTarget() == transform && 
            HandlerManager.Instance.AllowTransformations &&
            HandlerManager.Instance.GetTarget() != null)
        {
            Vector3 scale = transform.localScale;

            // Scale according to the specified axis
            switch (scalingAxis)
            {
                case ScaleAxis.X:
                    scale.y = Mathf.Clamp(scale.y, 1.0f, 1.0f); // Keep y-axis scale constant
                    scale.z = Mathf.Clamp(scale.z, 1.0f, 1.0f); // Keep z-axis scale constant
                    break;
                case ScaleAxis.Y:
                    scale.x = Mathf.Clamp(scale.x, 1.0f, 1.0f); // Keep x-axis scale constant
                    scale.z = Mathf.Clamp(scale.z, 1.0f, 1.0f); // Keep z-axis scale constant
                    break;
            }

            transform.localScale = scale;
        }
    }
}
