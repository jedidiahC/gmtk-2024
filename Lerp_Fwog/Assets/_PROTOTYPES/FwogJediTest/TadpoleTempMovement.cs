using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMovement : MonoBehaviour
{
    [SerializeField] private Vector2 _moveDir;

    public void Stop()
    {
        _moveDir = Vector2.zero;
    }


    // Start is called before the first frame update
    void Start()
    {
        _moveDir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 delta = _moveDir * 1.0f * Time.fixedDeltaTime;
        transform.position += delta;
    }
}
