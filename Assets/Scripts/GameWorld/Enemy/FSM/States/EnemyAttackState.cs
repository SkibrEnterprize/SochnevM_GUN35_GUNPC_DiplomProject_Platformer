using UnityEngine;

public class AttackState : IEnemyState
{
    private Enemy _enemy;
    private float _attackWindow = 0.5f;
    private float _timer;

    public AttackState(Enemy enemy) => _enemy = enemy;

    public void Enter()
    {
        _timer = _attackWindow;
        _enemy.SetTrigger("Attack");
        _enemy.Move(Vector3.zero);
        _enemy.PerformAttack();
        Debug.Log("Враг атакует!");
    }

    public void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _enemy.ChangeState(new EnemyChaseState(_enemy));
        }
    }

    public void Exit() { }
   
}