using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolObject : MonoBehaviour {
    private bool _isValid = false;
    public bool IsValid { get { return _isValid; } }
    public virtual void Spawn(Vector3 inSpawnPos) {
        _isValid = true;
        transform.position = inSpawnPos;
    }
    public virtual void Despawn() {
        _isValid = false;
    }
}
