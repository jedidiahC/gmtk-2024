using UnityEngine;

public class DebugDrawFunctions : MonoBehaviour
{
    private bool _mouseDowned;
    private Vector3 _savedMousePosition;
    private float _lineLength;

    private void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!_mouseDowned)
        {
            _savedMousePosition = mouseWorldPosition;
            _lineLength = Vector3.Distance(transform.position, mouseWorldPosition);
        }
        else
        {
            Debug.DrawLine(transform.position, mouseWorldPosition);
        }
        Debug.DrawLine(transform.position, _savedMousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            _mouseDowned = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _mouseDowned = false;
        }
        Debug.DrawLine(_savedMousePosition, mouseWorldPosition);
    }
}
