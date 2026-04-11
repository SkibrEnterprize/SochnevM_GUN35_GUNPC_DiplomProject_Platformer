using UnityEngine;

public interface IVFXSystem
{
    void Play(VFXType type, 
        Vector3 position,         
        Quaternion rotation = default, 
        Transform parent = null);
}