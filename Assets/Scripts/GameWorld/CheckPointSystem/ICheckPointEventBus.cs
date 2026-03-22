
using System;
using UnityEngine;

public interface ICheckPointEventBus
{
    public event Action<Vector3, Quaternion> OnCheckPointReached;
    public event Action OnCheckPointUse;
    void CheckPointReached(Vector3 point, Quaternion rotation);
    void CheckPointUse();
}
