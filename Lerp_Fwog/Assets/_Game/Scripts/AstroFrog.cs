using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroFrog : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private ParticleSystem _fireParticles = null;

    private Vector3 _particleOffset;

    void Awake() {
        Debug.Assert(_rigidbody != null, "_rigidbody is not assigned!");
        Debug.Assert(_fireParticles != null, "_fireParticles is not assigned!");
    
        StopFireParticles();
        _particleOffset = _fireParticles.transform.position - transform.position;
    }

    void Update() {
        _fireParticles.transform.position = transform.position + _particleOffset;
        _fireParticles.transform.rotation = Quaternion.identity;
    }

    public void StartFireParticles() {
        _fireParticles.Play();
    }

    public void StopFireParticles() {
        _fireParticles.Stop();
        _fireParticles.Clear();
    } 
}
