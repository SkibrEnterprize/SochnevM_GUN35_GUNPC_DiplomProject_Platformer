using Player;
using System;
using Zenject;


public class PlayerEventObserver : IInitializable, IDisposable
{
    private HealthModel _healthModel;
    private PlayerDeathObserver _playerDeathHandler;
    private MovementComponent _movementComponent;


    public PlayerEventObserver(HealthModel healthModel,
                                PlayerDeathObserver playerDeathHandler,                                
                                MovementComponent movementComponent)
    {
        _healthModel = healthModel;
        _playerDeathHandler = playerDeathHandler;
        _movementComponent = movementComponent;
    }
    public void Initialize()
    {
        _movementComponent.OnFallDistanceEvent += TakeDamageWhenFalling;
    }


    public void Dispose()
    {
        _movementComponent.OnFallDistanceEvent -= TakeDamageWhenFalling;
    }

    private void TakeDamageWhenFalling(float distance)
    {
        _healthModel.FallDistanceReceived(distance);
    }    
}

