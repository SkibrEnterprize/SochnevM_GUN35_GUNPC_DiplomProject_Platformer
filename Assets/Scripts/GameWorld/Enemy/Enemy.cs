using DG.Tweening;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(EnemyHealth))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _runSpeed = 4f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private float _checkDistance = 0.5f;
    [SerializeField] private float _turnSpeed = 10f;
    [SerializeField] private float _knockbackResistance = 5f;

    [Header("Detection")]
    [SerializeField] private Transform _enemyEyesForDebug;
    [SerializeField] private float _detectionRange = 7f;
    [SerializeField] private float _lostRange = 10f;
    [SerializeField] private float _closeAwarenessRange = 2f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private LayerMask _layersForVision;
    [SerializeField] private float _viewAngle = 90f;
    private Transform _player;

    [Header("Attack")]
    [SerializeField] private EnemyTypeAttack _enemyTypeAttack;

    [Header("Combat Settings")]
    [SerializeField] private int _health = 100;
    [SerializeField] private int _attackDamage = 15;
    [SerializeField] private float _attackCooldown = 1.2f;
    [SerializeField] private LayerMask _attackMask;
    private float _lastAttackTime;

    [Header("Ranged Attack")]
    [SerializeField] private GameObject _projectilePrefab;
    private Transform _firePoint;

    [Header("Audio Settings")]
    private float _stepTimer;
    [SerializeField] private float _baseStepInterval = 0.5f;

    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private bool _showDetectRadius = true;
    [SerializeField] private bool _showAttackRadius = true;
    [SerializeField] private bool _showEyeVision = true;
    [SerializeField] private bool _showAwarenessRange = true;

    //VFX config
    private Transform _combatVFXPoint;
    private VFXEventBus _vfxBus;
    private SoundEventBus _soundBus;


    public float LostRange => _lostRange;
    public EnemyTypeAttack EnemyTypeAttack => _enemyTypeAttack;
    private Transform _enemyEyes;
    private Transform _groundCheck;
    private Transform _wallCheck;
    private Vector3 _impactVelocity = Vector3.zero; 

    private CharacterController _controller;
    private EnemyStateMachine _stateMachine;
    private Animator _animator;

    [Inject]
    public void Construct(VFXEventBus vfxBus, SoundEventBus soundEventBus)
    {
        _vfxBus = vfxBus;
        _soundBus = soundEventBus;
    }
    private void Awake()
    {
        _stateMachine = new EnemyStateMachine();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _stateMachine.ChangeState(new EnemyPatrolState(this));

        _firePoint = GetComponentInChildren<FirePoint>().transform;
        _combatVFXPoint = GetComponentInChildren<AttackVFXPoint>().transform;
        _enemyEyes = GetComponentInChildren<EnemyEyesPoint>().transform;
        _groundCheck = GetComponentInChildren<EnemyGroundCheck>().transform;
        _wallCheck = GetComponentInChildren<EnemyWallCheck>().transform;
    }

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        var health = GetComponent<EnemyHealth>();
        health.OnTakeDamage += (damage) => ChangeState(new EnemyHitState(this, Vector3.zero));
        health.OnDeath += () => OnDeathLogic();
    }

    private void OnDeathLogic()
    {
        ChangeState(new EnemyDeathState(this));
        _soundBus.Play(SoundType.EnemyDeath);
    }

    private void Update() => _stateMachine.Update();
    public void ChangeState(IEnemyState newState) => _stateMachine.ChangeState(newState);
    public Transform GetPlayer() => _player;

    public bool IsObstacleAhead()
    {
        LayerMask combinedLayer = _wallLayer | _enemyLayer;
        bool hit = Physics.Raycast(_wallCheck.position, transform.right, _checkDistance, combinedLayer);
        Debug.DrawRay(_wallCheck.position, transform.right * _checkDistance, hit ? Color.red : Color.green);
        return hit;
    }

    public bool IsGroundAhead()
    {
        return Physics.CheckSphere(_groundCheck.position, 0.2f, _groundLayer);
    }

    public void Move(Vector3 direction, float speedMultiplier = 1f)
    {
        direction.z = 0;
        Vector3 velocity = direction * (_moveSpeed * speedMultiplier);

        if (_impactVelocity.magnitude > 0.2f)
        {
            velocity += _impactVelocity;
            _impactVelocity = Vector3.Lerp(_impactVelocity, Vector3.zero, _knockbackResistance * Time.deltaTime);
        }
        else
        {
            _impactVelocity = Vector3.zero;
        }

        float animValue = direction.magnitude * speedMultiplier;

        if (_controller.isGrounded && animValue > 0.1f)
        {
            _stepTimer -= Time.deltaTime * animValue;

            if (_stepTimer <= 0)
            {
                _soundBus.Play(SoundType.EnemyStep, transform.position);
                _stepTimer = _baseStepInterval; 
            }
        }
        else
        {
            _stepTimer = 0;
        }
        // -------------------------------------

        if (_animator != null)
        {
            _animator.SetFloat("Speed", animValue, 0.1f, Time.deltaTime);
        }

        velocity.z = 0;
        velocity.y -= 9.81f;

        if (_controller.enabled)
        {
            _controller.Move(velocity * Time.deltaTime);
        }

        Vector3 pos = transform.position;
        if (Mathf.Abs(pos.z) > 0.001f)
        {
            pos.z = 0;
            transform.position = pos;
        }
    }

    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        Vector3 pushDirection = (transform.position - attackerPosition).normalized;
        _stateMachine.ChangeState(new EnemyHitState(this, pushDirection));
        _soundBus.Play(SoundType.EnemyHit);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        _impactVelocity = direction.normalized * force;
    }

    public bool CanSeePlayer()
    {
        if (_player == null || _enemyEyes == null) return false;

        float distance = Vector3.Distance(_enemyEyes.position, _player.position);
        Vector3 dirToPlayer = (_player.position - _enemyEyes.position).normalized;

        if (distance <= _closeAwarenessRange)
        {
            if (Physics.Raycast(_enemyEyes.position, dirToPlayer, out RaycastHit hit, _closeAwarenessRange, _layersForVision))
            {
                if (hit.collider.TryGetComponent<PlayerHealth>(out _)) return true;
            }
        }

        if (distance <= _detectionRange)
        {
            if (Vector3.Angle(_enemyEyes.right, dirToPlayer) < _viewAngle / 2f)
            {
                if (Physics.Raycast(_enemyEyes.position, dirToPlayer, out RaycastHit hit, _detectionRange, _layersForVision))
                {
                    if (hit.collider.TryGetComponent<PlayerHealth>(out _))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
        //if (_player == null || _enemyEyes == null) return false;

        //float distance = Vector3.Distance(_enemyEyes.position, _player.position);

        //if (distance <= _closeAwarenessRange)
        //{
        //    return true;
        //}

        //Vector3 dirToPlayer = (_player.position - _enemyEyes.position).normalized;
        //if (distance <= _detectionRange)
        //{
        //    if (Vector3.Angle(_enemyEyes.right, dirToPlayer) < _viewAngle / 2f)
        //    {
        //        if (Physics.Raycast(_enemyEyes.position, dirToPlayer, out RaycastHit hit, _detectionRange))
        //        {
        //            if (hit.collider.TryGetComponent<PlayerHealth>(out _))
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //}
        //return false;
    }

    public void RotateTowards(Vector3 targetPosition)
    {
        float targetY = targetPosition.x > transform.position.x ? 0f : -180f;
        Quaternion targetRotation = Quaternion.Euler(0, targetY, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
    }

    public bool CanAttackReady() => Time.time >= _lastAttackTime + _attackCooldown;
    public bool CanAttackPlayer()
    {
        if (_player == null) return false;

        Vector3 enemyPos = transform.position;
        Vector3 playerPos = _player.position;

        float horizontalDistance = Vector2.Distance(new Vector2(enemyPos.x, enemyPos.z), new Vector2(playerPos.x, playerPos.z));

        float verticalDistance = Mathf.Abs(enemyPos.y - playerPos.y);

        return horizontalDistance <= _attackRange && verticalDistance < 1f;
    }


    public void PerformAttack()
    {
        _lastAttackTime = Time.time;

        Collider[] hitColliders = Physics.OverlapSphere(_enemyEyes.position, _attackRange, _attackMask);
        foreach (var hit in hitColliders)
        {
            if (hit.TryGetComponent(out IHealthAffected target))
            {
                float verticalDiff = Mathf.Abs(hit.transform.position.y - _enemyEyes.position.y);
                if (verticalDiff > 1f) continue;

                target.ApplyHealthChange(-_attackDamage, transform.position);

                Vector3 vfxPosition = _combatVFXPoint.position;
                Quaternion vfxRotation = _combatVFXPoint.rotation * Quaternion.Euler(25, -90, 45);
                _vfxBus.Play(VFXType.Attack, vfxPosition, vfxRotation, _controller.gameObject.transform);
                _soundBus.Play(SoundType.EnemyAttack);
            }
        }
    }
    public void Shoot()
    {
        if (_player == null) return;
        _lastAttackTime = Time.time;

        GameObject bullet = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
        Vector3 direction = (_player.position - _firePoint.position).normalized;

        if (bullet.TryGetComponent(out EnemyProjectile projectile))
        {
            projectile.Launch(direction, transform.position);
            _soundBus.Play(SoundType.EnemyRangeAttack, transform.position);
        }
    }
    public void StartDespawn(float delay)
    {
        Invoke(nameof(FadeOut), delay - 1f);
    }

    private void FadeOut()
    {
        transform.DOMoveY(transform.position.y - 0.5f, 1f).OnComplete(() => Destroy(gameObject));
    }
    public void SetTrigger(string name) => _animator.SetTrigger(name);
    public void SetBool(string name, bool value) => _animator.SetBool(name, value);

    public bool HasClearLineOfSight()
    {
        if (_player == null || _enemyEyes == null) return false;

        Vector3 direction = (_player.position - _enemyEyes.position).normalized;
        float distance = Vector3.Distance(_enemyEyes.position, _player.position);

        if (Physics.Raycast(_enemyEyes.position, direction, out RaycastHit hit, distance, _layersForVision | _groundLayer | _wallLayer))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                return false;
            }
        }
        return true;
    }
    private void OnDrawGizmos()
    {
        if (!_showGizmos || _enemyEyesForDebug == null) return;

        if (_showDetectRadius)
        {
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(_enemyEyesForDebug.position, _detectionRange);
        }
        if (_showAttackRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_enemyEyesForDebug.position, _attackRange);
        }

        if (_showEyeVision)
        {
            Gizmos.color = Color.yellow;
            Vector3 topBoundary = Quaternion.Euler(0, 0, _viewAngle / 2f) * _enemyEyesForDebug.right;
            Vector3 bottomBoundary = Quaternion.Euler(0, 0, -_viewAngle / 2f) * _enemyEyesForDebug.right;

            Gizmos.DrawRay(_enemyEyesForDebug.position, topBoundary * _detectionRange);
            Gizmos.DrawRay(_enemyEyesForDebug.position, bottomBoundary * _detectionRange);

            Gizmos.DrawLine(_enemyEyesForDebug.position + topBoundary * _detectionRange,
                            _enemyEyesForDebug.position + bottomBoundary * _detectionRange);
        }

        if (CanSeePlayer() && _player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_enemyEyesForDebug.position, _player.position);

            Gizmos.DrawSphere(_player.position, 0.2f);
        }
        if (_showAwarenessRange)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _closeAwarenessRange);
        }
    }
}
