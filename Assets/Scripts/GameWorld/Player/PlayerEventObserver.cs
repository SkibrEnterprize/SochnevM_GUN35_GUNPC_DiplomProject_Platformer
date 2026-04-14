using Player;
using System;
using Zenject;

public class PlayerEventObserver : IInitializable, IDisposable
{
    private HealthModel _healthModel;
    private PlayerMovementSystem _movementComponent;


    public PlayerEventObserver(HealthModel healthModel,                                                                
                                PlayerMovementSystem movementComponent)
    {
        _healthModel = healthModel;
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

