using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScalableObject))]
public class ScalableObjectEyes : MonoBehaviour
{
    [SerializeField] private GameObject _eyePrefab = null;
    [SerializeField] private Transform _eyeAnchor = null;

    private ScalableObject _scalable = null;
    private EyeControls _eyes = null;

    private void Awake()
    {
        Debug.Assert(_eyePrefab != null, "_eyePrefab is not assigned!");
        Debug.Assert(_eyeAnchor != null, "_eyeAnchor is not assigned!");

        _scalable = GetComponent<ScalableObject>();
        _eyes = Instantiate(_eyePrefab).GetComponent<EyeControls>();

        Debug.Assert(_eyePrefab != null, "_eyes is not assigned!");

        _scalable.OnEnter.AddListener(() => { _eyes.triggerNormal = true; });
        _scalable.OnExit.AddListener(() => { _eyes.triggerBored = true; });
        _scalable.OnStartInteract.AddListener(() => { _eyes.triggerShook = true; });
        _scalable.OnStopInteract.AddListener(() => { _eyes.triggerNormal = true; });
    }

    private void Start()
    {
        _eyes.triggerBored = true;
    }

    private void Update()
    {
        _eyes.transform.position = _eyeAnchor.transform.position;
        _eyes.transform.rotation = _eyeAnchor.transform.rotation;
    }
}
