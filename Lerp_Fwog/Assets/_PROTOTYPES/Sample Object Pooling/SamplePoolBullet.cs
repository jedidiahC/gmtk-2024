using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplePoolBullet : PoolObject
{
    [SerializeField] private SpriteRenderer _spriteRen = null;
    private Vector3 _spawnPos;
    private Vector3 _targetPos;
    private Vector3 _ctrlPos;
    private float _speed;
    private float _t;

    public override void Spawn(Vector3 inSpawnPos) {
        base.Spawn(inSpawnPos);
        _spriteRen.enabled = true;
        _spawnPos = inSpawnPos;

        _t = 0.0f;
    }
    public void SetTarget(Vector3 inTargetPos, float inSpeed, Vector3 inCtrlPos) {
        _targetPos = inTargetPos;
        _speed = inSpeed;
        _ctrlPos = inCtrlPos;

        // RANDOM COLOUR
        _spriteRen.color = Random.ColorHSV(0f, 1f, 0f, 1f, 0.5f, 1f, 1f, 1f);
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

        // transform.position = Vector3.Lerp(_spawnPos, _targetPos, _t);
        transform.position = Utils.QuadraticBezier(_spawnPos, _ctrlPos, _targetPos, _t);
        _t += Time.deltaTime * _speed;
        if (_t > 1.0f) Despawn();
    }
}
