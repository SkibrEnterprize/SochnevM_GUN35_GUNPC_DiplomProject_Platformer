using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private readonly Enemy _enemy;
    private bool _movingRight = true;

    public EnemyPatrolState(Enemy enemy) => _enemy = enemy;

    public void Enter() => Debug.Log("Начало патрулирования");
    public void Exit() { }

    public void Update()
    {
        bool wall = _enemy.IsWallAhead();
        bool ground = _enemy.IsGroundAhead();


        if (wall || !ground)
        {
            Flip();
        }
        _enemy.Move(_enemy.transform.right);

        if (_enemy.CanSeePlayer())
        {
            if (_enemy.EnemyTypeAttack == EnemyTypeAttack.CloseAttack)
            {
                _enemy.ChangeState(new EnemyChaseState(_enemy));
                Debug.Log("Change state to chase");
            }
            else if (_enemy.EnemyTypeAttack == EnemyTypeAttack.RangeAttack)
            {
                _enemy.ChangeState(new EnemyRangedAttackState(_enemy));
            }
        }
    }

    private void Flip()
    {
        _movingRight = !_movingRight;

        float targetY = _movingRight ? 0f : -180f;
        _enemy.transform.rotation = Quaternion.Euler(0, targetY, 0);
    }
}