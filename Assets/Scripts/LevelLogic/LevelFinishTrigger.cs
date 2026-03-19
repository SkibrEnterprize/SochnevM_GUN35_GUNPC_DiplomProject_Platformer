using UnityEngine;
using Zenject;

public class LevelFinishTrigger : MonoBehaviour
{
    private LevelFinishSystem _system;
    private bool _isActivate;

    [Inject]
    private void Construct(LevelFinishSystem system)
    {
        _system = system;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller)
            && !_isActivate)
        {
            _isActivate = true;
            _system.EndPointReached();
        }
    }
}
