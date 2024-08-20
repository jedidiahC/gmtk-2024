using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector3 _targetPosition;
    [SerializeField] private float _targetOrthoSize;

    private Vector3 _originPosition;
    private float _originOrthoSize;
    float t = 0;

    public void LerpToCameraProperties(Camera camera)
    {
        _camera.backgroundColor = camera.backgroundColor;
        _originPosition = transform.position;
        _originOrthoSize = camera.orthographicSize;
        _targetPosition = camera.transform.position;
        _targetOrthoSize = camera.orthographicSize;
        t = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        t += Time.deltaTime;
        transform.position = Vector3.Slerp(_originPosition, _targetPosition, t * t);
        _camera.orthographicSize = Mathf.Lerp(_originOrthoSize, _targetOrthoSize, t * t);
    }

    public void SetTargetPos(Vector3 inTargetCamPos) {
        _targetPosition = inTargetCamPos;
    }
}
