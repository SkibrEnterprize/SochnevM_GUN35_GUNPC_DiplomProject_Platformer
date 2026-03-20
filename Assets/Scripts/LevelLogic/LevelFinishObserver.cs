using System;
using Zenject;
using UnityEngine;

public class LevelFinishObserver : IInitializable, IDisposable
{
    private readonly LevelFinishSystem _levelFinishSystem;
    private SoundLibrary _soundLibrary;

    public LevelFinishObserver(LevelFinishSystem levelFinishSystem, 
                                SoundLibrary soundLibrary)
    {
        _levelFinishSystem = levelFinishSystem;
    }
    public void Initialize()
    {
        _levelFinishSystem.OnEndPointReached += EndPointReached;
        _levelFinishSystem.OnLevelFinished += LevelFinished;
    }
    public void Dispose()
    {
        _levelFinishSystem.OnEndPointReached -= EndPointReached;
        _levelFinishSystem.OnLevelFinished -= LevelFinished;
    }
    private void EndPointReached()
    {
        Debug.Log("End Point Reached");
    }

    private void LevelFinished()
    {
        Debug.Log("Level Finish");
    }


}
