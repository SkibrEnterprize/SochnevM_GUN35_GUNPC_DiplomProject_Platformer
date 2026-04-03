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
        private readonly PlayerStartParameters _startParameters;
        private readonly PlayerAnimator _playerAnimator;
        private readonly CombatUI _combatUI;
        

        private bool _isCharged;
        private float _nextAttackTime;


        private bool _isCharging;      // Флаг процесса зажатия
        private float _currentChargeTimer;

        public CombatComponent(CharacterController controller,
            Controls controls,
            CombatConfig config,
            SoundEventBus soundEventBus,
            VFXEventBus vfxEventBus,
            PlayerStartParameters startParameters,
            PlayerAnimator playerAnimator,
            CombatUI combatUI)
        {
            _controller = controller;
            _controls = controls;
            _config = config;
            _soundBus = soundEventBus;
            _vfxBus = vfxEventBus;
            _startParameters = startParameters;
            _playerAnimator = playerAnimator;
            _combatUI = combatUI;
        }

        public void Initialize()
        {
            _controls.Player.Attack.started += OnAttackStarted;
            _controls.Player.Attack.performed += OnHoldCharged;
            _controls.Player.Attack.canceled += OnAttackReleased;
        }

        public void Dispose()
        {
            _controls.Player.Attack.started -= OnAttackStarted;
            _controls.Player.Attack.performed -= OnHoldCharged;
            _controls.Player.Attack.canceled -= OnAttackReleased;
        }

        // Метод из ITickable, будет работать каждый кадр
        public void Tick()
        {
            if (_isCharging)
            {
                _currentChargeTimer += Time.deltaTime;

                float progress = Mathf.Clamp01(_currentChargeTimer / _config.HeavyAttackChargeTime);

                _combatUI.UpdateProgress(progress);
            }
        }

        private void OnAttackStarted(InputAction.CallbackContext context)
        {
            if (Time.time < _nextAttackTime) return;

            _isCharging = true;
            _isCharged = false;

            _currentChargeTimer = 0f;

            _combatUI.Show(true);
            _combatUI.UpdateProgress(0f);
        }

        private void OnHoldCharged(InputAction.CallbackContext context)
        {
            if (Time.time < _nextAttackTime) return;

            _isCharged = true;
            _combatUI.UpdateProgress(1f); // Сразу заполняем до конца???
            Debug.Log("Удар ЗАРЯЖЕН");
        }

        private void OnAttackReleased(InputAction.CallbackContext context)
        {
            _isCharging = false;
            _combatUI.Show(false);

            if (Time.time < _nextAttackTime)
            {
                _isCharged = false;
                return;
            }

            if (_isCharged)
            {
                PerformAttack(_config.HeavyAttack, SoundType.HeavyAttack, VFXType.HeavyAttack, true);
            }
            else
            {
                PerformAttack(_config.LightAttack, SoundType.Attack, VFXType.Attack, false);
            }

            _isCharged = false;
        }       
        private void PerformAttack(AttackData data, SoundType sound, VFXType vfxType, bool isHeavy)
        {
            _nextAttackTime = Time.time + data.Cooldown;

            
            _playerAnimator.PlayAttack(isHeavy);
            _soundBus.Play(sound);

            Vector3 vfxPosition = _startParameters.CombatVFXPoint.position;
            Quaternion vfxRotation = _startParameters.CombatVFXPoint.rotation;
            _vfxBus.Play(vfxType, vfxPosition, vfxRotation, _controller.gameObject.transform);

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
                    if (enemy.TryGetComponent<IHealthAffected>(out var target))
                    {
                        target.ApplyHealthChange(-data.Damage, origin);
                        Debug.Log($"[Combat] Удар по {enemy.name}. Направление: {directionX}");                        
                    }
                }
            }            
        }        
    }
}
