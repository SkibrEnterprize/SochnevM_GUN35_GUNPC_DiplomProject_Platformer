using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
    public sealed class MovementComponent : IInitializable, IFixedTickable, IDisposable
    {
        private readonly Rigidbody _rigidbody;
        private readonly Transform _feetPosition;
        private readonly LayerMask _groundLayer;
        private readonly Controls _controls;      // Input Actions

        private readonly PlayerConfig _playerConfig;

        private int _jumpCount;
        private float _moveSpeedGround;
        private float _moveSpeedAir;

        private Vector2 _moveInput = Vector2.zero; // знать куда смотрит игрок

        public MovementComponent(Rigidbody rigidbody,
                         Transform feetPosition,
                         LayerMask groundLayer,
                         Controls controls,
                         PlayerConfig playerConfig)
        {
            _rigidbody = rigidbody;
            _feetPosition = feetPosition;
            _groundLayer = groundLayer;
            _controls = controls;
            _playerConfig = playerConfig;
        }

        public void Initialize()
        {
            _controls.Player.Jump.performed += OnJumpPerformed;
            _controls.Player.Move.performed += OnMoveStarted;
            _controls.Player.Move.started += OnMoveStarted;
            _controls.Player.Move.canceled += OnMoveCanceled;
            _moveSpeedGround = _playerConfig.MoveSpeedGround;
            _moveSpeedAir = _playerConfig.MoveSpeedAir;

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
            _moveInput = context.ReadValue<Vector2>(); 
        }
        private bool CanJump()
        {
            if (IsGrounded())
            {
                _jumpCount = 0;
                return true;
            }
            return _jumpCount < 2;            // в воздухе – только один двойной прыжок
        }

        private void Jump()
        {
            var vel = _rigidbody.velocity;
            vel.y = 0;
            _rigidbody.velocity = vel;
            _rigidbody.AddForce(Vector3.up * _playerConfig.JumpForce, ForceMode.Force);
            _jumpCount++;          
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(_feetPosition.position,
                                   Vector3.down,
                                   _playerConfig.GroundCheckDistance,
                                   _groundLayer);
        }
       
        public void FixedTick()
        {
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            if (_moveInput != Vector2.zero)
            {               
                float speed = IsGrounded() ? _playerConfig.MoveSpeedGround
                                           : _playerConfig.MoveSpeedAir;
                float desiredX = _moveInput.x * speed;
                Vector3 deltaV = new Vector3(desiredX - _rigidbody.velocity.x, 0f, 0f);
                Vector3 acceleration = deltaV / Time.fixedDeltaTime;
                _rigidbody.AddForce(acceleration, ForceMode.Acceleration);
               
            }
            else
            {
                // при отпускании клавиши – мягко тормоpзимм (не обнуляем сразу)
                var vel = _rigidbody.velocity;
                vel.x = Mathf.Lerp(vel.x, 0f, 0.1f);   // можно настроить коэффициент
                _rigidbody.velocity = vel;
            }
           
        }
    }
}