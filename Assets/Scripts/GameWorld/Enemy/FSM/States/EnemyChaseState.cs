using UnityEngine;

public class ChaseState : IEnemyState
{
    private Enemy _enemy;
    private Transform _player;

    public ChaseState(Enemy enemy)
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

        // Вычисляем направление к игроку
        Vector3 direction = (_player.position - _enemy.transform.position).normalized;
        direction.y = 0; // Нам нужно только влево/вправо

        // Разворот в сторону игрока
        float angle = _player.position.x > _enemy.transform.position.x ? 0 : 180;
        _enemy.transform.rotation = Quaternion.Euler(0, angle, 0);

        _enemy.Move(direction);

        if (_enemy.CanAttackPlayer())
        {
            // Здесь будет переход в AttackState
            Debug.Log("АТАКА!");
        }

        // Проверка препятствий перед движением
        bool canMove = _enemy.IsGroundAhead() && !_enemy.IsWallAhead();

        if (canMove)
        {
            // Если путь чист — бежим к игроку
            _enemy.Move(_enemy.transform.right);
        }
        else
        {
            // Если впереди стена или обрыв — замираем у края/стены
            _enemy.Move(Vector3.zero);
            Debug.Log("Препятствие! Не могу преследовать дальше.");
        }
    }

    public void Exit() { }
}