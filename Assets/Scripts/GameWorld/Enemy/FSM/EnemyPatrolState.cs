using UnityEngine;

public class PatrolState : IEnemyState
{
    private readonly Enemy _enemy;
    private bool _movingRight = true;

    public PatrolState(Enemy enemy) => _enemy = enemy;

    public void Enter() => Debug.Log("Начало патрулирования");
    public void Exit() { }

    public void Update()
    {
        bool wall = _enemy.IsWallAhead();
        bool ground = _enemy.IsGroundAhead();

        Debug.Log($"Wall: {wall}, Ground: {ground}"); 

        if (wall || !ground)
        {
            Flip();
        }
        _enemy.Move(_enemy.transform.right);
    }

    private void Flip()
    {
        _movingRight = !_movingRight;

        // Разворачиваем врага на 180 градусов
        float targetY = _movingRight ? 0f : -180f; // Подставь свои углы (0/180 или 90/-90)
        _enemy.transform.rotation = Quaternion.Euler(0, targetY, 0);
    }
}