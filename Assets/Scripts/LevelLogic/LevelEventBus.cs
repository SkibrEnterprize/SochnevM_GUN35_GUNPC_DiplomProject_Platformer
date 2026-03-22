using System;
public class LevelEventBus : ILevelEventBus
{
    public event Action OnLevelFinished;
    public event Action OnEndPointReached;
    public void FinishLevel() => OnLevelFinished?.Invoke();
    public void ReachEndPoint() => OnEndPointReached?.Invoke();
    
}