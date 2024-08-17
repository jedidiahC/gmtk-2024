using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    public UnityEvent<GameObject> OnSpawn = new();

    [SerializeField] private GameObject _prefab = null;
    [SerializeField] private Transform _parent = null;
    [SerializeField] private bool _isSpawning = true;
    [SerializeField] private float _spawnIntervalInSec = 6.0f;

    private float _timeToNextSpawn = 0.0f;

    private void Awake()
    {
        Debug.Assert(_prefab != null, "_prefab is not assigned!");
        _timeToNextSpawn = 0;
    }


    // Update is called once per frame
    private void Update()
    {
        // Don't spawn on interval if _spawnIntervalInSec < 0.
        if (_spawnIntervalInSec < 0)
        {
            return;
        }

        if (_isSpawning && _timeToNextSpawn <= 0)
        {
            Spawn();
            _timeToNextSpawn = _spawnIntervalInSec;
        }

        _timeToNextSpawn -= Time.deltaTime;
    }

    public GameObject Spawn()
    {
        GameObject newGo = Instantiate(_prefab, this.transform.position, Quaternion.identity, _parent);
        OnSpawn.Invoke(newGo);
        return newGo;
    }

}
