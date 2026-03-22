using System;
using UnityEngine;

public class CheckPointEventBus : ICheckPointEventBus
{
    public event Action<Vector3, Quaternion> OnCheckPointReached;
    public event Action OnCheckPointUse;

    public void CheckPointUse() => OnCheckPointUse?.Invoke();

    public void CheckPointReached(Vector3 position, Quaternion rotation) 
        => OnCheckPointReached?.Invoke(position, rotation);

}
