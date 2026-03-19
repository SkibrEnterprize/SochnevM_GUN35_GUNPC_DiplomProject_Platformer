using Player;
using System;
using Zenject;


public class PlayerEventObserver : IInitializable, IDisposable
{
    private PlayerDeathHandler _playerDeathHandler;
    private HealthModel _healthModel;
    private MovementComponent _movementComponent;


    public PlayerEventObserver(PlayerDeathHandler playerDeathHandler,
                                HealthModel healthModel,
                                MovementComponent movementComponent)
    {
        _playerDeathHandler = playerDeathHandler;
        _healthModel = healthModel;
        _movementComponent = movementComponent;
    }
    public void Initialize()
    {
        _movementComponent.OnFallDistanceEvent += _healthModel.FallDistanceReceived;
        _healthModel.OnHealthIsOver += _playerDeathHandler.PlayerDied;
        _playerDeathHandler.OnHealthRepair += _healthModel.HealthAllRepair;
    }
    public void Dispose()
    {
        _movementComponent.OnFallDistanceEvent -= _healthModel.FallDistanceReceived;
        _healthModel.OnHealthIsOver -= _playerDeathHandler.PlayerDied;
        _playerDeathHandler.OnHealthRepair -= _healthModel.HealthAllRepair;
    }
}

