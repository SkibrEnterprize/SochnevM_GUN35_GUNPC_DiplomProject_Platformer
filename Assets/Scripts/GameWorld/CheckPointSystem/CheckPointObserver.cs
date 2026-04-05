using Player;
using System;
using UnityEngine;
using Zenject;

public class CheckPointObserver : IInitializable, IDisposable
{
    private ICheckPointEventBus _checkPointEventBus;
    private PlayerMovementSystem _movementComponent;
    private ISoundEventBus _soundBus;

    private Vector3 _position;
    public CheckPointObserver(PlayerMovementSystem movementComponent,
        ISoundEventBus soundBus,
        ICheckPointEventBus checkPointEventBus)
    {
        _movementComponent = movementComponent;
        _soundBus = soundBus;
        _checkPointEventBus = checkPointEventBus;
    }
    public void Initialize()
    {
        _checkPointEventBus.OnCheckPointReached += SetCheckPoint;
        _checkPointEventBus.OnCheckPointUse += MoveToCheckPoint;
    }


    private void SetCheckPoint(Vector3 position, Quaternion quaternion)
    {
        _position = position;
        _soundBus.Play(SoundType.CheckPointReached);
    }
    private void MoveToCheckPoint()
    {
        _movementComponent.MoveToCheckPoint(_position);
        _soundBus.Play(SoundType.CheckPointUse);
    }

    public void Dispose()
    {
        _checkPointEventBus.OnCheckPointReached -= SetCheckPoint;
        _checkPointEventBus.OnCheckPointUse -= MoveToCheckPoint;
    }
}
