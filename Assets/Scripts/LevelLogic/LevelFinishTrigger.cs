using UnityEngine;
using Zenject;

[RequireComponent(typeof(BoxCollider))]
public class LevelFinishTrigger : MonoBehaviour
{
    private LevelFinishSystem _levelSystem;
    private bool _isActivate;

    [Inject]
    private void Construct(LevelFinishSystem levelSystem)
    {
        _levelSystem = levelSystem;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller)
            && !_isActivate)
        {
            _isActivate = true;
            _levelSystem.EndPointReached();
        }
    }
}
