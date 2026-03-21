using System;
using UnityEngine;
using Zenject;

public class LevelFinishSystem : IInitializable, IDisposable
{
    //public event Action OnEndPointReached;
    //public event Action OnLevelFinished;

    private readonly LevelEventBus _eventBus;
    private readonly LevelFinishConfig _levelFinishConfig;
    private int _finishCount = 0;

    public LevelFinishSystem(LevelFinishConfig levelFinishConfig,
        LevelEventBus levelEventBus)
    {        
        _levelFinishConfig = levelFinishConfig;
        _eventBus = levelEventBus;
    }

    public void Initialize()
    {
    }
    public void Dispose()
    {
    }

    public void EndPointReached()
    {
        if (_finishCount < _levelFinishConfig.CollectObjectsForGoal)
        {
            _finishCount++;
            _eventBus.ReachEndPoint();            
        }
        else
        {
            _eventBus.FinishLevel();
        }
    }
}
