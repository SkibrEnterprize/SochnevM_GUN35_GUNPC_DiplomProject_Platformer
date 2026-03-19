using Player.Signals;
using System;
using Zenject;
using UnityEngine;

public class LevelFinishSystem : IInitializable, IDisposable
{
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
            Debug.Log($"finishCount = {_finishCount}");
        }
        else
        {
            Debug.Log("FINISH!!!!");
        }
    }
}
