using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameTracker : MonoBehaviour
{
    private static GameTracker _instance = null;


    UnityEvent onAllDead = new();

    [SerializeField] private List<TadpoleLife> _lives = new();

    public static GameTracker GetInstance()
    {
        return _instance;
    }

    public List<TadpoleLife> GetTadpoles()
    {
        return _lives;
    }

    public void AddLifeGo(GameObject lifeGo)
    {
        if (lifeGo.TryGetComponent<TadpoleLife>(out var life))
        {
            AddLife(life);
        }
    }

    public void AddLife(TadpoleLife life)
    {
        _lives.Add(life);
        life.onDead.AddListener(CheckAllDeadCondition);
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != null && _instance != this)
        {
            DestroyImmediate(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (TadpoleLife life in _lives)
        {
            life.onDead.AddListener(CheckAllDeadCondition);
            AddLife(life);
        }
    }

    void CheckAllDeadCondition()
    {
        foreach (TadpoleLife life in _lives)
        {
            if (!life.IsDead)
            {
                return;
            }
        }

        Debug.Log("All dead");
        onAllDead.Invoke();
    }

    void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

}
