using UnityEngine;
using Zenject;

public class CheckPointModel : IInitializable
{
    private ISoundEventBus _soundBus;
    private ICheckPointEventBus _checkPointEventBus;

    public CheckPointModel(ISoundEventBus soundEventBus,
        ICheckPointEventBus checkPointEventBus)
    {
        _soundBus = soundEventBus;
        _checkPointEventBus = checkPointEventBus;
    }
    public void Initialize()
    {
    }
    public void SetCheckpoint(Vector3 position, Quaternion rotation)
    {
        _checkPointEventBus.CheckPointReached(position, rotation);
    }
}
