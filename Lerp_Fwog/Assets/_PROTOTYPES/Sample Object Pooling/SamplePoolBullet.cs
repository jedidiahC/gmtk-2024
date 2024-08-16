using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SamplePoolBullet : PoolObject
{
    [SerializeField] private SpriteRenderer _spriteRen = null;
    private Vector3 _spawnPos;
    private Vector3 _targetPos;
    private float _speed;
    private float _t;

    public override void Spawn(Vector3 inSpawnPos) {
        base.Spawn(inSpawnPos);
        _spriteRen.enabled = true;
        _spawnPos = inSpawnPos;

        _t = 0.0f;
    }
    public void SetTarget(Vector3 inTargetPos, float inSpeed) {
        _targetPos = inTargetPos;
        _speed = inSpeed;
    }

    public override void Despawn() {
        base.Despawn();
        _spriteRen.enabled = false;
    }

    void Awake() {
        Debug.Assert(_spriteRen != null, "_spriteRen is not assigned");
    }

    void Update() {
        if (!base.IsValid) return;

        transform.position = Vector3.Lerp(_spawnPos, _targetPos, _t);
        _t += Time.deltaTime * _speed;
        if (_t > 1.0f) Despawn();
    }
}
