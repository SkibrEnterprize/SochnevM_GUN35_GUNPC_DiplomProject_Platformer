
using Cysharp.Threading.Tasks;
using Player;
using UnityEngine;
using Zenject;

public class TriggerExplosionComponent : MonoBehaviour, IHealthAffected
{
    [Header("General")]
    [SerializeField] private bool _isActivatedOfContact = true;

    [Header("Config Delay for Explode")]
    [SerializeField] private float _activationDelay = 1.5f;
    [SerializeField] private Color _warningColor = Color.red;
    [SerializeField] float _scaleMultiplier = 1.2f;

    [Header("Config Damage and Force")]
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _knockbackForce = 10f;
    [SerializeField] private float _knockupForce = 2f;
    [SerializeField] private float _explosionRadius = 2f;
    [SerializeField] private bool _showGizmos = true;

    private PlayerMovementSystem _movementComponent;
    private HealthModel _healthModel;
    private ISoundEventBus _soundBus;
    private VFXEventBus _vfxBus;
    private MeshRenderer _renderer;
    private Color _originalColor;
    private bool _isTriggered = false;
    private Vector3 _originalScale;

    [Inject]
    private void Construct(PlayerMovementSystem movementComponent,
        HealthModel healthModel,
        ISoundEventBus soundEventBus,
        VFXEventBus vFXEventBus)
    {
        _movementComponent = movementComponent;
        _healthModel = healthModel;
        _soundBus = soundEventBus;
        _vfxBus = vFXEventBus;
    }

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        if (_renderer != null)
            _originalColor = _renderer.material.color;
        _originalScale = transform.localScale;
    }

    public void ApplyHealthChange(int delta, Vector3 sourcePosition = default,
                                 DamageType type = DamageType.Default, float knockbackForce = 0f)
    {
        if (delta < 0 && !_isTriggered)
        {
            ActivateExplosionWithDelay();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!_isActivatedOfContact) return;
        if (!_isTriggered && other.TryGetComponent<CharacterController>(out _))
        {
            ActivateExplosionWithDelay();
        }
    }

    private void ActivateExplosionWithDelay()
    {
        _vfxBus.Play(VFXType.FlameUp, transform.position);
        ActivateTrapAsync().Forget();
    }
    private async UniTaskVoid ActivateTrapAsync()
    {
        _isTriggered = true;
        var ct = this.GetCancellationTokenOnDestroy();

        float elapsed = 0;
        while (elapsed < _activationDelay)
        {
            float t = elapsed / _activationDelay;

            if (_renderer != null)
                _renderer.material.color = Color.Lerp(_originalColor, _warningColor, t);

            transform.localScale = Vector3.Lerp(_originalScale, _originalScale * _scaleMultiplier, t);

            elapsed += Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        Explode();


        if (_renderer != null)
            _renderer.material.color = _originalColor;

        transform.localScale = _originalScale;

        _isTriggered = false;
        Destroy(gameObject);
    }

    private void Explode()
    {
        _soundBus.Play(SoundType.Explode, transform.position);
        _vfxBus.Play(VFXType.Explode, transform.position);
        Collider[] targets = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (var target in targets)
        {
            var controller = target.GetComponent<CharacterController>();

            if (controller != null)
            {
                Debug.Log("Игрок в зоне взрыва, толкаю!");
                ApplyEffects(controller);
                continue; // Чтобы не наносить урон дважды через интерфейс ниже
            }

            if (target.TryGetComponent<IHealthAffected>(out var health))
            {
                if (target.gameObject != this.gameObject)
                {
                    health.ApplyHealthChange(-_damage);
                }
            }

        }
    }

    private void ApplyEffects(CharacterController controller)
    {
        Vector3 direction = controller.transform.position - transform.position;
        direction.z = 0;
        direction = direction.normalized;
        direction.y = _knockupForce;

        _movementComponent.ApplyImpulse(direction * _knockbackForce);
        _healthModel.ApplyHealthChange(-_damage,
            transform.position,
            DamageType.Default,
            _knockbackForce);
    }

    private void OnDrawGizmosSelected()
    {
        if (_showGizmos)
        {
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            Gizmos.DrawSphere(transform.position, _explosionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}
