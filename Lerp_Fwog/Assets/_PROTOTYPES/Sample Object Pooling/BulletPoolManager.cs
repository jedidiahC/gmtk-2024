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

    private float _countDown = 0.5f;
    private const float SHOT_INTERVAL = 0.025f;
    void Update() {
        if (_countDown <= 0.0f) {
            SamplePoolBullet bullet = (SamplePoolBullet)_bulletPool.GetFreePoolObject();
            if (bullet == null) return;

            Vector3 ctrlPoint = Vector3.Lerp(_spawnPoint.position, _targetPoint.position, Random.Range(0.15f, 0.5f));
            const float DEVIANCE = 4.5f;
            ctrlPoint.y += Random.Range(-DEVIANCE, DEVIANCE);
            ctrlPoint.x += Random.Range(-DEVIANCE, DEVIANCE);
            bullet.SetTarget(_targetPoint.position, 1.0f, ctrlPoint);
            bullet.Spawn(_spawnPoint.position);

            _countDown += SHOT_INTERVAL;
        }

        _countDown -= Time.deltaTime;
    }
}
