using UnityEngine;

public class EnemyHitState : IEnemyState
{
    private Enemy _enemy;
    private float _stunDuration = 0.4f;
    private float _timer;

    public EnemyHitState(Enemy enemy, Vector3 pushDir) => _enemy = enemy;

    public void Enter()
    {
        _timer = _stunDuration;
        _enemy.ApplyKnockback(Vector3.up + Vector3.back, 5f); // импульс
        Debug.Log("Враг в ступоре!");
    }

    public void Update()
    {
        _timer -= Time.deltaTime;
        _enemy.Move(Vector3.zero); // Только гравитация и импульс работают внутри Move

        if (_timer <= 0)
        {
            _enemy.ChangeState(new ChaseState(_enemy));
        }
    }

    public void Exit() { }
}