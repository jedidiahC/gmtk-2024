using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroFrog : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;

    void Awake() {
        Debug.Assert(_rigidbody != null, "_rigidbody is not assigned!");
    }

    void FixedUpdate() {
    }
}
