using UnityEngine;
using Zenject;

public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;

    protected EnemyStateMachine StateMachine;

    // Ссылки для состояний
    public CharacterController Controller { get; private set; }
    public Transform Target { get; private set; } // Игрок

    [Inject]
    public void Construct(Player.CombatComponent player)
    {
        // В платформерах часто ищут игрока через Zenject или тег
        // Для примера найдем по тегу или через инъекцию
    }

    protected virtual void Awake()
    {
        Controller = GetComponent<CharacterController>();
        StateMachine = new EnemyStateMachine();
    }

    protected virtual void Update()
    {
        StateMachine.Update();
    }

    public abstract void TakeDamage(float damage, Vector3 attackerPosition);
}