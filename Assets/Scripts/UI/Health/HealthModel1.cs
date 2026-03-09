using System;
using UnityEngine;
public sealed class HealthModel1
{
    private int _health;
    public event Action<int> OnHealthChanged;
    public int Health
    {
        get => _health;
        set
        {
            if (_health != value)
            {
                _health = Mathf.Clamp(value, 0, 100);
                OnHealthChanged?.Invoke(_health);
            }
        }
    }
    public HealthModel1(int initialHealth)
    {
        _health = initialHealth;
    }
}


