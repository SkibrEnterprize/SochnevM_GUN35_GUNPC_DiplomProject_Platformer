using UnityEngine;

public class CheckPointHolder : ICheckPointHolder
{
    private Vector3 _position;
    private Quaternion _rotation;

    public CheckPointHolder(Vector3 position, Quaternion rotation)
    {
        _position = position;
        _rotation = rotation;
    }

    public void SetCheckpoint(Vector3 position, Quaternion rotation)
    {
        _position = position;
        _rotation = rotation;
        Debug.Log($"[CheckpointService] Сохранено: {_position}");
    }

    public Vector3 GetPosition() => _position;
    public Quaternion GetRotation() => _rotation;
}
