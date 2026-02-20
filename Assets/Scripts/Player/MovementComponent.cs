
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{

    public sealed class MovementComponent : Zenject.IInitializable, IFixedTickable, IDisposable
    {
        private readonly CharacterController _controller;
        private readonly Controls _controls;
        private readonly PlayerConfig _playerConfig;
        private int _jumpCount;
        private Vector2 _moveInput = Vector2.zero;
        private float _rayDistance = 0.6f;
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _velocitySmoothRef = Vector3.zero;

        public MovementComponent(
            CharacterController controller,
            Controls controls,
            PlayerConfig playerConfig)
        {
            _controller = controller;
            _controls = controls;
            _playerConfig = playerConfig;
        }

        public void Initialize()
        {
            _controls.Player.Jump.started += OnJumpStarted;
            _controls.Player.Jump.canceled += OnJumpCanceled;

            _controls.Player.Move.started += OnMoveStarted;
            _controls.Player.Move.canceled += OnMoveCanceled;
        }

        public void Dispose()
        {
            _controls.Player.Jump.started -= OnJumpStarted;
            _controls.Player.Jump.canceled -= OnJumpCanceled;

            _controls.Player.Move.started -= OnMoveStarted;
            _controls.Player.Move.canceled -= OnMoveCanceled;
        }

        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            if (IsWallClinging()) // Прыжок от стены
            {
                WallJump();
                return;
            }

            if (!CanJump()) return;

            Jump();
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
        }

        private void OnMoveStarted(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;
        }

        private bool CanJump()
        {
            if (IsGrounded())
            {
                return true;
            }

            // двойной прыжок: пока _jumpCount < максимум разрешённых прыжков
            return _jumpCount < _playerConfig.JumpCountInAir;
        }

        private void Jump()
        {
            if (!IsGrounded())
                _velocity.y = 0f;

            _velocity.y += Mathf.Sqrt(2 * _playerConfig.JumpForce);
            _jumpCount++;
        }

        // Прыжок от стены — отталкиваемся по диагонали в противоположную сторону от стены
        private void WallJump()
        {
            bool wallOnRight = IsWallAtSide(1);
            bool wallOnLeft = IsWallAtSide(-1);

            // Направление отталкивания — противоположно стене
            float horizontalForce = 0f;
            if (wallOnRight) horizontalForce = -_playerConfig.WallJumpForceX;
            else if (wallOnLeft) horizontalForce = _playerConfig.WallJumpForceX;

            _velocity.x = horizontalForce;
            _velocity.y = _playerConfig.WallJumpForceY;

            _jumpCount++; // Замечаем прыжок

            // Можно "отрыв" от стены убрать, чтобы прыжок не считывался как земля
            // (если надо, например, сбросить к нулю _jumpCount можно добавить сюда)
        }

        private bool IsGrounded()
        {
            if (_controller.isGrounded)
            {
                _jumpCount = 0;
                return true;
            }
            return false;
        }

        // Проверяем наличие стены слева и справа возле персонажа
        private bool IsWallAtSide(float directionX)
        {
            Vector3 origin = _controller.transform.position + Vector3.up * 0.5f;
            Vector3 dir = _controller.transform.right * directionX;

            return Physics.Raycast(origin, dir, _rayDistance);
        }

        // Определяем, облокотился ли персонаж на стену (стена слева или справа + не на земле)
        private bool IsWallClinging()
        {
            if (IsGrounded())
                return false;

            bool wallLeft = IsWallAtSide(-1);
            bool wallRight = IsWallAtSide(1);

            return wallLeft || wallRight;
        }

        public void FixedTick()
        {
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            float speed = IsGrounded()
        ? _playerConfig.MoveSpeedGround
        : _playerConfig.MoveSpeedAir;

            Vector3 inputDirection = new Vector3(_moveInput.x, 0, 0);

            bool wallLeft = IsWallAtSide(-1);
            bool wallRight = IsWallAtSide(1);

            // Блокировка горизонтального движения в сторону стены
            if (wallLeft && inputDirection.x < 0)
                inputDirection.x = 0;

            if (wallRight && inputDirection.x > 0)
                inputDirection.x = 0;

            float targetSpeed = inputDirection.x * speed;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targetSpeed, ref _velocitySmoothRef.x, 0.1f);

            // Если персонаж облокотился на стену и не на земле — плавный спад по стене
            if (IsWallClinging() && _velocity.y < _playerConfig.WallSlideSpeed)
            {
                // Спускаемся по стене плавно с максимально заданной скоростью
                _velocity.y = Mathf.Lerp(_velocity.y, _playerConfig.WallSlideSpeed, _playerConfig.SlowClingFallSpeed);
            }
            else
            {
                // Если не на стене — применяем обычную гравитацию
                _velocity.y -= _playerConfig.Gravity * Time.fixedDeltaTime;
            }

            Vector3 move = new Vector3(_velocity.x, _velocity.y, 0) * Time.fixedDeltaTime;
            _controller.Move(move);

            if (_controller.isGrounded && _velocity.y < 0)
                _velocity.y = 0f;
        }
    }
}

