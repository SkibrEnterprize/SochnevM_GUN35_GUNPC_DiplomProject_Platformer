using System;
using UnityEngine;

public class VFXEventBus
{
    // Событие: Тип эффекта, Позиция, Поворот, Родитель (опционально)
    public event Action<VFXType, Vector3, Quaternion, Transform> OnVFXRequested;

    public void Play(VFXType type, Vector3 position, Quaternion rotation = default, Transform parent = null)
    {
        OnVFXRequested?.Invoke(type, position, rotation, parent);
    }
}