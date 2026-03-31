using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour, IHealthAffected
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _checkDistance = 0.5f;

    [SerializeField] private float _knockbackResistance = 5f; // Насколько быстро гасится отскок

    [Header("Detection")]
    [SerializeField] private Transform _enemyEyesForDebug;
    [SerializeField] private float _detectionRange = 7f;
    [SerializeField] private float _lostRange = 10f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private LayerMask _obstacleLayer; // Слой стен и препятствий
    [SerializeField] private float _viewAngle = 90f; // Угол обзора
    private Transform _player;

    [Header("Attack")]
    [SerializeField] private EnemyTypeAttack _enemyTypeAttack;

    [Header("Combat Settings")]
    [SerializeField] private int _health = 100;
    [SerializeField] private int _attackDamage = 15;
    [SerializeField] private float _attackCooldown = 1.2f;
    [SerializeField] private LayerMask _attackMask;
    private float _lastAttackTime;                        // Таймер последнего удара

    [Header("Ranged Attack")]
    [SerializeField] private GameObject _projectilePrefab;
    private Transform _firePoint; // Точка, откуда вылетает пуля

    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private bool _showDetectRadius = true;
    [SerializeField] private bool _showAttackRadius = true;
    [SerializeField] private bool _showEyeVision = true;

    [Header("VFX Settings")]
    [SerializeField] private Transform _combatVFXPoint;
    private VFXEventBus _vfxBus;


    public float LostRange => _lostRange;
    public EnemyTypeAttack EnemyTypeAttack => _enemyTypeAttack;
    private Transform _enemyEyes;
    private Transform _groundCheck;
    private Transform _wallCheck;
    private Vector3 _impactVelocity = Vector3.zero; // Текущий импульс отскока

    private CharacterController _controller;
    private EnemyStateMachine _stateMachine;

    [Inject]
    public void Construct(VFXEventBus vfxBus) => _vfxBus = vfxBus;
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stateMachine = new EnemyStateMachine();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _stateMachine.ChangeState(new EnemyPatrolState(this));

        _firePoint = GetComponentInChildren<FirePoint>().transform;
        _combatVFXPoint = GetComponentInChildren<CombatVFXPoint>().transform;
        _enemyEyes = GetComponentInChildren<EnemyEyesPoint>().transform;
        _groundCheck = GetComponentInChildren<EnemyGroundCheck>().transform;
        _wallCheck = GetComponentInChildren<EnemyWallCheck>().transform;
    }


    private void Update() => _stateMachine.Update();
    public void ChangeState(IEnemyState newState) => _stateMachine.ChangeState(newState);
    public Transform GetPlayer() => _player;

    public bool IsWallAhead()
    {
        bool hit = Physics.Raycast(_wallCheck.position, transform.right, _checkDistance, _wallLayer);
        Debug.DrawRay(_wallCheck.position, transform.right * _checkDistance, hit ? Color.red : Color.green);
        return hit;
    }

    public bool IsGroundAhead()
    {
        // Проверяем сферу в точке GroundCheck. Если она НЕ касается земли — впереди обрыв.
        return Physics.CheckSphere(_groundCheck.position, 0.2f, _groundLayer);
    }

    public void Move(Vector3 direction)
    {
        Vector3 velocity = direction * _moveSpeed;

        // Добавляем влияние отскока
        if (_impactVelocity.magnitude > 0.2f)
        {
            velocity += _impactVelocity;
            // Плавно гасим импульс со временем
            _impactVelocity = Vector3.Lerp(_impactVelocity, Vector3.zero, _knockbackResistance * Time.deltaTime);
        }
        else
        {
            _impactVelocity = Vector3.zero;
        }

        velocity.y -= 9.81f; // Гравитация
        _controller.Move(velocity * Time.deltaTime);
    }

    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        Vector3 pushDirection = (transform.position - attackerPosition).normalized;
        _stateMachine.ChangeState(new EnemyHitState(this, pushDirection));
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        // Направление отскока (обычно от игрока) * силу
        _impactVelocity = direction.normalized * force;

        // Переключаем стейт в Hit (если есть), чтобы прервать патруль
        // _stateMachine.ChangeState(new HitState(this)); 
    }

    public bool CanSeePlayer()
    {

        if (_player == null || _enemyEyes == null) return false;

        // Вектор от ГЛАЗ врага к игроку
        Vector3 dirToPlayer = (_player.position - _enemyEyes.position).normalized;
        float distance = Vector3.Distance(_enemyEyes.position, _player.position);

        // 1. Проверка дистанции
        if (distance <= _detectionRange)
        {
            // 2. Проверка угла обзора (используем направление глаз _enemyEyes.right)
            if (Vector3.Angle(_enemyEyes.right, dirToPlayer) < _viewAngle / 2f)
            {

                // 3. Raycast на препятствия
                if (Physics.Raycast(_enemyEyes.position, dirToPlayer, out RaycastHit hit, _detectionRange))
                {
                    if (hit.collider.TryGetComponent<PlayerHealth>(out PlayerHealth playerDetect))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // Реализация интерфейса IHealthAffected
    public void ApplyHealthChange(int delta, Vector3 sourcePosition = default)
    {
        _health += delta;

        if (delta < 0) // Если это урон
        {
            ApplyKnockback((transform.position - sourcePosition).normalized, 5f);
            // Если HP <= 0, переходим в DeathState (создадим позже)
            if (_health <= 0) _stateMachine.ChangeState(new EnemyDeathState(this));
        }
    }
    public bool CanAttackReady() => Time.time >= _lastAttackTime + _attackCooldown;
    public bool CanAttackPlayer()
    {
        if (_player == null) return false;
        // Считаем расстояние от врага до игрока
        float distance = Vector3.Distance(transform.position, _player.position);
        // Возвращаем true, если игрок в радиусе атаки
        return distance <= _attackRange;
    }


    public void PerformAttack()
    {
        _lastAttackTime = Time.time;

        // Поиск цели в радиусе атаки (Physics или Physics2D в зависимости от проекта)
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _attackRange, _attackMask);
        foreach (var hit in hitColliders)
        {
            // Ищем наш новый интерфейс на цели (Игроке)
            if (hit.TryGetComponent(out IHealthAffected target))
            {
                target.ApplyHealthChange(-_attackDamage, transform.position);
                Vector3 vfxPosition = _combatVFXPoint.position;
                Quaternion vfxRotation = _combatVFXPoint.rotation * Quaternion.Euler(25, -90, 45);
                _vfxBus.Play(VFXType.Attack, vfxPosition, vfxRotation, _controller.gameObject.transform);
            }
        }
    }
    public void Shoot()
    {
        if (_player == null) return;

        _lastAttackTime = Time.time;

        // Создаем пулю
        GameObject bullet = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);

        // 1. Вычисляем направление: (Куда - Откуда)
        Vector3 direction = (_player.position - _firePoint.position).normalized;

        // 2. Если пуля 3D, можно развернуть её «лицом» к игроку
        bullet.transform.right = direction;

        if (bullet.TryGetComponent(out EnemyProjectile projectile))
        {
            // Передаем вычисленное направление в метод Launch
            projectile.Launch(direction);
        }
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos || _enemyEyesForDebug == null) return;

        // радиус обнаружения
        if (_showDetectRadius)
        {
            Gizmos.color = new Color(0, 1, 0, 0.2f); // Прозрачно-зеленый
            Gizmos.DrawWireSphere(_enemyEyesForDebug.position, _detectionRange);
        }
        // радиус атаки 
        if (_showAttackRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }

        // конус обзора
        if (_showEyeVision)
        {
            Gizmos.color = Color.yellow;
            Vector3 topBoundary = Quaternion.Euler(0, 0, _viewAngle / 2f) * _enemyEyesForDebug.right;
            Vector3 bottomBoundary = Quaternion.Euler(0, 0, -_viewAngle / 2f) * _enemyEyesForDebug.right;

            Gizmos.DrawRay(_enemyEyesForDebug.position, topBoundary * _detectionRange);
            Gizmos.DrawRay(_enemyEyesForDebug.position, bottomBoundary * _detectionRange);

            // Соединяем концы лучей линией, чтобы получился треугольник (сектор)
            Gizmos.DrawLine(_enemyEyesForDebug.position + topBoundary * _detectionRange,
                            _enemyEyesForDebug.position + bottomBoundary * _detectionRange);
        }

        // линия до игрока (только если он увиден)
        if (CanSeePlayer() && _player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_enemyEyesForDebug.position, _player.position);

            Gizmos.DrawSphere(_player.position, 0.2f);
        }
    }
}
