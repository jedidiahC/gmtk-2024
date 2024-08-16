using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolManager : MonoBehaviour {
    [SerializeField] private ObjectPool _bulletPool = null;
    [SerializeField] private Transform _spawnPoint = null;
    [SerializeField] private Transform _targetPoint = null;

    void Awake() {
        Debug.Assert(_bulletPool != null, "_bulletPool not assigned");
        Debug.Assert(_spawnPoint != null, "_spawnPoint not assigned");
        Debug.Assert(_targetPoint != null, "_targetPoint not assigned");
    }

    private float _countDown = 1.0f;
    private const float SHOT_INTERVAL = 0.15f;
    void Update() {
        if (_countDown <= 0.0f) {
            SamplePoolBullet bullet = (SamplePoolBullet)_bulletPool.GetFreePoolObject();
            if (bullet == null) return;

            bullet.SetTarget(_targetPoint.position, 1.0f);
            bullet.Spawn(_spawnPoint.position);

            _countDown += SHOT_INTERVAL;
        }

        _countDown -= Time.deltaTime;
    }
}
