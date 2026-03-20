using System;
using Zenject;
using UnityEngine;

public class LevelFinishSystem : IInitializable, IDisposable
{
    public event Action OnEndPointReached;
    public event Action OnLevelFinished;

    private readonly LevelFinishConfig _levelFinishConfig;
    private int _finishCount = 0;

    public LevelFinishSystem(LevelFinishConfig levelFinishConfig)
    {        
        _levelFinishConfig = levelFinishConfig;
    }

    public void Initialize()
    {
    }
    public void Dispose()
    {
    }

    public void EndPointReached()
    {
        if (_levelFinishConfig.CollectObjectForGoal > _finishCount)
        {
            _finishCount++;
            OnEndPointReached?.Invoke();
        }
        else
        {
            OnLevelFinished?.Invoke();
        }
    }
}
