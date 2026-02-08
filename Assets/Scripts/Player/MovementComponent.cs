
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
    public sealed class MovementComponent : IInitializable, IFixedTickable, IDisposable
    {
        private readonly CharacterController _controller;
        private readonly Controls _controls;

        private readonly PlayerConfig _playerConfig;

        private int _jumpCount;
        private Vector2 _moveInput = Vector2.zero;          // направление

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
            _controls.Player.Jump.performed += OnJumpPerformed;
            _controls.Player.Move.started += OnMoveStarted;
            _controls.Player.Move.performed += OnMoveStarted;
            _controls.Player.Move.canceled += OnMoveCanceled;
        }

        public void Dispose()
        {
            _controls.Player.Jump.performed -= OnJumpPerformed;
            _controls.Player.Move.started -= OnMoveStarted;
            _controls.Player.Move.canceled -= OnMoveCanceled;
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (!CanJump()) return;
            Jump();
        }

        private void OnMoveStarted(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput = Vector2.zero;          // можно просто сбросить
        }


        private bool CanJump()
        {
            if (IsGrounded())
            {
                //_jumpCount = 0;
                return true;
            }
            
            return _jumpCount < _playerConfig.JumpCountInAir;
        }

        private void Jump()
        {
           
            var velocity = _controller.velocity;
            if (!IsGrounded()) velocity.y = 0f;   // обнуляем, если в воздухе
            velocity.y += Mathf.Sqrt(2 * _playerConfig.JumpForce);
            _controller.Move(velocity * Time.fixedDeltaTime); 

            _jumpCount++;
        }

              private bool IsGrounded()
        {
            // Можно использовать готовый флаг
            if (_controller.isGrounded)
            {
                _jumpCount = 0;
                return true;
            }
            return false;
        }

        public void FixedTick()
        {
            ApplyMovement();
            Debug.Log($" _jumpCount = {_jumpCount}");
        }

        private void ApplyMovement()
        {
            float speed = IsGrounded()
                ? _playerConfig.MoveSpeedGround
                : _playerConfig.MoveSpeedAir;

            var velocity = _controller.velocity;

            if (_moveInput == Vector2.zero && !IsGrounded())
            {
                // Плавное торможение во время полёта
                velocity.x = Mathf.Lerp(velocity.x, 0f, _playerConfig.DampAir);
            }
            else if (_moveInput == Vector2.zero && IsGrounded())
            {
                // Плавное торможение на земле
                velocity.x = Mathf.Lerp(velocity.x, 0f, _playerConfig.DampGround);
            }
            else
            {
                velocity.x = _moveInput.x * speed;
            }

            // Гравитация (если не grounded)
            if (!IsGrounded())
                velocity.y -= _playerConfig.Gravity * Time.fixedDeltaTime;

            _controller.Move(velocity * Time.fixedDeltaTime);

            //ограничение движения по оси Z заменил коллайдерами
            //Vector3 pos = _controller.transform.position;
            //pos.z = 0;
            //_controller.transform.position = pos;
        }
    }
}