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

    public void Enter() => Debug.Log("Дальний бой активирован!");

    public void Update()
    {
        // 1. Если потеряли игрока из виду — возвращаемся в патруль
        if (!_enemy.CanSeePlayer())
        {
            _enemy.ChangeState(new EnemyPatrolState(_enemy));
            return;
        }

        // 2. Всегда поворачиваемся к игроку
        float targetAngle = _player.position.x > _enemy.transform.position.x ? 0 : 180;
        _enemy.transform.rotation = Quaternion.Euler(0, targetAngle, 0);

        // 3. Стреляем по кулдауну
        if (_enemy.CanAttackReady())
        {
            _enemy.Shoot();
        }

        // Враг стоит на месте во время стрельбы
        _enemy.Move(Vector3.zero);
    }

    public void Exit() { }
}