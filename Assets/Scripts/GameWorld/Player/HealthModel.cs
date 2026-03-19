using Player.Signals;
using System;
using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class HealthModel : IInitializable, IDisposable, ITakeChangeByTrigger
    {
        public event Action OnHealthIsOver;


        private readonly MovementComponent _movementComponent;
        private PlayerConfig _playerConfig;
        private int _health;
        private SoundLibrary _soundLibrary;


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
            SoundLibrary soundLibrary)
        {
            _playerConfig = playerConfig;
            _movementComponent = movementComponent;
            _soundLibrary = soundLibrary;
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
                //Health -= _playerConfig.DamageOfFall;
                Debug.Log($"Health = {_health} because of FallDistance is {fallDistance}");
            }
        }

        public void TakeChangeByTrigger(int value)
        {
            Health += value;
            if (value > 0)
            {
                _soundLibrary.RequestPlay(SoundType.Healing);
            }
            else
            {
                _soundLibrary.RequestPlay(SoundType.Dammage);
            }

                CheckHealthValue();
        }

        private void TakeDamage(int value) => Health -= value;
        private void TakeHealing(int value) => Health += value;
        private void CheckHealthValue()
        {
            if (_health <= 0) OnHealthIsOver?.Invoke();
        }

        public void HealthAllRepair()
        {
            Health = _playerConfig.MaxHealth;
        }
    }
}