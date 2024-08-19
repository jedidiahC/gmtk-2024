using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    [SerializeField] private Button _onPlay = null;
    [SerializeField] private Button _onSelectTranslate = null;
    [SerializeField] private Button _onSelectRotate = null;
    [SerializeField] private Button _onSelectScale = null;
    [SerializeField] private UIToolbarFrame _toolbar = null;

    private void Awake()
    {
        Debug.Assert(_onPlay != null, "_onPlay is not assigned!");
        Debug.Assert(_onSelectTranslate != null, "_onSelectTranslate is not assigned!");
        Debug.Assert(_onSelectRotate != null, "_onSelectRotate is not assigned!");
        Debug.Assert(_onSelectScale != null, "_onSelectScale is not assigned!");
        Debug.Assert(_toolbar != null, "_toolbar is not assigned!");
    }

    private void Start()
    {
        HandlerManager.OnSwitchMode.AddListener(OnSwitchMode);
        _onSelectTranslate.onClick.AddListener(() => { HandlerManager.Instance.SwitchMode(eTransformType.Translation); });
        _onSelectRotate.onClick.AddListener(() => { HandlerManager.Instance.SwitchMode(eTransformType.Rotation); });
        _onSelectScale.onClick.AddListener(() => { HandlerManager.Instance.SwitchMode(eTransformType.Scale); });
    }

    private void OnSwitchMode(eTransformType transformType)
    {
        _toolbar.ToggleTransformInUse(transformType);
    }
}
