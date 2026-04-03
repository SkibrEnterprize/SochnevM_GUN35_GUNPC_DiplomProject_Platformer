using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private readonly Enemy _enemy;
    private readonly Transform _player;

    public EnemyChaseState(Enemy enemy)
    {
        _enemy = enemy;
        _player = enemy.GetPlayer();
    }

    public void Enter() => Debug.Log("Начинаю погоню!");

    public void Update()
    {
        if (_player == null || Vector3.Distance(_enemy.transform.position, _player.position) > _enemy.LostRange)
        {
            _enemy.ChangeState(new EnemyPatrolState(_enemy));
            return;
        }

        float targetAngle = _player.position.x > _enemy.transform.position.x ? 0 : 180;
        _enemy.transform.rotation = Quaternion.Euler(0, targetAngle, 0);

        if (_enemy.CanAttackPlayer())
        {
            if (_enemy.CanAttackReady())
            {
                _enemy.ChangeState(new AttackState(_enemy));
                return;
            }
            else
            {
                _enemy.Move(Vector3.zero);
                return;
            }
        }

        bool canMove = _enemy.IsGroundAhead() && !_enemy.IsWallAhead();

        if (canMove)
        {
            _enemy.Move(_enemy.transform.right);
        }
        else
        {
            _enemy.Move(Vector3.zero);
        }
    }

    public void Exit() { }
}