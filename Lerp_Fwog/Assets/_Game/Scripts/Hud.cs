using System;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    [SerializeField] private Button _onReset = null;
    [SerializeField] private Button _onPlay = null;
    [SerializeField] private Button _onSelectTranslate = null;
    [SerializeField] private Button _onSelectRotate = null;
    [SerializeField] private Button _onSelectScale = null;
    [SerializeField] private UIToolbarFrame _toolbar = null;

    private TransformConstraints _currentContraints;
    private eTransformType _currentActiveType;
    private StageManager _stage;
    public void SetStageManager(StageManager inStage) { _stage = inStage; }

    private void Awake()
    {
        Debug.Assert(_onPlay != null, "_onPlay is not assigned!");
        Debug.Assert(_onSelectTranslate != null, "_onSelectTranslate is not assigned!");
        Debug.Assert(_onSelectRotate != null, "_onSelectRotate is not assigned!");
        Debug.Assert(_onSelectScale != null, "_onSelectScale is not assigned!");
        Debug.Assert(_toolbar != null, "_toolbar is not assigned!");
        // _toolbar.ToggleTransformInUse(eTransformType.Rotation);
        _toolbar.SetAllInUseToFalse();
    }

    private void Start()
    {
        HandlerManager.Instance.OnSetTarget.AddListener(OnSetTarget);
        HandlerManager.Instance.OnSwitchMode.AddListener(OnSwitchMode);
        _onReset.onClick.AddListener(OnReset);
        _onPlay.onClick.AddListener(OnPlay);
        _onSelectTranslate.onClick.AddListener(OnSelectTranslate);
        _onSelectRotate.onClick.AddListener(OnSelectRotate);
        _onSelectScale.onClick.AddListener(OnSelectScale);

        Debug.Assert(_stage != null, "_stage is not assigned yet by StageManager!");
    }

    // TODO: Make this into event.
    private void LateUpdate() {
        if (_stage.GetIsSimulating()) {
            _onPlay.interactable = false;
            _toolbar.TogglePlayInteractable(false);
            _onReset.interactable = true;
            _toolbar.ToggleResetInteractable(true);
        } else {
            _onPlay.interactable = true;
            _toolbar.TogglePlayInteractable(true);
            _onReset.interactable = false;
            _toolbar.ToggleResetInteractable(false);
        }
    }

    public void OnSetTarget()
    {
        if (HandlerManager.Instance.GetTarget() == null) {
            _toolbar.SetAllInUseToFalse();
            _toolbar.ToggleTransformControls(false, false, false);
            return;
        }
        
        _currentContraints = HandlerManager.Instance.GetConstraints();
        _toolbar.ToggleTransformControls(_currentContraints.AllowTranslation, _currentContraints.AllowRotation, _currentContraints.AllowScaling);

        // This is really horrible and it makes me want to cry
        if (_currentActiveType == eTransformType.Translation && !_currentContraints.AllowTranslation)
        {
            Debug.Log("doesn't allow translation, changing to rotation");
            HandlerManager.Instance.SwitchMode(eTransformType.Rotation);
        }
        if (_currentActiveType == eTransformType.Rotation && !_currentContraints.AllowRotation)
        {
            Debug.Log("doesn't allow rotation, changing to scaling");
            HandlerManager.Instance.SwitchMode(eTransformType.Scale);
        }
        if (_currentActiveType == eTransformType.Scale && !_currentContraints.AllowScaling)
        {
            Debug.Log("doesn't allow scaling, changing to translate");
            HandlerManager.Instance.SwitchMode(eTransformType.Translation);
        }

        if (_currentActiveType != HandlerManager.Instance.GetTransformType()) {
            // Rayner: Note - I think should switch to the last used transform type when possible?
            HandlerManager.Instance.SwitchMode(_currentActiveType);
        }

        _toolbar.ToggleTransformInUse(_currentActiveType);
    }

    public void OnReset() {
        _stage.Reset();
    }

    public void OnPlay() {
        _stage.ResumePhysics();
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
        _currentActiveType = transformType;
        _toolbar.ToggleTransformInUse(transformType);
    }
}
