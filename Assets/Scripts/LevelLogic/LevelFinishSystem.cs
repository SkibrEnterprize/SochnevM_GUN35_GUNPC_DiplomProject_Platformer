public class LevelFinishSystem
{
    private readonly ILevelEventBus _levelEventBus;
    private readonly LevelFinishConfig _config;
    private int _finishCount = 0;

    public LevelFinishSystem(LevelFinishConfig config, ILevelEventBus levelEventBus)
    {
        _config = config;
        _levelEventBus = levelEventBus;
    }

    public void EndPointReached()
    {
        _finishCount++;
        // Проверяем достижение цели из нашего ScriptableObject
        if (_finishCount >= _config.CollectObjectsForGoal)
            _levelEventBus.FinishLevel();
        else
            _levelEventBus.ReachEndPoint();
    }
}
