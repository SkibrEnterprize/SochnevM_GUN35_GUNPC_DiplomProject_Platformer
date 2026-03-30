using Player;
using System.Collections;
using UnityEngine;
using Zenject;

public class TriggerExplosionComponent : MonoBehaviour
{
    [Header("Config Delay")]
    [SerializeField] float _activationDelay = 1.5f; // Та самая визуальная задержка
    [SerializeField] Color _warningColor = Color.red;

    [Header("Config Damage and Adforce")]
    [SerializeField] int _damage = 20;
    [SerializeField] float _knockbackForce = 10f;
    [SerializeField] float _knockupForce = 2f;
    [SerializeField] float _explosionRadius = 2f;
    [SerializeField] private bool _showGizmos = true;


    private PlayerMovementSystem _movementComponent;
    private HealthModel _healthModel;
    private MeshRenderer _renderer;
    private Color _originalColor;
    private bool _isTriggered = false;

    [Inject]
    private void Construct(PlayerMovementSystem movementComponent,
            HealthModel healthModel)
    {
        _movementComponent = movementComponent;
        _healthModel = healthModel;
    }
    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _originalColor = _renderer.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isTriggered &&
            other.gameObject.TryGetComponent<CharacterController>(out CharacterController controller))
        {
            StartCoroutine(ActivateTrapRoutine(controller));
        }
    }

    private IEnumerator ActivateTrapRoutine(CharacterController controller)
    {
        _isTriggered = true;

        // заставить объект изменить цвет
        float elapsed = 0;
        while (elapsed < _activationDelay)
        {
            _renderer.material.color = Color.Lerp(_originalColor, _warningColor, elapsed / _activationDelay);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (Vector3.Distance(transform.position, controller.transform.position) < _explosionRadius)
        {
            ApplyEffects(controller);
        }

        // После срабатывания возвращаем всё как было
        yield return new WaitForSeconds(0.5f);
        _renderer.material.color = _originalColor;
        _isTriggered = false;
    }

    private void ApplyEffects(CharacterController controller)
    {     
        Vector3 direction = (controller.transform.position - transform.position).normalized;
        direction.y = _knockupForce; // Немного подбрасываем вверх

        _movementComponent.ApplyImpulse(direction * _knockbackForce);
        _healthModel.ApplyHealthChange(-_damage);
    }

    private void OnDrawGizmosSelected()
    {
        if (_showGizmos)
        {
            // Прозрачный красный для заливки
            Gizmos.color = new Color(1, 0, 0, 0.2f);
            Gizmos.DrawSphere(transform.position, _explosionRadius);

            // Яркий красный для контура
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);

        }
    }
}
