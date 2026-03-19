using UnityEngine;

public class CheckPointHolder : ICheckPointHolder
{
    private Vector3 _position;
    private Quaternion _rotation;
    private SoundLibrary _soundLibrary;

    public CheckPointHolder(Vector3 position, Quaternion rotation, SoundLibrary soundLibrary)
    {
        _position = position;
        _rotation = rotation;
        _soundLibrary = soundLibrary;
    }

    public void SetCheckpoint(Vector3 position, Quaternion rotation)
    {
        _position = position;
        _rotation = rotation;
        _soundLibrary.RequestPlay(SoundType.CheckPoint);
        Debug.Log($"[CheckpointService] Сохранено: {_position}");
    }

    public Vector3 GetPosition() => _position;
    public Quaternion GetRotation() => _rotation;
}
