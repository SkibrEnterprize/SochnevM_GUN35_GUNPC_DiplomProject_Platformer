using UnityEngine;

public interface IKnockbackable 
{
    void ApplyKnockback(Vector3 sourcePosition = default, float knockbackForce = 0f);
}
