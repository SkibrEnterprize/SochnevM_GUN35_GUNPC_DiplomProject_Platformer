using UnityEngine;

public interface IHealthAffected
{
    void ApplyHealthChange(int delta, Vector3 sourcePosition = default,
                          DamageType type = DamageType.Default, float knockbackForce = 0f);
}