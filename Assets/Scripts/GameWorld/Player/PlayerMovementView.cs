using Player;
using UnityEngine;
using Zenject;

public class PlayerMovementView : MonoBehaviour, IKnockbackReceiver
{
    [Inject] public PlayerMovementSystem MovementSystem;

    public void ApplyImpulse(Vector3 impulse)
    {
        MovementSystem.ApplyImpulse(impulse);
    }
}