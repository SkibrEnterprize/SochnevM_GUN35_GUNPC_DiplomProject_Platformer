using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHealthAffected
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    public event Action OnDeath;
    public event Action<float> OnTakeDamage;

    private void Awake() => _currentHealth = _maxHealth;
    public void ApplyHealthChange(int delta, Vector3 sourcePosition = default,
                              DamageType type = DamageType.Default, float knockbackForce = 0f)
    {
        if (_currentHealth <= 0) return;

        _currentHealth += delta;

        if (delta < 0 && type != DamageType.Fall)
        {
            OnTakeDamage?.Invoke(delta);
        }
        else if (delta < 0)
        {
            // to do
        }
        Debug.Log($"{gameObject.name} получил урон ({type}). Осталось: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDeath?.Invoke();
        }
    }
}