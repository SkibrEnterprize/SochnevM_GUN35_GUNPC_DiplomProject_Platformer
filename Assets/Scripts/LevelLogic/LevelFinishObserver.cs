using System;
using Zenject;
using UnityEngine;

public class LevelFinishObserver : IInitializable, IDisposable
{
    private readonly ILevelEventBus _levelBus;
    private readonly ISoundEventBus _soundBus;
    public LevelFinishObserver(ILevelEventBus levelBus, ISoundEventBus soundBus)
    {
        _levelBus = levelBus;
        _soundBus = soundBus;
    }
    public void Initialize()
    {
        _levelBus.OnLevelFinished += HandleLevelFinished;
    }
    public void Dispose()
    {
        _levelBus.OnLevelFinished -= HandleLevelFinished;
    }   

    private void HandleLevelFinished()
    {
        _soundBus.Play(SoundType.Finish);
    }


}
