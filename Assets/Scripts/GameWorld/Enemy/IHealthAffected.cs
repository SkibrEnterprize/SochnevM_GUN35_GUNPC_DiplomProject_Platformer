using UnityEngine;

public interface IHealthAffected
{    
    void ApplyHealthChange(int delta, Vector3 sourcePosition = default);
}