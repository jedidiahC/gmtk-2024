using System;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DebugDrawFunctions : MonoBehaviour
{
    private bool _mouseDowned;
    private Vector3 _savedMousePosition;
    private float _lineLength;
    [SerializeField] private TMP_Text _angleText;

    private float _currentAngle, _velocityAngle, _targetAngle;
    private float _dampingRatio = 0.5f, _angularFrequency = 0.1f, _timeStep = 1.0f;

    private float _angleRef;
    private float _angleResult;

    public bool _doAngleShit;
    public bool _doScaleShit;

    private void Start()
    {
        _angleText.gameObject.SetActive(_doAngleShit);
    }

    private void Update()
    {
        if (_doAngleShit)
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

            Vector3 v1 = _savedMousePosition - transform.position;
            Vector3 v2 = mouseWorldPosition - transform.position;
            v1.z = 0;
            v2.z = 0;
            _angleResult = Vector3.SignedAngle(v1, v2, Vector3.forward);
            // if (_angleResult < 0) { _angleResult += 360.0f; }
            _angleText.text = String.Format("{0}", _angleResult);

            if (Input.GetMouseButtonDown(0))
            {
                _mouseDowned = true;
                _angleText.gameObject.SetActive(true);
                _angleRef = transform.eulerAngles.z;
                _currentAngle = 0.0f;
            }
            if (Input.GetMouseButton(0))
            {
                _targetAngle = _angleResult;
            }
            if (Input.GetMouseButtonUp(0))
            {
                _mouseDowned = false;
                _angleText.gameObject.SetActive(false);
            }
            // Debug.DrawLine(_savedMousePosition, mouseWorldPosition);}

            SpringMath.Lerp(ref _currentAngle, ref _velocityAngle, _targetAngle, _dampingRatio, _angularFrequency, _timeStep);
            transform.localEulerAngles = Vector3.forward * (_angleRef + _currentAngle);
        }
    }
}
