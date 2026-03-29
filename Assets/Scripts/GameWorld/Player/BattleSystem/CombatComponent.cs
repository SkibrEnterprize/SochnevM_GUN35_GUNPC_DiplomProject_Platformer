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
        private readonly VFXEventBus _vfxBus;
        private PlayerStartParameters _startParameters;

        private bool _isCharged;       // Флаг: накопили ли мы тяжелый удар
        private float _nextAttackTime; // Таймер кулдауна

        public CombatComponent(CharacterController controller,
            Controls controls,
            CombatConfig config,
            SoundEventBus soundEventBus,
            VFXEventBus vfxEventBus,
            PlayerStartParameters startParameters)
        {
            _controller = controller;
            _controls = controls;
            _config = config;
            _soundBus = soundEventBus;
            _vfxBus = vfxEventBus;
            _startParameters = startParameters;
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
                PerformAttack(_config.HeavyAttack, SoundType.HeavyAttack, VFXType.HeavyAttack);
                Debug.Log("ВЫПОЛНЕН: Тяжелый удар");
            }
            else
            {
                // Обычный удар
                PerformAttack(_config.LightAttack, SoundType.Attack, VFXType.Attack);
                Debug.Log("ВЫПОЛНЕН: Обычный удар");
            }

            _isCharged = false; // Сбрасываем заряд после любого удара
        }

        private void PerformAttack(AttackData data, SoundType sound, VFXType vfxType)
        {
            _nextAttackTime = Time.time + data.Cooldown;

            _soundBus.Play(sound);

            Vector3 origin = _controller.bounds.center;

            float yRot = _startParameters.ViewTransform.eulerAngles.y;
            float directionX = (Mathf.Abs(Mathf.DeltaAngle(yRot, 180f)) < 10f) ? -1f : 1f;
            Vector3 attackDir = new Vector3(directionX, 0, 0);

            float playerScale = Mathf.Abs(_controller.transform.localScale.x);
            float currentRadius = data.Range * playerScale;

            // Сдвигаем на 70% от радиуса, чтобы бить "в упор", но не спиной
            Vector3 attackCenter = origin + attackDir * (currentRadius * 0.7f);

            Debug.DrawRay(attackCenter, Vector3.up * currentRadius, Color.red, 0.5f);
            Debug.DrawRay(attackCenter, attackDir * currentRadius, Color.red, 0.5f);
            Debug.DrawRay(attackCenter, attackDir * currentRadius * -1, Color.red, 0.5f);

            Collider[] hitEnemies = Physics.OverlapSphere(attackCenter, currentRadius, _config.EnemyLayer);

            foreach (var enemy in hitEnemies)
            {
                Vector3 dirToEnemy = (enemy.transform.position - origin).normalized;

                if (Vector3.Dot(attackDir, dirToEnemy) > 0.1f)
                {
                    if (enemy.TryGetComponent<IDamageable>(out var target))
                    {
                        target.TakeDamage(data.Damage);
                        Debug.Log($"[Combat] Удар по {enemy.name}. Направление: {directionX}");
                    }
                }
            }

            Quaternion vfxRotation = _startParameters.ViewTransform.rotation * Quaternion.Euler(25, -90, 45);
            Vector3 vfxPosition = origin + attackDir * 0.8f;
            _vfxBus.Play(vfxType, vfxPosition, vfxRotation, _controller.gameObject.transform);
            
        }
    }
}
