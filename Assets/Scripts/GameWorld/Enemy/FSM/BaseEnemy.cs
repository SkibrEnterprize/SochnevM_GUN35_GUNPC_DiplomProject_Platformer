using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IHealthAffected
{
    [Header("Settings")]
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;

    protected EnemyStateMachine StateMachine;
       
    public CharacterController Controller { get; private set; }
    public Transform Target { get; private set; } 
    

    protected virtual void Awake()
    {
        Controller = GetComponent<CharacterController>();
        StateMachine = new EnemyStateMachine();
    }

    protected virtual void Update()
    {
        StateMachine.Update();
    }
    public abstract void ApplyHealthChange(int delta, Vector3 sourcePosition = default);
    
}