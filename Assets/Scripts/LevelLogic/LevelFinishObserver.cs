using Player.Signals;
using System;
using Zenject;
using UnityEngine;

public class LevelFinishObserver : IInitializable, IDisposable, ILevelFinishObserver
{
    private readonly SignalBus _signalBus;
    private readonly LevelFinishConfig _levelFinishConfig;
    private int _finishCount = 0;


    public LevelFinishObserver(SignalBus signalBus,
        LevelFinishConfig levelFinishConfig)
    {        
        _signalBus = signalBus;
        _levelFinishConfig = levelFinishConfig;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<LevelFinishCollectSignal>(OnEndPointReached);
    }
    public void Dispose()
    {
        _signalBus.Unsubscribe<LevelFinishCollectSignal>(OnEndPointReached);
    }

    public void OnEndPointReached()
    {
        if (_levelFinishConfig.CollectObjectForGoal > _finishCount)
        {
            _finishCount++;
            Debug.Log($"finishCount = {_finishCount}");
        }
        else
        {
            Debug.Log("FINISH!!!!");
        }
    }
}
