using UnityEngine;

public class EnemySearchState : IEnemyState
{
    private readonly Enemy _enemy;
    private readonly Vector3 _targetPos;

    private float _searchTimer = 2.5f; // Âðåìÿ îæèäàíèÿ íà òî÷êå
    private bool _hasReachedDestination = false;

    public EnemySearchState(Enemy enemy, Vector3 targetPos)
    {
        _enemy = enemy;
        _targetPos = targetPos;
    }

    public void Enter()
    {
        Debug.Log("Враг: Потерял цель, проверяю последнюю позицию...");
    }

    public void Update()
    {
        if (_enemy.CanSeePlayer())
        {
            _enemy.ChangeState(new EnemyChaseState(_enemy));
            return;
        }

        float distanceX = Mathf.Abs(_enemy.transform.position.x - _targetPos.x);

        if (!_hasReachedDestination && distanceX > 0.5f)
        {
            _enemy.RotateTowards(_targetPos);
            _enemy.Move(_enemy.transform.right);

            if (_enemy.IsGroundAhead() && !_enemy.IsObstacleAhead())
            {
                _enemy.Move(_enemy.transform.right);
            }
            else
            {
                _hasReachedDestination = true;
            }
        }
        else
        {
            _hasReachedDestination = true;
            _enemy.Move(Vector3.zero);

            _searchTimer -= Time.deltaTime;

            if (_searchTimer <= 0)
            {
                // Время вышло, игрок не найден — возвращаемся к патрулю
                _enemy.ChangeState(new EnemyPatrolState(_enemy));
            }
        }
    }

    public void Exit()
    {
        Debug.Log("Враг: Поиск завершен.");
    }
}