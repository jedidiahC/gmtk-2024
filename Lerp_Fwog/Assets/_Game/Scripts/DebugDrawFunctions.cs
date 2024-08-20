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
    private float _currentScale, _velocityScale, _targetScale;
    private float _dampingRatio = 0.5f, _angularFrequency = 0.1f, _timeStep = 1.0f;

    private float _angleRef;
    private float _angleResult;

    public bool _doAngleShit;
    public bool _doScaleShit;

    private float fistance;
    private Vector3 fistanceV;

    private float ssssscale;

    private void Start()
    {
        _angleText.gameObject.SetActive(_doAngleShit || _doScaleShit);
        _targetScale = 1.0f;
        ssssscale = transform.localScale.x;
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0.0f;

        if (Input.GetMouseButtonDown(0))
        {
            _mouseDowned = true;
            _savedMousePosition = mouseWorldPosition;
            ssssscale = transform.localScale.x;
            _currentScale = ssssscale;
        }
        if (Input.GetMouseButton(0))
        {
            _angleText.text = fistance.ToString();
            _targetScale = fistance;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _mouseDowned = false;
        }

        if (_doScaleShit)
        {
            fistance = Vector3.Distance(_savedMousePosition, mouseWorldPosition) * Mathf.Clamp(mouseWorldPosition.x - _savedMousePosition.x, -1, 1);
            // fistance = Mathf.Max(0.0f, fistance * 0.5f);
            SpringMath.Lerp(ref _currentScale, ref _velocityScale, _targetScale, _dampingRatio, _angularFrequency, _timeStep);
            transform.localScale = new Vector3(ssssscale + _targetScale, ssssscale + _targetScale, 1.0f);
            return;
        }

        if (_doAngleShit)
        {
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
