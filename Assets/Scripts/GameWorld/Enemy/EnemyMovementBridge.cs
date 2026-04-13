using UnityEngine;

public class EnemyMovementBridge : MonoBehaviour, IKnockbackReceiver
{
    [SerializeField] private CharacterController _controller;

    private Vector3 _velocity;

    public void ApplyImpulse(Vector3 impulse)
    {
        _velocity += impulse;
    }

    private void Update()
    {
        if (_controller == null)
            return;

        if (!_controller.gameObject.activeInHierarchy)
            return;

        if (!_controller.enabled)
            return;

        _velocity.y -= 9.81f * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime);
    }

    private bool CanMove()
{
    return _controller != null
        && _controller.enabled
        && _controller.gameObject.activeInHierarchy;
}
}