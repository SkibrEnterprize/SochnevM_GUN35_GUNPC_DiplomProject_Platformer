using UnityEngine;
using Zenject;

public class TriggerChangeHealthComponent : MonoBehaviour
{
    [Tooltip("A negative value causes damage\nA positive value heals for that amount")]
    [SerializeField] private int _amountOfChange = 10;
    //private IHealthAffected _healthComponent;

    //[Inject] 
    //private void Construct(IHealthAffected healthComponent)
    //{
    //    _healthComponent = healthComponent;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<IHealthAffected>(out IHealthAffected healthAffected))
            healthAffected.ApplyHealthChange(_amountOfChange);
    }
}
