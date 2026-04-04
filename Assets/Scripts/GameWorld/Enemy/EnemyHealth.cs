using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IHealthAffected
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    public event Action OnDeath;
    public event Action<float> OnTakeDamage;

    private void Awake() => _currentHealth = _maxHealth;
    public void ApplyHealthChange(int delta, Vector3 sourcePosition = default)
    {
        if (_currentHealth <= 0) return;

        _currentHealth += delta;

        OnTakeDamage?.Invoke(delta);

        Debug.Log($"{gameObject.name} получил урон. Осталось: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            OnDeath?.Invoke();
        }
    }
}