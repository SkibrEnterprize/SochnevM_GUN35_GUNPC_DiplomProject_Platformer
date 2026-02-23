using UnityEngine;
using Zenject;

public class TriggerDamageComponent : MonoBehaviour
{
    [SerializeField] private int _damage = 10;
    private IDamageZoneHandler _healthComponent;

    [Inject] 
    private void Construct(IDamageZoneHandler healthComponent)
    {
        _healthComponent = healthComponent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<CharacterController>(out CharacterController  controller))
        _healthComponent.TakeDamageByTrigger(_damage);
    }
}
