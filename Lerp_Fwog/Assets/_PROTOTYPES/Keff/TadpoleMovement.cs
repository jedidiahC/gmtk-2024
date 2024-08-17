using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCreatureType
{
    NONE,
    TADPOLE,
    FISH
}

public class TadpoleMovement : MonoBehaviour
{
    [SerializeField] private eCreatureType _creatureType;
    private Vector3 _dir;
    [SerializeField] private float _moveForce;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _rotateSpeed;
    [Range(0,1)] [SerializeField] private float _rotateDampening;
    [SerializeField] private float _viewTargetRange;
    [Range(0,180)] [SerializeField] private float _viewTargetAngle;

    [SerializeField] private Transform _targetTransform;
    private Rigidbody2D _rb;

    // steering vectors
    [Header("Steering Variables")]
    [SerializeField] private float _steeringUpdateInterval;
    [Range(1,36)] [SerializeField] private int _steeringVectorsQty;
    [Range(0,360)] [SerializeField] private int _steeringVectorsMaxAngle;
    [Range(0,1)] [SerializeField] private float _steeringCircleCastRadiusRatio;
    private float _steeringCircleCastRadius;
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
        _steeringCircleCastRadius = transform.localScale.x * _steeringCircleCastRadiusRatio;

        if (_creatureType == eCreatureType.TADPOLE) _targetTransform = GameObject.FindGameObjectWithTag("Finish").transform;
        else if (_creatureType == eCreatureType.FISH) FindTadpoleTarget();
    }

    void Update()
    {
        if (_creatureType == eCreatureType.FISH) FindTadpoleTarget();
        if (!MoveToTargetInSight())
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
                RaycastHit2D hit = Physics2D.CircleCast(transform.position + (_steeringVectorsArr[i] * (_steeringCircleCastRadius * 2 + 0.05f)), _steeringCircleCastRadius, _steeringVectorsArr[i], raycastDist);
                if (hit.collider == null)
                {
                    _desiredDir = _steeringVectorsArr[i];
                    Debug.DrawLine(transform.position, transform.position + _steeringVectorsArr[i], Color.green, 5f);
                    break;
                }
                Debug.DrawLine(transform.position, transform.position + _steeringVectorsArr[i], Color.red, 5f);
                if (i == _steeringVectorsQty - 1) _desiredDir = _steeringVectorsArr[i];
            }
            _currentSteeringUpdateDuration = 0;
        }
    }

    bool MoveToTargetInSight()
    {
        if (_targetTransform == null) return false;

        Vector3 targetDir = (_targetTransform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position + (targetDir * (_steeringCircleCastRadius * 2 + 0.05f)), _steeringCircleCastRadius, targetDir, _viewTargetRange);

        // I can see my target
        if (hit.collider != null && hit.collider.transform == _targetTransform)
        {
            if (Vector3.Angle(_dir, targetDir) <= _viewTargetAngle)
            {
                _desiredDir = targetDir;
                return true;
            }
        }
        return false;
    }

    void FindTadpoleTarget()
    {
        Collider2D[] foundColliders = Physics2D.OverlapCircleAll(transform.position, _viewTargetRange);
        if (foundColliders == null || foundColliders.Length == 0) return;

        foreach(Collider2D col in foundColliders)
        {
            if (col.gameObject.CompareTag("Tadpole"))
            {
                if (Vector3.Angle(_dir, (col.transform.position - transform.position).normalized) <= _viewTargetAngle)
                {
                    _targetTransform = col.transform;
                    return;
                }
            }
        }
    }
}
