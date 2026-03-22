using System;

public interface ILevelEventBus
{
    event Action OnLevelFinished;
    event Action OnEndPointReached;
    void FinishLevel();
    void ReachEndPoint();
}