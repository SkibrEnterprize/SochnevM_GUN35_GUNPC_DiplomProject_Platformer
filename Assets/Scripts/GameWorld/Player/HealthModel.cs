using Player.Signals;
using System;
using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class HealthModel : IInitializable, IDisposable, ITakeChangeByTrigger
    {
        private readonly SignalBus _signalBus;
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
                    Debug.Log("Health Update ON Envoke");
                }
            }
        }
        public HealthModel(PlayerConfig playerConfig,
            SignalBus signalBus)
        {
            _playerConfig = playerConfig;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<FallDistanceSignal>(OnFallDistanceReceived);
            Health = _playerConfig.MaxHealth;
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<FallDistanceSignal>(OnFallDistanceReceived);
        }

        private void OnFallDistanceReceived(FallDistanceSignal signal)
        {
            TakeFallDamage(signal.FallDistance);
        }

        private void TakeFallDamage(float fallDistance)
        {
            if (fallDistance > _playerConfig.MinHeightForDamage)
            {
                Health -= _playerConfig.DamageOfFall;
                Debug.Log($"Health = {_health} because of FallDistance is {fallDistance}");
            }
        }

        public void TakeChangeByTrigger(int value)
        {
            Health += value;
            Debug.Log($"Value by Trigger - {value}, Total Health = {_health}");
        }
    }
}