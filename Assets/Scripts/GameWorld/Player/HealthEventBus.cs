using System;
using Zenject;

public class HealthEventBus : IHealthEventBus
{
    public event Action OnHealthIsOver;
    public event Action OnDeathAnimationComplete;
    public event Action<int> OnHealthUpdated;

    public void HealthUpdated(int value) => OnHealthUpdated?.Invoke(value);
    public void HealthIsOver() => OnHealthIsOver?.Invoke();

    public void DeathAnimationCompleted() => OnDeathAnimationComplete?.Invoke();
}
