using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Vector3 _targetPosition;

    private Vector3 _origin;
    float t = 0;

    public void SetTargetPosition(Vector3 targetPosition)
    {
        _origin = transform.position;
        _targetPosition = targetPosition;
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        Debug.Log(t);
        transform.position = Vector3.Slerp(_origin, _targetPosition, t);
    }
}
