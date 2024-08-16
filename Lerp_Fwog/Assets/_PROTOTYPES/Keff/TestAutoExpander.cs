using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAutoExpander : MonoBehaviour
{
    [SerializeField] private float _expandSpeed;
    [SerializeField] private float _maxSize;
    [SerializeField] private float _minSize;
    [SerializeField] private float _stopDuration;

    private bool _expand;
    private bool _pause;
    private Vector3 _startScale;

    void Start()
    {
        _startScale = transform.localScale;
        _expand = true;
        _pause = false;
    }

    void Update()
    {
        if (_expand)
        {
            if (transform.localScale.x > _startScale.x * _maxSize)
            {
                _pause = true;
                StartCoroutine(WaitToChangeExpandState(false));
            }
            else
            {
                if (!_pause)
                    transform.localScale = new Vector3(transform.localScale.x + _expandSpeed * Time.deltaTime, transform.localScale.y + _expandSpeed * Time.deltaTime, transform.localScale.z);
            }
        }
        else
        {
            if (transform.localScale.x < _startScale.x * _minSize)
            {
                _pause = true;
                StartCoroutine(WaitToChangeExpandState(true));
            }
            else
            {
                if (!_pause)
                    transform.localScale = new Vector3(transform.localScale.x - _expandSpeed * Time.deltaTime, transform.localScale.y - _expandSpeed * Time.deltaTime, transform.localScale.z);
            }
        }    
    }

    IEnumerator WaitToChangeExpandState(bool inExpandState)
    {
        yield return new WaitForSeconds(_stopDuration);
        _expand = inExpandState;
        _pause = false;
    }
}
