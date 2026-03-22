using UnityEngine;
using Zenject;

public class CheckPointComponent : MonoBehaviour
{
    [SerializeField] private bool _isActivate = false;
    private CheckPointModel _checkPointModel;

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    [Inject]
    private void Construct(CheckPointModel checkPointHandler)
    {
        _checkPointModel = checkPointHandler;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isActivate) return;
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
        {
            _isActivate = true;
            _checkPointModel.SetCheckpoint(transform.position, transform.rotation);
            Debug.Log($"[Checkpoint] Активирован: {gameObject.name}");
        }
    }
}