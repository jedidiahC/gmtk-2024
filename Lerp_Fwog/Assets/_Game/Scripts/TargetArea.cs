using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEditor.Animations;

public class TargetArea : MonoBehaviour
{
    public enum eTargetType
    {
        Mandatory,
        Optional
    }

    public UnityEvent OnGoalReached = new();

    [SerializeField] private eTargetType _targetType = eTargetType.Mandatory;
    public eTargetType TargetType { get { return _targetType; } }
    [SerializeField] private int _targetGoalNum = 0;
    [SerializeField] private RectTransform _canvasRectTransform = null;
    [SerializeField] private TextMeshProUGUI _countLeftText = null;
    [SerializeField] private SpriteRenderer _circleSpriteRen = null;
    [SerializeField] private Color _goalReachedColor = Color.green;
    [SerializeField] private ParticleSystem _flashParticleSystem = null;
    [SerializeField] private ParticleSystem _confettiParticleSystem = null;
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClipGroup _eatSound = null;
    private Color _circleInitialColor;
    private int _targetCount = 0;
    private bool _reachedTarget = false;
    public bool ReachedTarget { get { return _reachedTarget; } }

    [SerializeField] Animator _animator;


    void Awake()
    {
        Debug.Assert(_targetGoalNum > 0, "_targetGoalNum must be > 0");
        Debug.Assert(_canvasRectTransform != null, "_canvasRectTransform is not assigned");
        Debug.Assert(_countLeftText != null, "_countLeftText is not assigned");
        Debug.Assert(_circleSpriteRen != null, "_circleSpriteRen is not assigned");
        Debug.Assert(_flashParticleSystem != null, "_flashParticleSystem is not assigned");
        Debug.Assert(_confettiParticleSystem != null, "_confettiParticleSystem is not assigned");
        Debug.Assert(_audioSource != null, "_audioSource is not assigned");
        Debug.Assert(_eatSound != null, "_eatSound is not assigned");

        _circleInitialColor = _circleSpriteRen.color;

        ResetTarget();

        // Force it to be same size so text not skewed.
        Utils.SetGlobalScale(_canvasRectTransform, new Vector3(0.01f, 0.01f, 0.01f));

        OnGoalReached.AddListener(delegate { _animator.SetTrigger("ate"); });
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_reachedTarget) return;
        if (other.gameObject.tag == Constants.TAG_TARGET_OBJ)
        {
            _eatSound.PlayOneShot(_audioSource);
            _targetCount++;
            _flashParticleSystem.Stop();
            _flashParticleSystem.Clear();
            _flashParticleSystem.Play();

            TargetObject targetObj = other.gameObject.GetComponent<TargetObject>();
            targetObj.Consume();

            _countLeftText.text = Mathf.Max(_targetGoalNum - _targetCount, 0).ToString();
            if (_targetCount >= _targetGoalNum)
            {
                _reachedTarget = true;
                // _circleSpriteRen.color = _goalReachedColor;
                _confettiParticleSystem.Play();
                OnGoalReached.Invoke();
            }
        }
    }

    public void ResetTarget()
    {
        _targetCount = 0;
        _reachedTarget = false;
        _circleSpriteRen.color = _circleInitialColor;
        _flashParticleSystem.Stop();
        _flashParticleSystem.Clear();
        _confettiParticleSystem.Stop();
        _confettiParticleSystem.Clear();
        _countLeftText.text = _targetGoalNum.ToString();

        _animator.SetTrigger("idle");
    }
}
