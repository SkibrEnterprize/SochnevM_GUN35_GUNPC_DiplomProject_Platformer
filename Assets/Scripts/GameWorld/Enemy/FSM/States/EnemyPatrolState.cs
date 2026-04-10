using UnityEngine;

public class EnemyPatrolState : IEnemyState
{

    private readonly Enemy _enemy;
    private bool _movingRight = true;

    public EnemyPatrolState(Enemy enemy) => _enemy = enemy;

    public void Enter() {}
    public void Exit() { }

    public void Update()
    {
        Vector3 targetDirection = _movingRight ? Vector3.right : Vector3.left;
        Vector3 lookAtPoint = _enemy.transform.position + targetDirection;

        _enemy.RotateTowards(lookAtPoint);

        float angleToTarget = Vector3.Angle(_enemy.transform.right, targetDirection);

        if (angleToTarget > 30f)
        {
            _enemy.Move(Vector3.zero);
            return;
        }

        bool wall = _enemy.IsObstacleAhead();
        bool ground = _enemy.IsGroundAhead();

        if (wall || !ground)
        {
            _movingRight = !_movingRight;
            _enemy.Move(Vector3.zero);
            return;
        }

        _enemy.Move(_enemy.transform.right);

        if (_enemy.CanSeePlayer())
        {
            if (_enemy.EnemyTypeAttack == EnemyTypeAttack.CloseAttack)
            {
                _enemy.ChangeState(new EnemyChaseState(_enemy));
            }
            else if (_enemy.EnemyTypeAttack == EnemyTypeAttack.RangeAttack)
            {
                _enemy.ChangeState(new EnemyRangedAttackState(_enemy));
            }
        }
    }
}
