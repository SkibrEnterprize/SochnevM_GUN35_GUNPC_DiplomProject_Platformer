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
        //_position = position;
        //_rotation = rotation;
        //_soundBus.Play(SoundType.CheckPoint);
        ////_soundLibrary.RequestPlay(SoundType.CheckPoint);
        //Debug.Log($"[CheckpointService] Сохранено: {_position}");
    }

    //public Vector3 GetPosition() => _position;
    //public Quaternion GetRotation() => _rotation;

}
