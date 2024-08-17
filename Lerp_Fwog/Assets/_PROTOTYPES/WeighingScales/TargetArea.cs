using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArea : MonoBehaviour {
    [SerializeField] private int _targetGoalNum = 0;
    private SpriteRenderer _spriteRen = null;
    private int _targetCount = 0;
    private bool _reachedTarget = false;
    public bool ReachedTarget { get { return _reachedTarget; } }

    private Color _unreachedColor = new Color(1f, 0f, 0f, 0.5f);
    private Color _reachedColor = new Color(0f, 1f, 0f, 0.15f);

    void Awake() {
        Debug.Assert(_targetGoalNum > 0, "_targetGoalNum must be > 0");
        _spriteRen = GetComponent<SpriteRenderer>();
        Debug.Assert(_spriteRen != null, "_spriteRen is not found!");

        ResetTarget();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Finish") {
            _targetCount++;
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
    }
}
