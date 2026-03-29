using System;
using UnityEngine;

public interface IVFXEventBus
{
    
    public event Action<VFXType, Vector3, Quaternion, Transform> OnVFXRequested;

    public void Play(VFXType type, Vector3 position, Quaternion rotation = default, Transform parent = null);
   
}