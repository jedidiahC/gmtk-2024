using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetArea : MonoBehaviour {
    [SerializeField] private int _targetGoalNum = 0;
    [SerializeField] private RectTransform _canvasRectTransform = null;
    [SerializeField] private TextMeshProUGUI _countLeftText = null;
    private SpriteRenderer _spriteRen = null;
    private int _targetCount = 0;
    private bool _reachedTarget = false;
    public bool ReachedTarget { get { return _reachedTarget; } }

    private Color _unreachedColor = new Color(1f, 0f, 0f, 0.5f);
    private Color _reachedColor = new Color(0f, 1f, 0f, 0.15f);

    void Awake() {
        Debug.Assert(_targetGoalNum > 0, "_targetGoalNum must be > 0");
        Debug.Assert(_canvasRectTransform != null, "_canvasRectTransform is not assigned");
        Debug.Assert(_countLeftText != null, "_countLeftText is not assigned");
        _spriteRen = GetComponent<SpriteRenderer>();
        Debug.Assert(_spriteRen != null, "_spriteRen is not found!");

        ResetTarget();

        // Force it to be same size so text not skewed.
        Utils.SetGlobalScale(_canvasRectTransform, new Vector3(0.01f, 0.01f, 0.01f));
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Finish") {
            _targetCount++;
            _countLeftText.text = Mathf.Max(_targetGoalNum - _targetCount, 0).ToString();
            if (_targetCount >= _targetGoalNum) {
                _reachedTarget = true;
                _spriteRen.color = _reachedColor;
                StageManager.Instance.CheckLevelClear();
            }
        }
    }

    public void ResetTarget() {
        _targetCount = 0;
        _reachedTarget = false;
        _spriteRen.color = _unreachedColor;
        _countLeftText.text = _targetGoalNum.ToString();
    }
}
