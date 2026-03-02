using UnityEngine;
using Zenject;

public class TriggerFlyingComponent : MonoBehaviour
{
    private IFlyingZoneHandler _movementComponent;

    [Inject]
    private void Construct(IFlyingZoneHandler movementComponent)
    {
        _movementComponent = movementComponent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
            _movementComponent.AddForceFlyingByTrigger();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
            _movementComponent.RemoveForceFlyingByTrigger();
    }

}
