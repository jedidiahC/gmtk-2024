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
        _toolbar.ToggleTransformInUse(eTransformType.Rotation);
    }

    private void Start()
    {
        HandlerManager.Instance.OnSetTarget.AddListener(OnSetTarget);
        HandlerManager.Instance.OnSwitchMode.AddListener(OnSwitchMode);
        _onSelectTranslate.onClick.AddListener(OnSelectTranslate);
        _onSelectRotate.onClick.AddListener(OnSelectRotate);
        _onSelectScale.onClick.AddListener(OnSelectScale);
    }

    public void OnSetTarget()
    {
        TransformConstraints constraints = HandlerManager.Instance.GetConstraints();
        _toolbar.ToggleTransformControls(constraints.AllowTranslation, constraints.AllowRotation, constraints.AllowScaling);
    }

    public void OnSelectTranslate()
    {
        HandlerManager.Instance.SwitchMode(eTransformType.Translation);
    }

    public void OnSelectRotate()
    {
        HandlerManager.Instance.SwitchMode(eTransformType.Rotation);
    }

    public void OnSelectScale()
    {
        HandlerManager.Instance.SwitchMode(eTransformType.Scale);
    }

    private void OnSwitchMode(eTransformType transformType)
    {
        Debug.Log("Switched to " + transformType);
        _toolbar.ToggleTransformInUse(transformType);
    }
}
