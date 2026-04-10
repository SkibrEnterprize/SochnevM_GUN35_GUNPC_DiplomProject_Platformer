using Player;
using System;
using UnityEngine;
using Zenject;
public class PlayerDeathObserver : IInitializable, IDisposable
{
    private HealthModel _healthModel;
    private IHealthEventBus _healthEventBus;
    private ISoundEventBus _soundEventBus;
    private ICheckPointEventBus _checkPointEventBus;
    private readonly PlayerAnimator _playerAnimator;
    private readonly CharacterController _controller;
    private readonly PlayerMovementSystem _playerMovementSystem;


    public PlayerDeathObserver(HealthModel healthModel,
        IHealthEventBus healthEventBus,
        ISoundEventBus soundEventBus,
        ICheckPointEventBus checkPointEventBus,
        PlayerAnimator playerAnimator,
        CharacterController controller,
        PlayerMovementSystem playerMovementSystem)
    {
        _healthModel = healthModel;
        _healthEventBus = healthEventBus;
        _soundEventBus = soundEventBus;
        _checkPointEventBus = checkPointEventBus;
        _playerAnimator = playerAnimator;
        _controller = controller;
        _playerMovementSystem = playerMovementSystem;
    }
    public void Initialize()
    {
        _healthEventBus.OnHealthIsOver += StartDeathProcess;
        _healthEventBus.OnDeathAnimationComplete += FinalizeDeath;
    }

    public void Dispose()
    {
        _healthEventBus.OnHealthIsOver -= StartDeathProcess;
        _healthEventBus.OnDeathAnimationComplete -= FinalizeDeath;
    }
    private void StartDeathProcess()
    {
        _controller.enabled = false;
        _playerMovementSystem.StopImmediately();
        _playerAnimator.PlayDeath();
        _soundEventBus.Play(SoundType.Die);
    }

    private void FinalizeDeath()
    {
        MoveToLastCheckPoint();
        _playerMovementSystem.StopImmediately();
        HealthRepair();
        ResetAnimation();
        _controller.enabled = true;
        _playerMovementSystem.IsMovementFrozen = false;
        Debug.Log("FINALIZE");
    }

    private void HealthRepair() => _healthModel.HealthAllRepair();

    public void MoveToLastCheckPoint() => _checkPointEventBus.CheckPointUse();
    private void ResetAnimation() => _playerAnimator.ResetToIdle();
}
