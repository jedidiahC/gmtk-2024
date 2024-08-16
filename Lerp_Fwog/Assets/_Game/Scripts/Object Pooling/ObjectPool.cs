using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    [Header("Prefab must inherit PoolObject")]
    [Tooltip("Prefab must have component script that inherits from PoolObject class")]
    [SerializeField] private GameObject _poolObjectPrefab = null;
    [SerializeField] private int _initialPoolSize = 0;
    private List<PoolObject> _objectPool;

    void Awake() {
        Debug.Assert(_poolObjectPrefab != null, "_poolObjectPrefab not assigned");
        Debug.Assert(_initialPoolSize > 0, "_initialPoolSize must be > 0");
        _objectPool = new List<PoolObject>(_initialPoolSize);

        for (int i = 0; i < _initialPoolSize; i++) {
            GameObject instance = Instantiate(_poolObjectPrefab);
            PoolObject poolObject = instance.GetComponent<PoolObject>();
            poolObject.Despawn();
            _objectPool.Add(poolObject);
        }
    }

    public PoolObject GetFreePoolObject() {
        for (int i = 0; i < _objectPool.Count; i++) {
            if (!_objectPool[i].IsValid) {
                return _objectPool[i];
            }
        }

        GameObject newInstance = Instantiate(_poolObjectPrefab);
        PoolObject newPoolObject = newInstance.GetComponent<PoolObject>();
        newPoolObject.Despawn();
        _objectPool.Add(newPoolObject);
        Debug.LogWarning("OUT OF FREE OBJECTS! Creating new instance to object pool: " + _poolObjectPrefab.name);
        return newPoolObject;
    }
}
