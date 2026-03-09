using UnityEngine;

public interface ICheckPointHolder
{
    void SetCheckpoint(Vector3 position, Quaternion rotation);
    Vector3 GetPosition();
    Quaternion GetRotation();
}
