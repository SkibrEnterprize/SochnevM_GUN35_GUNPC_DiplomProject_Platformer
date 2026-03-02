using Player.Signals;
using System;
using UnityEngine;
using Zenject;

namespace Player
{
    public sealed class HealthComponent : IInitializable, IDisposable, IDamageZoneHandler
    {
        private readonly SignalBus _signalBus;
        private PlayerConfig _playerConfig;
        private int _health;

        public HealthComponent(PlayerConfig playerConfig,
            SignalBus signalBus)
        {
            _playerConfig = playerConfig;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<FallDistanceSignal>(OnFallDistanceReceived);
            _health = _playerConfig.MaxHealth;
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
                _health -= _playerConfig.DamageOfFall;
                Debug.Log($"Health = {_health} because of FallDistance is {fallDistance}");
            }
        }

        public void TakeDamageByTrigger(int damage)
        {
           _health -= damage;
            Debug.Log($"Damage by Trigger - {damage}, Total Health = {_health}");
        }
    }
}