using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour {
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private SpriteRenderer _spriteRen;

    void Awake() {
        Debug.Assert(tag == Constants.TAG_TARGET_OBJ, "Target Object is set to wrong tag. " + gameObject.name);

        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spriteRen = GetComponent<SpriteRenderer>();
        Debug.Assert(_rigidbody != null, "_rigidbody is not found!");
        Debug.Assert(_collider != null, "_collider is not found!");
        Debug.Assert(_spriteRen != null, "_spriteRen is not found!");

        Reset();
    }

    public void Consume() {
        // TODO: Juice up the consume
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        _spriteRen.enabled = false;
    }

    public void Reset() {
        _rigidbody.isKinematic = true; // NOTE: When reset is called, stage should be paused and not simulating.
        _collider.enabled = true;
        _spriteRen.enabled = true;
    }
}
