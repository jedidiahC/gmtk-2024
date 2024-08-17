using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TadpoleLife : MonoBehaviour
{
    public UnityEvent onDead = new UnityEvent();

    [SerializeField] private bool _isDead;

    public bool IsDead { get { return _isDead; } }

    public void Kill()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        onDead.Invoke();
        this.gameObject.SetActive(false);
    }
}
