using UnityEngine;

public class EnemyDeathState : IEnemyState
{
    private readonly Enemy _enemy;
    private float _despawnDelay = 5f; // Сколько тело лежит до исчезновения

    public EnemyDeathState(Enemy enemy) => _enemy = enemy;

    public void Enter()
    {
        _enemy.StartDespawn(_despawnDelay);
        _enemy.SetBool("IsDead", true);

        _enemy.Move(Vector3.zero);

        if (_enemy.TryGetComponent<CharacterController>(out var controller))
        {
            controller.enabled = false;
        }

        _enemy.enabled = false;

        Object.Destroy(_enemy.gameObject, _despawnDelay);
    }

    public void Update() { }
    public void Exit() { }
}