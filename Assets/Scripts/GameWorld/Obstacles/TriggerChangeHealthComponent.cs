using UnityEngine;
using Zenject;

public class TriggerChangeHealthComponent : MonoBehaviour
{
    [Tooltip("A negative value causes damage\nA positive value heals for that amount")]
    [SerializeField] private int _amountOfChangeInHealth = 10;
    private ITakeChangeByTrigger _healthComponent;

    [Inject] 
    private void Construct(ITakeChangeByTrigger healthComponent)
    {
        _healthComponent = healthComponent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<CharacterController>(out CharacterController  controller))
        _healthComponent.TakeChangeByTrigger(_amountOfChangeInHealth);
    }
}
