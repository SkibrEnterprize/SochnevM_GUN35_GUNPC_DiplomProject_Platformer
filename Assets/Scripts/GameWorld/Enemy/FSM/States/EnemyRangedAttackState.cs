using UnityEngine;

public class EnemyRangedAttackState : IEnemyState
{
    private readonly Enemy _enemy;
    private readonly Transform _player;

    public EnemyRangedAttackState(Enemy enemy)
    {
        _enemy = enemy;
        _player = enemy.GetPlayer();
    }

    public void Enter()
    {
        _enemy.SetTrigger("RangeAttack");
    }

    public void Update()
    {
        if (!_enemy.CanSeePlayer())
        {
            _enemy.ChangeState(new EnemyPatrolState(_enemy));
            return;
        }

        float targetAngle = _player.position.x > _enemy.transform.position.x ? 0 : 180;
        _enemy.transform.rotation = Quaternion.Euler(0, targetAngle, 0);

        if (_enemy.CanAttackReady())
        {
            if (_enemy.HasClearLineOfSight())
            {
                _enemy.SetTrigger("RangeAttack");
                _enemy.Shoot();
            }
        }

        _enemy.Move(Vector3.zero);
    }

    public void Exit() { }
    
}
