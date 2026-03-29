using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    public event Action OnDeath;
    public event Action<float> OnTakeDamage;

    private void Awake() => _currentHealth = _maxHealth;

    public void TakeDamage(float amount, Vector3 attackerPosition)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= amount;
        OnTakeDamage?.Invoke(amount);

        Debug.Log($"{gameObject.name} получил урон: {amount}. Осталось: {_currentHealth}");

        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();
        // логика смерти: частицы, звук, удаление
        Destroy(gameObject, 0.1f);
    }
}