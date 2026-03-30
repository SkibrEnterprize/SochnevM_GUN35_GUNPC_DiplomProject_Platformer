using UnityEngine;

public class EnemyDeathState : IEnemyState
{
    private readonly Enemy _enemy;
    private float _despawnDelay = 3f; // Время до полного удаления объекта из сцены

    public EnemyDeathState(Enemy enemy) => _enemy = enemy;

    public void Enter()
    {
        Debug.Log("Враг повержен!");

        // 1. Останавливаем движение
        _enemy.Move(Vector3.zero);

        // 2. Отключаем компоненты, чтобы враг не мешал игроку ходить сквозь него
        if (_enemy.TryGetComponent<CharacterController>(out var controller))
        {
            Debug.Log("Controller!!!");
            controller.enabled = false;
        }

        // 3. Если есть визуальный эффект или анимация смерти — запускаем тут
        // _enemy.GetComponent<Animator>().SetTrigger("Die");

        // 4. Опционально: удаляем объект через несколько секунд
        Object.Destroy(_enemy.gameObject, _despawnDelay);
    }

    public void Update() { /* В смерти ничего не делаем */ }

    public void Exit() { /* Из смерти выхода обычно нет */ }
}