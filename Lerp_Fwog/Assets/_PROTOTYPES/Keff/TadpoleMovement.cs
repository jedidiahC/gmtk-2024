using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TadpoleMovement : MonoBehaviour
{
    private Vector3 _dir;
    [SerializeField] private float _moveForce;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _rotateSpeed;
    [Range(0,1)] [SerializeField] private float _rotateDampening;

    private Rigidbody2D _rb;

    // steering vectors
    [Header("Steering Variables")]
    [SerializeField] private float _steeringUpdateInterval;
    [Range(1,36)] [SerializeField] private int _steeringVectorsQty;
    [Range(0,360)] [SerializeField] private int _steeringVectorsMaxAngle;
    [SerializeField] private float _steeringCircleCastRadius;
    [SerializeField] private float _steeringCirclecastDistance;
    // front vector checks the furthest
    [SerializeField] private float _circlecastExtendedDist;

    private Vector3 _desiredDir;
    private Vector3[] _steeringVectorsArr;
    private float _currentSteeringUpdateDuration;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _desiredDir = transform.right;

        // generate a circle of vectors around tadpole
        _steeringVectorsArr = new Vector3[_steeringVectorsQty];
        SetSteeringVectors();
        _currentSteeringUpdateDuration = 0;
    }

    void SetSteeringVectors(float inFacingRelativeAngleRad = 0)
    {
        float steeringAngleStep = (float)_steeringVectorsMaxAngle * Mathf.Deg2Rad / (float)(_steeringVectorsQty - 1);
        int levelStep = 0;

        for (int i = 0; i < _steeringVectorsQty; i++)
        {
            Vector3 newVect = Vector3.zero;
            float currentAngleStep = levelStep * steeringAngleStep;
            if (i % 2 == 0)
            {
                newVect = new Vector3(Mathf.Cos(inFacingRelativeAngleRad + currentAngleStep), Mathf.Sin(inFacingRelativeAngleRad + currentAngleStep), 0);
                // Debug.DrawLine(transform.position, transform.position + _steeringVectorsArr[i], Color.red);
                levelStep++;
            }
            else
            {
                newVect = new Vector3(Mathf.Cos(inFacingRelativeAngleRad + -currentAngleStep), Mathf.Sin(inFacingRelativeAngleRad + -currentAngleStep), 0);
                // Debug.DrawLine(transform.position, transform.position + _steeringVectorsArr[i], Color.green);
            }
            _steeringVectorsArr[i] = newVect;
        }
    }

    void Update()
    {
        Steering();
    }

    void FixedUpdate()
    {
        _desiredDir.Normalize();
        if (Vector3.Angle(_dir, _desiredDir) > 1)
        {
            _dir = Vector3.Lerp(_dir, _desiredDir, _rotateSpeed * Time.fixedDeltaTime);
            _dir.Normalize();
            _rb.velocity *= _rotateDampening;
        }
        else
        {
            _dir = _desiredDir;
        }
        float angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
        _rb.rotation = angle;

        if (_rb.velocity.magnitude < _maxSpeed)
        {
            _rb.AddForce(_dir * _moveForce * Time.fixedDeltaTime);
        }
    }

    void Steering()
    {
        _currentSteeringUpdateDuration += Time.deltaTime;
        if (_currentSteeringUpdateDuration > _steeringUpdateInterval)
        {
            float facingRelativeAngle = Vector3.Angle(transform.right, Vector2.right) * Mathf.Deg2Rad;
            if (Vector3.Dot(transform.right, Vector2.up) < 0) facingRelativeAngle *= -1;
            SetSteeringVectors(facingRelativeAngle);


            for (int i = 0; i < _steeringVectorsQty; i++)
            {
                float raycastDist = _steeringCirclecastDistance + (1 - i / _steeringVectorsQty) * _circlecastExtendedDist;
                RaycastHit2D hit = Physics2D.CircleCast(transform.position, _steeringCircleCastRadius, _steeringVectorsArr[i], raycastDist);
                if (hit.collider == null)
                {
                    _desiredDir = _steeringVectorsArr[i];
                    Debug.DrawLine(transform.position, transform.position + _steeringVectorsArr[i], Color.green);
                    break;
                }
                Debug.DrawLine(transform.position, transform.position + _steeringVectorsArr[i], Color.red);
                if (i == _steeringVectorsQty - 1) _desiredDir = _steeringVectorsArr[i];
            }
            _currentSteeringUpdateDuration = 0;
        }
    }
}
