using System;
using Zenject;

public class CheckPointObserver : IInitializable, IDisposable
{
    private readonly ILevelEventBus _levelBus;
    private readonly ISoundEventBus _soundBus;
    public CheckPointObserver(ILevelEventBus levelBus, ISoundEventBus soundBus)
    {
        _levelBus = levelBus;
        _soundBus = soundBus;
    }
    public void Initialize()
    {
        _levelBus.OnEndPointReached += HandleEndPointReached;
    }
    public void Dispose()
    {
        _levelBus.OnEndPointReached -= HandleEndPointReached;
    }

    private void HandleEndPointReached()
    {        
        _soundBus.Play(SoundType.EndPoint);
    }

}
