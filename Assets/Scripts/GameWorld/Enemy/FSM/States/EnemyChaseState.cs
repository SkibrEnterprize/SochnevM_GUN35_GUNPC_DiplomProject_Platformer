using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private readonly Enemy _enemy;
    private readonly Transform _player;
    private readonly float _runMultiplier = 2.0f;

    public EnemyChaseState(Enemy enemy)
    {
        _enemy = enemy;
        _player = enemy.GetPlayer();
    }

    public void Enter() => Debug.Log("Начинаю погоню! Бегу к цели.");

    public void Update()
    {
        if (_player == null) return;

        float distanceX = Mathf.Abs(_player.position.x - _enemy.transform.position.x);
        float distanceY = Mathf.Abs(_player.position.y - _enemy.transform.position.y);
        float distanceTotal = Vector3.Distance(_enemy.transform.position, _player.position);

        if (!_enemy.CanSeePlayer() || distanceTotal > _enemy.LostRange)
        {
            _enemy.ChangeState(new EnemySearchState(_enemy, _player.position));
            return;
        }

        if (distanceY > 1.2f)
        {
            _enemy.Move(Vector3.zero); // Стоим в Idle
            return;
        }

        if (distanceX > 0.5f)
        {
            _enemy.RotateTowards(_player.position);
        }

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

        bool canMoveForward = _enemy.IsGroundAhead() && !_enemy.IsObstacleAhead();

        if (canMoveForward)
        {
            _enemy.Move(_enemy.transform.right, _runMultiplier);
        }
        else
        {
            // Если впереди тупик — стоим и смотрим на игрока
            _enemy.Move(Vector3.zero);
        }
    }

    public void Exit() { }
}