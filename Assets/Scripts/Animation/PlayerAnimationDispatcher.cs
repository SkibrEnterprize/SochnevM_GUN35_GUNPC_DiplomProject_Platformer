using Player;
using UnityEngine;
using Zenject;

public class PlayerAnimationDispatcher : MonoBehaviour
{
    private IHealthEventBus _healthEventBus;
    private PlayerMovementSystem _playerMovementSystem;

    [Inject]
    public void Construct(IHealthEventBus healthEventBus, PlayerMovementSystem playerMovementSystem)
    {
        _healthEventBus = healthEventBus;
        _playerMovementSystem = playerMovementSystem;
    }
    public void OnDeathAnimationFinished()
    {
        Debug.Log("Animation Event: Death Finished!");
        _healthEventBus.DeathAnimationCompleted();
    }

    public void OnAttackStart() => _playerMovementSystem.SetMovementLock(true);
    public void OnAttackEnding() => _playerMovementSystem.SetMovementLock(false);
    public void OnHitStart() => _playerMovementSystem.SetMovementLock(true);
    public void OnHitEnding() => _playerMovementSystem.SetMovementLock(false);
}