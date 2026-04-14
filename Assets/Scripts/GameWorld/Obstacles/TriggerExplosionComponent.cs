using System.Collections;
using UnityEngine;
using Zenject;

public class TriggerExplosionComponent : MonoBehaviour, IHealthAffected
{
    [Header("General")]
    [SerializeField] private bool _isActivatedOfContact = true;

    [Header("Config Delay for Explode")]
    [SerializeField] private float _activationDelay = 1.5f;
    [SerializeField] private Color _warningColor = Color.red;
    [SerializeField] private float _scaleMultiplier = 1.2f;

    [Header("Config Damage and Force")]
    [SerializeField] private int _damage = 20;
    [SerializeField] private float _knockbackForce = 10f;
    [SerializeField] private float _knockupForce = 2f;
    [SerializeField] private float _explosionRadius = 2f;
    [SerializeField] private bool _showGizmos = true;

    private ISoundEventBus _soundBus;
    private VFXEventBus _vfxBus;

    private MeshRenderer _renderer;
    private Color _originalColor;
    private Vector3 _originalScale;

    private bool _isTriggered;

    [Inject]
    private void Construct(ISoundEventBus soundBus, VFXEventBus vfxBus)
    {
        _soundBus = soundBus;
        _vfxBus = vfxBus;
    }

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();

        if (_renderer != null)
            _originalColor = _renderer.material.color;

        _originalScale = transform.localScale;
    }

    
    public void ApplyHealthChange(
    int delta,
    Vector3 sourcePosition = default,
    DamageType type = DamageType.Default,
    float knockbackForce = 0f)
    {
        if (_isTriggered)
            return;

        if (delta < 0)
            ActivateExplosionWithDelay();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isActivatedOfContact)
            return;

        if (_isTriggered)
            return;

        if (other.TryGetComponent<IHealthAffected>(out _))
        {
            ActivateExplosionWithDelay();
        }
    }

    private void ActivateExplosionWithDelay()
    {
        if (_isTriggered)
            return;

        _isTriggered = true;

        StopAllCoroutines(); 

        _vfxBus.Play(VFXType.FlameUp, transform.position);
        StartCoroutine(ActivateTrapCoroutine());
    }

    private IEnumerator ActivateTrapCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < _activationDelay)
        {
            float t = elapsed / _activationDelay;

            if (_renderer != null)
                _renderer.material.color = Color.Lerp(_originalColor, _warningColor, t);

            transform.localScale =
                Vector3.Lerp(_originalScale, _originalScale * _scaleMultiplier, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Explode();

        ResetVisuals();

        Destroy(gameObject);
    }

   
    private void Explode()
    {
        _soundBus.Play(SoundType.Explode, transform.position);
        _vfxBus.Play(VFXType.Explode, transform.position);

        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (hit.TryGetComponent<IHealthAffected>(out var health))
            {
                health.ApplyHealthChange(-_damage, transform.position);
            }

            if (hit.TryGetComponent<IKnockbackReceiver>(out var knockback))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                dir.z = 0f;
                dir.Normalize();
                dir.y = _knockupForce;

                knockback.ApplyImpulse(dir * _knockbackForce);
            }
        }
    }

    private void ResetVisuals()
    {
        if (_renderer != null)
            _renderer.material.color = _originalColor;

        transform.localScale = _originalScale;
    }

   
    private void OnDrawGizmosSelected()
    {
        if (!_showGizmos) return;

        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _explosionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}