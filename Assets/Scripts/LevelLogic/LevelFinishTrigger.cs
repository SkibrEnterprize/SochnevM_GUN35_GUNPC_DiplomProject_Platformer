using Player.Signals;
using UnityEngine;
using Zenject;

public class LevelFinishTrigger : MonoBehaviour
{
    private bool _isActivate;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller)
            && !_isActivate)
        {
            _isActivate = true;
            _signalBus.Fire<LevelFinishCollectSignal>();
        }
    }
}
