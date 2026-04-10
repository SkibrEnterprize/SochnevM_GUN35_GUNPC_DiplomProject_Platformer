using DG.Tweening;
using Player;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public class SpringTrigger : MonoBehaviour
{
    [Header("Импульс")]

    [SerializeField] private int _impulseX = 5;
    [SerializeField] private int _impulseY = 5;
    [Tooltip("Относительно локальной оси объекта?")]
    [SerializeField] private bool _useLocalSpace = true;
    private PlayerMovementSystem _movementComponent;
    private ISoundEventBus _soundBus;
    private Vector3 _impulse;

    [Header("Визуал")]
    [SerializeField] private float _animationDuration = 0.1f;
    private Transform _visualModel; // Ссылка на меш пружины

    [Inject]
    public void Construct(PlayerMovementSystem movementComponent,
        ISoundEventBus soundEventBus)
    {
        _movementComponent = movementComponent;
        _soundBus = soundEventBus;
    }
    private void Awake()
    {
    _impulse = new Vector3(_impulseX, _impulseY, 0);
        _visualModel = this.transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
        {

            Vector3 worldImpulse = _useLocalSpace
                                   ? transform.TransformDirection(_impulse).normalized * _impulse.magnitude
                                   : _impulse.normalized * _impulse.magnitude;

            _movementComponent.ApplyImpulse(worldImpulse);
            _soundBus.Play(SoundType.Spring);
            PlaySpringAnimation();
        }
    }

    private void PlaySpringAnimation()
    {
        if (_visualModel == null) return;

        _visualModel.DOKill();

        Sequence s = DOTween.Sequence();

        s.Append(_visualModel.DOScale(new Vector3(1.3f, 0.5f, 1.3f), _animationDuration).SetEase(Ease.OutQuad));

        s.Append(_visualModel.DOScale(new Vector3(0.8f, 1.4f, 0.8f), _animationDuration * 0.5f).SetEase(Ease.OutBack));

        s.Append(_visualModel.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutElastic));
    }

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