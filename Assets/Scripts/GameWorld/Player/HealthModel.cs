using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using Zenject;

namespace Player
{
    public sealed class HealthModel : IInitializable, IDisposable, ITickable, IHealthAffected
    {
        private readonly CharacterController _controller;
        private IHealthEventBus _healthEventBus;
        private readonly PlayerMovementSystem _movementComponent;
        private readonly PlayerConfig _playerConfig;
        private readonly PlayerAnimator _playerAnimator;
        private readonly SoundEventBus _soundBus;
        private int _health;

        private bool _isWaitingForDeathLanding;
        public bool IsWaitingForDeathLanding => _isWaitingForDeathLanding;


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
            PlayerAnimator playerAnimator,
            SoundEventBus soundBus,
            CharacterController controller)
        {
            _playerConfig = playerConfig;
            _movementComponent = movementComponent;
            _healthEventBus = healthEventBus;
            _playerAnimator = playerAnimator;
            _soundBus = soundBus;
            _controller = controller;
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
            if (fallDistance > _playerConfig.MinHeightForDamage)
            {
                ApplyHealthChange(-_playerConfig.DamageOfFall, type: DamageType.Fall);

                Debug.Log($"Health = {Health} (Fall damage: {fallDistance}m)");
            }
        }

        public void ApplyHealthChange(int delta, Vector3 sourcePosition = default,
                      DamageType type = DamageType.Default, float knockbackForce = 0f)
        {
            if (delta < 0)
            {
                bool isGrounded = _movementComponent.IsGrounded();

                bool playFullAnimation = (type != DamageType.Fall) && isGrounded;

                _playerAnimator.PlayHit(playFullAnimation);

                TakeDamage(delta);
                _soundBus.Play(SoundType.Hit, _controller.transform.position);

                if (sourcePosition != default && type != DamageType.Fall)
                {
                    _movementComponent.ApplyKnockback(sourcePosition, knockbackForce);
                }
            }
            else if (delta > 0)
            {
                TakeHealing(delta);
                _soundBus.Play(SoundType.Healing, _controller.transform.position);
            }

            _healthEventBus.HealthUpdated(delta);
            CheckHealthValue();
        }
       
        private void TakeDamage(int value) => Health += value;
        private void TakeHealing(int value) => Health += value;

        private void CheckHealthValue()
        {
            if (Health <= 0)
            {
                Health = 0;

                if (!_movementComponent.IsGrounded())
                {
                    _isWaitingForDeathLanding = true;
                    _movementComponent.SetMovementLock(true);
                    return;
                }
                _healthEventBus.HealthIsOver();
            }
        }
        private void TriggerActualDeath()
        {
            _isWaitingForDeathLanding = false;
            _healthEventBus.HealthIsOver();
        }

        public void HealthAllRepair()
        {
            Health = _playerConfig.MaxHealth;
        }

        public void Tick()
        {
            if (_isWaitingForDeathLanding && _movementComponent.IsGrounded())
            {
                TriggerActualDeath();
            }
        }
    }
}