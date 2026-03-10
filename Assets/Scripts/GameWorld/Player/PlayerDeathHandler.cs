
using Player.Signals;
using System;
using Zenject;
using UnityEngine;


public class PlayerDeathHandler : IInitializable, IDisposable
{
    [SerializeField] private Transform _playerTransform;
    private CheckPointHolder _checkPointHolder;
    private SignalBus _signalBus;

    public PlayerDeathHandler(CheckPointHolder chechPointHolder, Transform playerTransform,
        SignalBus signalBus)
    {
        _checkPointHolder = chechPointHolder;
        _playerTransform = playerTransform;
        _signalBus = signalBus;
    }
    public void Initialize()
    {
        _signalBus.Subscribe<HealthIsOverSignal>(OnPlayerDied);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<HealthIsOverSignal>(OnPlayerDied);
    }


    public void OnPlayerDied()
    {
        MoveToLastCheckPoint();
        _signalBus.Fire<HealthIsRepairSignal>();
    }

    private void MoveToLastCheckPoint()
    {
        Vector3 pos = _checkPointHolder.GetPosition();
        Quaternion rot = _checkPointHolder.GetRotation();

        if (pos != default && rot != default)
        {
            _playerTransform.position = pos;
            _playerTransform.rotation = rot;
        }
    }
}
