using UnityEngine;

public class EnemyDeathState : IEnemyState
{
    private readonly Enemy _enemy;
    private float _despawnDelay = 3f; // Время до полного удаления объекта из сцены

    public EnemyDeathState(Enemy enemy) => _enemy = enemy;

    public void Enter()
    {
        Debug.Log("Враг повержен!");

        _enemy.Move(Vector3.zero);

        if (_enemy.TryGetComponent<CharacterController>(out var controller))
        {
            Debug.Log("Controller!!!");
            controller.enabled = false;
        }

        Object.Destroy(_enemy.gameObject, _despawnDelay);
    }

    public void Update() { }

    public void Exit() { }
}