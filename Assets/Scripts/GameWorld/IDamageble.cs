
using UnityEngine;

public interface IDamageble 
{
    void TakeDamage(int damage, Vector3 sourcePosition, float knockbackForce);

}
