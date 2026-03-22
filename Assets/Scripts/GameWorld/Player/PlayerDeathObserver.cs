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


    public PlayerDeathObserver(HealthModel healthModel,
        IHealthEventBus healthEventBus,
        ISoundEventBus soundEventBus,
        ICheckPointEventBus checkPointEventBus)
    {
        _healthModel = healthModel;
        _healthEventBus = healthEventBus;
        _soundEventBus = soundEventBus;
        _checkPointEventBus = checkPointEventBus;
    }
    public void Initialize()
    {
        _healthEventBus.OnHealthIsOver += PlayerDied;
    }

    public void Dispose()
    {
        _healthEventBus.OnHealthIsOver -= PlayerDied;
    }
    public void PlayerDied()
    {
        _soundEventBus.Play(SoundType.Die);
        Debug.Log("Plaer Died activate");
        MoveToLastCheckPoint();
    }

    public void MoveToLastCheckPoint()
    {
        _checkPointEventBus.CheckPointUse();
        _healthModel.HealthAllRepair();
    }
}
