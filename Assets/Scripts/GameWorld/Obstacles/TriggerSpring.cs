using Player;
using UnityEngine;
using Zenject;

/// <summary>
/// Простой «пружинный» триггер.
/// Добавьте его на любой Collider (IsTrigger = true).
/// </summary>
[RequireComponent(typeof(Collider))]
public class SpringTrigger : MonoBehaviour
{
    [Header("Импульс")]

    [SerializeField] private int _impulseX = 5;
    [SerializeField] private int _impulseY = 5;
    [Tooltip("Относительно локальной оси объекта?")]
    [SerializeField] private bool _useLocalSpace = true;
    private MovementComponent _movementComponent;
    private Vector3 _impulse;

    /* ------------------------------------------------------------------ */

    [Inject]
    public void Construct(MovementComponent movementComponent) {  _movementComponent = movementComponent; }
    private void Awake()
    {
    _impulse = new Vector3(_impulseX, _impulseY, 0);        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
        {

            Vector3 worldImpulse = _useLocalSpace
                                   ? transform.TransformDirection(_impulse).normalized * _impulse.magnitude
                                   : _impulse.normalized * _impulse.magnitude;

            // Мгновенно «толкнуть» игрока
            _movementComponent.ApplyImpulse(worldImpulse);
            Debug.Log("TriggerSpring!!!");
        }
    }


    /* ------------------------------------------------------------------ */

    private void OnDrawGizmosSelected()
    {
        var start = transform.position;
        var end = start + (_useLocalSpace
                             ? transform.TransformDirection(_impulse).normalized * _impulse.magnitude
                             : _impulse.normalized * _impulse.magnitude);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.15f);
    }

}