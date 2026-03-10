using UnityEngine;
using Zenject;

public class TriggerInpactOfMoveComponent : MonoBehaviour
{
    [Tooltip("Set CheckBox if need to add force\nDo not set if need to slow down")] 
    [SerializeField] private bool _isAddForce;
    [SerializeField] private float _advancedFlyForce;
    [SerializeField] private float _advancedMoveForce;

    private IInpactOfMoveHandler _movementComponent;


    [Inject]
    private void Construct(IInpactOfMoveHandler movementComponent)
    {
        _movementComponent = movementComponent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCharacter(other))
        {
            _movementComponent.ChangeForceByTrigger(_isAddForce, _advancedFlyForce, _advancedMoveForce);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsCharacter(other))
        {
            _movementComponent.ChangeForceByTrigger(!_isAddForce, _advancedFlyForce, _advancedMoveForce);
        }
    }

    private bool IsCharacter(Collider collider)
    {
        return collider.gameObject.TryGetComponent<CharacterController>(out CharacterController controller);
    }

}
