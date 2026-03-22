using System;
using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class HealthModel : IInitializable, IDisposable, ITakeChangeByTrigger
    {
        private IHealthEventBus _healthEventBus;
        private readonly MovementComponent _movementComponent;
        private PlayerConfig _playerConfig;
        private int _health;


        public event Action<int> OnHealthChanged;
        public int Health
        {
            get => _health;
            set
            {
                if (_health != value)
                {
                    _health = Mathf.Clamp(value, 0, _playerConfig.MaxHealth);
                    OnHealthChanged?.Invoke(_health);
                }
            }
        }
        public HealthModel(PlayerConfig playerConfig,
            MovementComponent movementComponent,
            IHealthEventBus healthEventBus)
        {
            _playerConfig = playerConfig;
            _movementComponent = movementComponent;
            _healthEventBus = healthEventBus;
        }

        public void Initialize()
        {
            Health = _playerConfig.MaxHealth;
        }

        public void Dispose()
        {
        }

        public void FallDistanceReceived(float fallDistance)
        {
            TakeFallDamage(fallDistance);
        }

        private void TakeFallDamage(float fallDistance)
        {
            if (fallDistance > _playerConfig.MinHeightForDamage)
            {
                TakeChangeByTrigger(-_playerConfig.DamageOfFall);
                Debug.Log($"Health = {_health} because of FallDistance is {fallDistance}");
            }
        }

        public void TakeChangeByTrigger(int value)
        {
            Health += value;
            _healthEventBus.HealthUpdated(value);
            CheckHealthValue();
        }

        private void TakeDamage(int value) => Health -= value;
        private void TakeHealing(int value) => Health += value;
        private void CheckHealthValue()
        {
            if (_health <= 0) _healthEventBus.HealthIsOver();
        }
        public void HealthAllRepair()
        {
            Health = _playerConfig.MaxHealth;
        }
    }
}