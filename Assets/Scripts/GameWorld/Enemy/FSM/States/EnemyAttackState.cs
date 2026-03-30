using UnityEngine;

public class AttackState : IEnemyState
{
    private Enemy _enemy;
    private float _attackWindow = 0.5f; // Время "замаха"
    private float _timer;

    public AttackState(Enemy enemy) => _enemy = enemy;

    public void Enter()
    {
        _timer = _attackWindow;
        _enemy.Move(Vector3.zero); // Останавливаемся для удара
        _enemy.PerformAttack();
        Debug.Log("Враг атакует!");
    }

    public void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            // После удара возвращаемся в погоню
            _enemy.ChangeState(new EnemyChaseState(_enemy));
        }
    }

    public void Exit() { }
}