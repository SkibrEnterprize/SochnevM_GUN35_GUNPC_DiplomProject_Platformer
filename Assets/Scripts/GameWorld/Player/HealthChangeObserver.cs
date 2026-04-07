using Player;
using System;
using Zenject;

public class HealthChangeObserver : IInitializable, IDisposable
{
    private IHealthEventBus _healthEventBus;
    private ISoundEventBus _soundEventBus;

    public HealthChangeObserver(HealthModel healthModel,
        IHealthEventBus healthEventBus,
        ISoundEventBus soundEventBus,
        ICheckPointEventBus checkPointEventBus)
    {
        _healthEventBus = healthEventBus;
        _soundEventBus = soundEventBus;
    }
    public void Initialize()
    {
        _healthEventBus.OnHealthUpdated += HealthUpdatedHandler;
    }
    public void Dispose()
    {
        _healthEventBus.OnHealthUpdated -= HealthUpdatedHandler;
    }

    private void HealthUpdatedHandler(int obj)
    {
        _soundEventBus.Play(obj > 0 ? SoundType.Healing : SoundType.Hit);
    }
}
