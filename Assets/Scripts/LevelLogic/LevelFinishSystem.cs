using System;
using Zenject;
public class LevelFinishSystem : IInitializable, IDisposable
{
    private readonly LevelEventBus _levelEventBus;
    private readonly LevelFinishConfig _levelFinishConfig;
    private int _finishCount = 0;

    public LevelFinishSystem(LevelFinishConfig levelFinishConfig,
        LevelEventBus levelEventBus)
    {        
        _levelFinishConfig = levelFinishConfig;
        _levelEventBus = levelEventBus;
    }

    public void Initialize()
    {
    }
    public void Dispose()
    {
    }

    public void EndPointReached()
    {
        if (_finishCount < _levelFinishConfig.CollectObjectsForGoal)
        {
            _finishCount++;
            _levelEventBus.ReachEndPoint();            
        }
        else
        {
            _levelEventBus.FinishLevel();
        }
    }
}
