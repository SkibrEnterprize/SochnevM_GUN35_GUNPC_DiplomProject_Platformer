using Player;
using UnityEngine;
using Zenject;

public class TriggerChangeSpeed : MonoBehaviour
{

    [Range(0.05f, 1f)]
    [SerializeField] float _speedModifire = 1f;
    [Range(0.05f, 1f)]
    [SerializeField] float _traction = 0.2f;
    private MovementComponent _movementComponent;

    [Inject]
    private void Construct(MovementComponent movementComponent)
    {
        _movementComponent = movementComponent;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsCaracter(other))
        {
            _movementComponent.SetSurfaceEffect(_speedModifire, _traction);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (IsCaracter(other))
        {
            _movementComponent.ResetSurfaceEffect();
        }
    }

    private bool IsCaracter(Collider collider)
    {
        return collider.gameObject.TryGetComponent<CharacterController>(out CharacterController controller);
    }

}
