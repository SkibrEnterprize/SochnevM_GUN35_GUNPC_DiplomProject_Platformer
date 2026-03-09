using UnityEngine;
using Zenject;

public class CheckPointComponent : MonoBehaviour
{
    [SerializeField] private bool _isActivate = false;
    private CheckPointHolder _checkPointHandler;

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    // —сылка на текущий чек?поинт (управл€етс€ через сервис)
    [Inject]
    private void Construct(CheckPointHolder checkPointHandler)
    {
        _checkPointHandler = checkPointHandler;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isActivate) return;
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
        {
            _isActivate = true;
            _checkPointHandler.SetCheckpoint(transform.position, transform.rotation);
            Debug.Log($"[Checkpoint] јктивирован: {gameObject.name}");
        }
    }
}