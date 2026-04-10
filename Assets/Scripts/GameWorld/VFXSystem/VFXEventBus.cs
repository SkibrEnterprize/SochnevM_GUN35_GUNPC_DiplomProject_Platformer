using System;
using UnityEngine;

public class VFXEventBus
{
    // Событие: Тип эффекта, Позиция, Поворот, Родитель (опционально)
    public event Action<VFXType, 
        Vector3, 
        float,
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
            scaleMultiplier, 
            rotation, 
            parent);
    }
}