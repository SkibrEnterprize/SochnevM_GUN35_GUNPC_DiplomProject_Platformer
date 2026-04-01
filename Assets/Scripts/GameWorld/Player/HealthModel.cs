using System;
using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class HealthModel : IInitializable, IDisposable, IHealthAffected
    {
        private IHealthEventBus _healthEventBus;
        private readonly PlayerMovementSystem _movementComponent;
        private readonly PlayerConfig _playerConfig;
        private readonly PlayerAnimator _playerAnimator;
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
            PlayerMovementSystem movementComponent,
            IHealthEventBus healthEventBus,
            PlayerAnimator playerAnimator)
        {
            _playerConfig = playerConfig;
            _movementComponent = movementComponent;
            _healthEventBus = healthEventBus;
            _playerAnimator = playerAnimator;
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
                ApplyHealthChange(-_playerConfig.DamageOfFall);
                Debug.Log($"Health = {_health} because of FallDistance is {fallDistance}");
            }
        }
                
        public void ApplyHealthChange(int delta, Vector3 sourcePosition = default)
        {
            Health += delta;
            _healthEventBus.HealthUpdated(delta);
            if (delta < 0) _playerAnimator.PlayHit();
            CheckHealthValue();
        }

        private void TakeDamage(int value) => Health -= value;
        private void TakeHealing(int value) => Health += value;
        private void CheckHealthValue()
        {
            if (_health <= 0)
            {
                _playerAnimator.PlayDeath(); // Запускаем анимацию
                _movementComponent.IsMovementFrozen = true; // Замораживаем ввод
                _healthEventBus.HealthIsOver();
            }
        }
        public void HealthAllRepair()
        {
            Health = _playerConfig.MaxHealth;
        }
        
    }
}