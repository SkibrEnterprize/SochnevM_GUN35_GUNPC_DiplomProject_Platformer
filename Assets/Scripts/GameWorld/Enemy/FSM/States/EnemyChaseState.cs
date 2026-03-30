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
        // 1. Проверка потери цели (игрок слишком далеко)
        if (_player == null || Vector3.Distance(_enemy.transform.position, _player.position) > _enemy.LostRange)
        {
            _enemy.ChangeState(new EnemyPatrolState(_enemy));
            return;
        }

        // 2. Поворот в сторону игрока (всегда смотрим на цель)
        float targetAngle = _player.position.x > _enemy.transform.position.x ? 0 : 180;
        _enemy.transform.rotation = Quaternion.Euler(0, targetAngle, 0);

        // 3. Проверка на атаку (дистанция + кулдаун)
        if (_enemy.CanAttackPlayer())
        {
            if (_enemy.CanAttackReady())
            {
                _enemy.ChangeState(new AttackState(_enemy));
                return;
            }
            else
            {
                // Если дистанция для атаки есть, но кулдаун не прошел — просто стоим и ждем
                _enemy.Move(Vector3.zero);
                return;
            }
        }

        // 4. Логика движения к игроку
        bool canMove = _enemy.IsGroundAhead() && !_enemy.IsWallAhead();

        if (canMove)
        {
            // Двигаемся вперед (в ту сторону, куда развернуты через rotation)
            _enemy.Move(_enemy.transform.right);
        }
        else
        {
            // Если впереди стена или обрыв — останавливаемся
            _enemy.Move(Vector3.zero);
        }
    }

    public void Exit() { }
}