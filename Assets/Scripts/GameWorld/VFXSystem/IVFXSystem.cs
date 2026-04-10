using UnityEngine;

public interface IVFXSystem
{
    void Play(VFXType type, 
        Vector3 position,
        float scaleMultiplier = 1f, 
        Quaternion rotation = default, 
        Transform parent = null);
}