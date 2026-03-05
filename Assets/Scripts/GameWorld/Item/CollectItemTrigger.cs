using Player.Signals;
using UnityEngine;
using Zenject;

public class CollectItemTrigger : MonoBehaviour
{
    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller)
            && isActiveAndEnabled)
        {
          gameObject.SetActive(false);
            _signalBus.Fire<CollectItemSignal>();
        }
    }
}
