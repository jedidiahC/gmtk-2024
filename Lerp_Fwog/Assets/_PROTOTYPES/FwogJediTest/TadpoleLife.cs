using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TadpoleLife : MonoBehaviour
{
    private const string ESCAPE_TAG = "Finish";

    public UnityEvent onDead = new UnityEvent();
    public UnityEvent onEscape = new UnityEvent();

    [SerializeField] private bool _isDead;
    [SerializeField] private bool _hasEscaped;

    public bool IsDead { get { return _isDead; } }
    public bool HasEscaped { get { return _hasEscaped; } }

    public void Kill()
    {
        if (_isDead || _hasEscaped)
        {
            return;
        }

        _isDead = true;
        onDead.Invoke();
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead)
        {
            return;
        }

        if (other.tag == ESCAPE_TAG)
        {
            _hasEscaped = true;
            onEscape.Invoke();
            this.gameObject.SetActive(false);
        }
    }
}
