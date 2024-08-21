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
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClipGroup _resetSound = null;
    [SerializeField] private AudioClipGroup _playSound = null;
    [SerializeField] private AudioClipGroup _pauseSound = null;
    [SerializeField] private AudioClipGroup _changeWidget = null;

    private TransformConstraints _currentContraints;
    private eTransformType _lastActiveTransformType;
    private StageManager _stage;

    public void SetStageManager(StageManager inStage)
    {
        if (_stage != null)
        {
            _stage.OnSimulateChange.RemoveListener(OnSimulateChange);
        }
        _stage = inStage;
        _stage.OnSimulateChange.AddListener(OnSimulateChange);
        OnSimulateChange();
    }

    private void Awake()
    {
        Debug.Assert(_onPlay != null, "_onPlay is not assigned!");
        Debug.Assert(_onSelectTranslate != null, "_onSelectTranslate is not assigned!");
        Debug.Assert(_onSelectRotate != null, "_onSelectRotate is not assigned!");
        Debug.Assert(_onSelectScale != null, "_onSelectScale is not assigned!");
        Debug.Assert(_toolbar != null, "_toolbar is not assigned!");
        Debug.Assert(_audioSource != null, "_audioSource is not assigned!");
        Debug.Assert(_resetSound != null, "_resetSound is not assigned!");
        Debug.Assert(_changeWidget != null, "_changeWidget is not assigned!");
        Debug.Assert(_playSound != null, "_playSound is not assigned!");
        Debug.Assert(_pauseSound != null, "_pauseSound is not assigned!");

        _toolbar.SetAllInUseToFalse();
    }

    private void Start()
    {
        HandlerManager.Instance.OnSetTarget.AddListener(OnSetTarget);
        HandlerManager.Instance.OnSwitchMode.AddListener(OnSwitchMode);

        _onPlay.onClick.RemoveAllListeners();
        _onReset.onClick.RemoveAllListeners();

        _onReset.onClick.AddListener(OnReset);
        _onPlay.onClick.AddListener(OnPlay);
        _onSelectTranslate.onClick.AddListener(OnSelectTranslate);
        _onSelectRotate.onClick.AddListener(OnSelectRotate);
        _onSelectScale.onClick.AddListener(OnSelectScale);

        Debug.Assert(_stage != null, "_stage is not assigned yet by StageManager!");
    }

    public void OnSimulateChange()
    {
        if (_stage.GetIsSimulating())
        {
            _playSound.PlayOneShot(_audioSource);
        }
        else
        {
            _pauseSound.PlayOneShot(_audioSource);
        }

        SetPlayResetInteractable(!_stage.GetIsSimulating());
    }

    public void OnSetTarget()
    {
        if (!IsTargetSelected())
        {
            _toolbar.SetAllInUseToFalse();
            _toolbar.ToggleTransformControls(false, false, false);
            return;
        }

        UpdateTransformType();

        _toolbar.ToggleTransformInUse(_lastActiveTransformType);
        _toolbar.ToggleTransformControls(_currentContraints.AllowTranslation, _currentContraints.AllowRotation, _currentContraints.AllowScaling);
    }

    public void OnReset()
    {
        _resetSound.PlayOneShot(_audioSource);
        _stage.ResetStage();
    }

    public void OnPlay()
    {
        _stage.Play();
    }

    public void OnSelectTranslate()
    {

        SwitchMode(eTransformType.Translation);
    }

    public void OnSelectRotate()
    {
        SwitchMode(eTransformType.Rotation);
    }

    public void OnSelectScale()
    {
        SwitchMode(eTransformType.Scale);
    }

    private void UpdateTransformType()
    {
        _currentContraints = HandlerManager.Instance.GetConstraints();

        // Try to switch to the previously selected transform type if it's allowed by the new selected target.
        if (_currentContraints.IsTransformTypeAllowed(_lastActiveTransformType))
        {
            SwitchMode(_lastActiveTransformType);
        }
        else
        {
            // Switch to the first allowed transform type in this order.
            eTransformType[] availableTransformTypePriority = new eTransformType[] { eTransformType.Translation, eTransformType.Rotation, eTransformType.Scale };

            foreach (eTransformType transformType in availableTransformTypePriority)
            {
                if (_currentContraints.IsTransformTypeAllowed(transformType))
                {
                    SwitchMode(transformType);
                    break;
                }
            }

            // If no transform types allowed, too bad!
        }
    }

    private void SetPlayResetInteractable(bool canInteract)
    {
        _toolbar.TogglePlayInteractable(canInteract, false);
        _toolbar.ToggleResetInteractable(canInteract);

        _onReset.interactable = canInteract;
    }

    private void SwitchMode(eTransformType transformType)
    {
        if (!IsTargetSelected()) { return; }

        HandlerManager.Instance.SwitchMode(transformType);
    }

    private bool IsTargetSelected()
    {
        return HandlerManager.Instance != null && HandlerManager.Instance.GetTarget() != null;
    }

    private void OnSwitchMode(eTransformType transformType)
    {
        if (!IsTargetSelected()) { return; }

        _lastActiveTransformType = transformType;

        _toolbar.ToggleTransformInUse(transformType);
        _changeWidget.PlayOneShot(_audioSource);
    }
}
