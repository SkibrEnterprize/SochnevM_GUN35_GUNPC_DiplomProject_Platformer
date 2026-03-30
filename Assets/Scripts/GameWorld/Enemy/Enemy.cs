using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour, IDamageable
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
    private Transform _player;
    [SerializeField] private LayerMask _obstacleLayer; // Слой стен и препятствий
    [SerializeField] private float _viewAngle = 90f; // Угол обзора
    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private bool _showDetectRadius = true;
    [SerializeField] private bool _showAttackRadius = true;
    [SerializeField] private bool _showEyeVision = true;

    public float LostRange => _lostRange;

    private Transform _enemyEyes;
    private Transform _groundCheck;
    private Transform _wallCheck;
    private Vector3 _impactVelocity = Vector3.zero; // Текущий импульс отскока

    private CharacterController _controller;
    private EnemyStateMachine _stateMachine;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stateMachine = new EnemyStateMachine();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _stateMachine.ChangeState(new EnemyPatrolState(this));

        _enemyEyes = GetComponentInChildren<EnemyEyesPoint>().transform;
        _groundCheck = GetComponentInChildren<EnemyGroundCheck>().transform;
        _wallCheck = GetComponentInChildren<EnemyWallCheck>().transform;
    }

    private void Update() => _stateMachine.Update();

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
                    if (hit.collider.TryGetComponent<PlayerDetect>(out PlayerDetect playerDetect))
                    {
                        Debug.Log("Find Player");
                        return true;
                    }
                }
            }
        }
        Debug.Log("NOT Find Player");
        return false;
    }

    public bool CanAttackPlayer()
    {
        if (_player == null) return false;
        return Vector3.Distance(transform.position, _player.position) <= _attackRange;
    }
    // Метод для удобной смены стейтов извне (из самих стейтов)
    public void ChangeState(IEnemyState newState) => _stateMachine.ChangeState(newState);
    public Transform GetPlayer() => _player;


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
