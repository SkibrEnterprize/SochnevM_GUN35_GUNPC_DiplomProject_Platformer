using UnityEngine;
using Zenject;

public class TriggerChangeOfForceComponent : MonoBehaviour
{
    [Tooltip("For Up and Down use AdvancedFlyForce\nFor Left and Right use AdvancedMoveForce")]
    [SerializeField] private TypeZoneOfAirForce _typeZoneOfForce;

    [SerializeField] private float _advancedFlyForce;

    private bool _isAddForce;
    private IChangeOfForceHandler _movementComponent;


    [Inject]
    private void Construct(IChangeOfForceHandler movementComponent)
    {
        _movementComponent = movementComponent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCharacter(other))
        {
            ChangeForceByZone(_isAddForce);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsCharacter(other))
        {
            ChangeForceByZone(!_isAddForce);
        }
    }

    private bool IsCharacter(Collider collider)
    {
        return collider.gameObject.TryGetComponent<CharacterController>(out CharacterController controller);
    }


    private void ChangeForceByZone(bool isAddForce)
    {
        // Обрабатываем выбор через switch
        switch (_typeZoneOfForce)
        {
            case TypeZoneOfAirForce.AirFlowUp:
                _movementComponent.ChangeForceByTrigger(isAddForce, _advancedFlyForce, 0);
                break;
            case TypeZoneOfAirForce.AirFlowDown:
                _movementComponent.ChangeForceByTrigger(isAddForce, -_advancedFlyForce, 0);
                break;
            case TypeZoneOfAirForce.AirFlowLeft:
                _movementComponent.ChangeForceByTrigger(isAddForce, 0, _advancedFlyForce);
                break;
            case TypeZoneOfAirForce.AirFlowRight:
                _movementComponent.ChangeForceByTrigger(isAddForce, 0, -_advancedFlyForce);
                break;            
        }
    }
}