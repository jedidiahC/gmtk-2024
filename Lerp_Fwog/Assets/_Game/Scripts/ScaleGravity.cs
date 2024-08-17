using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleGravity : MonoBehaviour
{
    [SerializeField] private PointEffector2D _effector2d = null;

    private float _originalMagnitude;

    private void Awake()
    {
        Debug.Assert(_effector2d != null, "_effector2d is not assigned!");
    }

    private void Start()
    {
        _originalMagnitude = _effector2d.forceMagnitude;
    }

    public void SetScale(float scaleFactor)
    {
        _effector2d.forceMagnitude = _originalMagnitude * scaleFactor;
    }
}
