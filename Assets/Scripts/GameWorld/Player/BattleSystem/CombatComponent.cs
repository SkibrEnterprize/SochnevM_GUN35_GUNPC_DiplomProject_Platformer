using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
    public sealed class CombatComponent : IInitializable, IDisposable
    {
        private readonly Controls _controls;
        private readonly CombatConfig _config;
        private readonly CharacterController _controller;
        private readonly SoundEventBus _soundBus;

        private bool _isCharged;       // Флаг: накопили ли мы тяжелый удар
        private float _nextAttackTime; // Таймер кулдауна

        public CombatComponent(CharacterController controller,
            Controls controls,
            CombatConfig config,
            SoundEventBus soundEventBus)
        {
            _controller = controller;
            _controls = controls;
            _config = config;
            _soundBus = soundEventBus;
        }

        public void Initialize()
        {
            _controls.Player.Attack.started += OnAttackStarted;     // Нажали кнопку
            _controls.Player.Attack.performed += OnHoldCharged;     // Удержали (зарядили)
            _controls.Player.Attack.canceled += OnAttackReleased;   // Отпустили (ударили)
        }

        public void Dispose()
        {
            _controls.Player.Attack.started -= OnAttackStarted;
            _controls.Player.Attack.performed -= OnHoldCharged;
            _controls.Player.Attack.canceled -= OnAttackReleased;
        }

        private void OnAttackStarted(InputAction.CallbackContext context)
        {
            if (Time.time < _nextAttackTime) return;

            // тут анимация замаха или звук
            Debug.Log("Замах начался...");
        }

        private void OnHoldCharged(InputAction.CallbackContext context)
        {
            if (Time.time < _nextAttackTime) return;

            _isCharged = true;
            // можно включить визуальный эффект (свечение)
            Debug.Log("Удар ЗАРЯЖЕН (Heavy ready)");
        }

        private void OnAttackReleased(InputAction.CallbackContext context)
        {
            // Проверяем кулдаун только в момент финального действия
            if (Time.time < _nextAttackTime)
            {
                _isCharged = false; // Сбрасываем флаг, если нажали слишком рано
                return;
            }

            if (_isCharged)
            {
                // Тяжелый удар
                PerformAttack(_config.HeavyAttack, SoundType.HeavyAttack);
                Debug.Log("ВЫПОЛНЕН: Тяжелый удар");
            }
            else
            {
                // Обычный удар
                PerformAttack(_config.LightAttack, SoundType.Attack);
                Debug.Log("ВЫПОЛНЕН: Обычный удар");
            }

            _isCharged = false; // Сбрасываем заряд после любого удара
        }

        private void PerformAttack(AttackData data, SoundType sound)
        {
                        _nextAttackTime = Time.time + data.Cooldown;

            _soundBus.Play(sound);

            Vector3 origin = _controller.bounds.center;
            // учитываем текущий масштаб игрока для радиуса атаки
            float currentRadius = data.Range * _controller.transform.localScale.x;

            Collider[] hitEnemies = Physics.OverlapSphere(origin, currentRadius, _config.EnemyLayer);

            foreach (var enemy in hitEnemies)
            {
                if (enemy.TryGetComponent<IDamageable>(out var target))
                {
                    target.TakeDamage(data.Damage);
                    Debug.Log($"Попал по {enemy.name}!");
                }
            }
        }
    }
}
