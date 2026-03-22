using System;

public interface IHealthEventBus
{    
    event Action OnHealthIsOver;
    event Action<int> OnHealthUpdated;

    void HealthUpdated(int value);
    void HealthIsOver();
}

