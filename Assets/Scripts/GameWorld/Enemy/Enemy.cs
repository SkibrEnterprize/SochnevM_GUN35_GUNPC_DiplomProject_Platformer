using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _checkDistance = 0.5f;

    [SerializeField] private float _knockbackResistance = 5f; // Насколько быстро гасится отскок



    private Transform _groundCheck; 
    private Transform _wallCheck;   
    private Vector3 _impactVelocity = Vector3.zero; // Текущий импульс отскока

    private CharacterController _controller;
    private EnemyStateMachine _stateMachine;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stateMachine = new EnemyStateMachine();

        _stateMachine.ChangeState(new PatrolState(this));
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
        Debug.Log($"Враг получил {damage} урона!");

        // Вычисляем направление от игрока к врагу
        Vector3 pushDirection = (transform.position - attackerPosition).normalized;
        pushDirection.y = 0.5f; // Немного подкидываем вверх для эффекта

        ApplyKnockback(pushDirection, 10f); // 10f — сила отскока
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        // Направление отскока (обычно от игрока) * силу
        _impactVelocity = direction.normalized * force;

        // Переключаем стейт в Hit (если есть), чтобы прервать патруль
        // _stateMachine.ChangeState(new HitState(this)); 
    }

}