using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
    public sealed class CombatComponent : IInitializable, IDisposable, ITickable
    {
        private readonly Controls _controls;
        private readonly CombatConfig _config;
        private readonly CharacterController _controller;
        private readonly SoundEventBus _soundBus;
        private readonly VFXEventBus _vfxBus;
        private readonly PlayerStartParameters _startParameters;
        private readonly PlayerAnimator _playerAnimator;
        private readonly CombatUI _combatUI;

        private readonly AttackZoneDetector _attackZoneDetector;
        private readonly HeavyAttackZoneDetector _heavyAttackZoneDetector;
        private readonly Transform _attackVFXPoint;
        private readonly Transform _heavyAttackVFXPoint;
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
            _attackZoneDetector = _controller.GetComponentInChildren<AttackZoneDetector>();
            _heavyAttackZoneDetector = _controller.GetComponentInChildren<HeavyAttackZoneDetector>();
            _attackVFXPoint = _controller.GetComponentInChildren<AttackVFXPoint>().transform;
            _heavyAttackVFXPoint = _controller.GetComponentInChildren<HeavyAttackVFXPoint>().transform;
        }

        public void Initialize()
        {
            _controls.Player.Attack.started += OnAttackStarted;
            _controls.Player.Attack.canceled += OnAttackReleased;
        }

        public void Dispose()
        {
            _controls.Player.Attack.started -= OnAttackStarted;
            _controls.Player.Attack.canceled -= OnAttackReleased;
        }

        public void Tick()
        {
            if (!_isCharging) return;

            _currentChargeTimer += Time.deltaTime;

            float progress = Mathf.Clamp01(_currentChargeTimer / _config.HeavyAttackChargeTime);
            _combatUI.UpdateProgress(progress);

            if (progress >= 1f && !_isCharged)
            {
                _isCharged = true;
                _combatUI.SetChargedColor(true); 
                Debug.Log("Удар ПОЛНОСТЬЮ ЗАРЯЖЕН (из скрипта)");
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
            _currentChargeTimer = 0f; 
        }
        private void PerformAttack(AttackData data, SoundType sound, VFXType vfxType, bool isHeavy)
        {
            _nextAttackTime = Time.time + data.Cooldown;
            DrawViewInInspector(isHeavy);

            _playerAnimator.PlayAttack(isHeavy);
            _soundBus.Play(sound);
            var vfxPoint = isHeavy? _heavyAttackVFXPoint : _attackVFXPoint;
            // VFX...
            _vfxBus.Play(vfxType,
                vfxPoint.position,
                vfxPoint.rotation, 
                _controller.gameObject.transform);

            var detector = isHeavy ? _heavyAttackZoneDetector : _attackZoneDetector;
            var targets = detector.GetTargets();

            foreach (var target in targets)
            {
                if (target != null && target is MonoBehaviour mb && mb != null)
                {
                    target.ApplyHealthChange(-data.Damage, _controller.transform.position);
                }
            }
        }

        private void DrawViewInInspector(bool isHeavy)
        {
            var detector = isHeavy ? _heavyAttackZoneDetector : _attackZoneDetector;

            if (detector.TryGetComponent<BoxCollider>(out var box))
            {
                Vector3 forward = detector.transform.forward;
                float attackDistance = box.size.z * detector.transform.lossyScale.z;
                Debug.DrawRay(detector.transform.position, forward * attackDistance, Color.red, 1f);
            }
        }
    }
}
