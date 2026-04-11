using System;
using UnityEngine;

public class VFXEventBus
{
    public event Action<VFXType,
        Vector3,        
        Quaternion,
        Transform>
        OnVFXRequested;

public void Play(VFXType type,
    Vector3 position,
    float scaleMultiplier = 1f,
    Quaternion rotation = default,
    Transform parent = null)
{
    OnVFXRequested?.Invoke(type,
        position,        
        rotation,
        parent);
}
}