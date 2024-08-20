using System.Collections;
using System.Collections.Generic;
using RuntimeHandle;
using UnityEngine;
using UnityEngine.Events;

public class HandlerManager : MonoBehaviour
{
    private static HandlerManager _instance = null;

    public static HandlerManager Instance { get { return _instance; } }

    public UnityEvent<eTransformType> OnSwitchMode = new();
    public UnityEvent OnSetTarget = new();

    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClipGroup _selectSound = null;

    private bool _allowTransformations = true;

    public bool AllowTransformations { get { return _allowTransformations; } }
    public void ResumeTransformations() { _allowTransformations = true; }
    public eTransformType GetTransformType() { return _transformHandler.transformType; }

    public void PauseTransformations()
    {
        SetTarget(null);
        _allowTransformations = false;
        if (OnPauseTransformations != null) OnPauseTransformations.Invoke();
    }

    public UnityEvent OnPauseTransformations = new();

    [SerializeField] private TransformHandler _transformHandler = null;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Setup();
        }
        else if (_instance != this) DestroyImmediate(gameObject);
    }

    void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    void Setup()
    {
        Debug.Assert(_transformHandler != null, "_transformHandler not assigned");
        Debug.Assert(_audioSource != null, "_audioSource is not assigned!");
        Debug.Assert(_selectSound != null, "_selectSound is not assigned!");
        SetTarget(null);
        PauseTransformations();

        DontDestroyOnLoad(gameObject);
    }

    public void SetTarget(Transform inTarget, TransformConstraints transformConstraints = new TransformConstraints())
    {
        if (!_allowTransformations) return;

        if (inTarget != null)
        {
            _selectSound.PlayOneShot(_audioSource);
        }

        _transformHandler.SetTarget(inTarget, transformConstraints);
        OnSetTarget.Invoke();
    }

    public Transform GetTarget() { return _transformHandler.target; }
    public TransformConstraints GetConstraints() { return _transformHandler.constraints; }

    public void SwitchMode(eTransformType inTransformType)
    {
        //Debug.Log("Switch mode " + inTransformType);
        _transformHandler.SetTransformType(inTransformType);
        OnSwitchMode.Invoke(inTransformType);
    }

    private void Update()
    {
        if (!_allowTransformations) return;

        if (Input.GetKeyDown(KeyCode.Q)) SetTarget(null);
        else if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchMode(eTransformType.Translation);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchMode(eTransformType.Scale);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchMode(eTransformType.Rotation);
    }
}
