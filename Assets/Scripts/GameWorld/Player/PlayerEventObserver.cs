using Player;
using System;
using Zenject;


public class PlayerEventObserver : IInitializable, IDisposable
{
    private HealthModel _healthModel;
    private PlayerDeathHandler _playerDeathHandler;
    private MovementComponent _movementComponent;


    public PlayerEventObserver(HealthModel healthModel,
                                PlayerDeathHandler playerDeathHandler,                                
                                MovementComponent movementComponent)
    {
        _healthModel = healthModel;
        _playerDeathHandler = playerDeathHandler;
        _movementComponent = movementComponent;
    }
    public void Initialize()
    {
        _movementComponent.OnFallDistanceEvent += TakeDamageWhenFalling;
        _healthModel.OnHealthIsOver += PlayerDieEvents;
    }


    public void Dispose()
    {
        _movementComponent.OnFallDistanceEvent -= TakeDamageWhenFalling;
        _healthModel.OnHealthIsOver -= PlayerDieEvents;
    }

    private void TakeDamageWhenFalling(float distance)
    {
        _healthModel.FallDistanceReceived(distance);
    }
    private void PlayerDieEvents()
    {
        _playerDeathHandler.PlayerDied();
        PlayerSpawnWithHealing();
    }

    private void PlayerSpawnWithHealing()
    {
        _playerDeathHandler.MoveToLastCheckPoint();
        _healthModel.HealthAllRepair();
    }
}

