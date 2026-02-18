
using System;
using Unity.VisualScripting;
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
        private Vector2 _moveInput = Vector2.zero;          // направление

        private bool _jumpPressed = false;
        private float _rayDistance = 0.6f;

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
            //_controls.Player.Move.performed += OnMoveStarted;
            _controls.Player.Move.canceled += OnMoveCanceled;


        }

        public void Dispose()
        {
            _controls.Player.Jump.started -= OnJumpStarted;
            _controls.Player.Jump.canceled -= OnJumpCanceled;

            _controls.Player.Move.started -= OnMoveStarted;
            //_controls.Player.Move.performed -= OnMoveStarted;
            _controls.Player.Move.canceled -= OnMoveCanceled;
        }

        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            if (!CanJump()) return;
            Jump();
            _jumpPressed = true;
        }
        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            _jumpPressed = true;
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
            if (_controller.isGrounded)
            {
                _jumpCount = 0;
                return true;
            }
            return false;
        }
        private bool IsClinging()
        {
            Vector3 direction = new Vector3(_moveInput.x, 0, 0);
            if (direction == Vector3.zero)
                return false;

            return Physics.Raycast(_controller.transform.position, direction, _rayDistance);
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

            var vel = _controller.velocity;
            bool clinging = !IsGrounded() && IsClinging();
            // ---------- Горизонтальное движение ----------
            if (_moveInput == Vector2.zero)
            {
                if (IsGrounded())
                {
                    // На земле: полностью обнуляем X
                    //vel.x = Mathf.Lerp(vel.x, 0f, _playerConfig.DampGround);
                    vel.x = 0;
                    vel.y = 0;
                }
                else
                {
                    // В воздухе: мягкое торможение
                    vel.x = Mathf.Lerp(vel.x, 0f, _playerConfig.DampAir);
                }
            }
            else
            {
                // Когда есть ввод – задаём скорость по X напрямую
                vel.x = _moveInput.x * speed;
            }

            // ---------- Вертикальное движение ----------
            if (clinging)
            {
                Debug.Log("IS Clinging!");
                vel.y = -_playerConfig.SlowClingFallSpeed;
                _jumpCount = _playerConfig.JumpCountInAir - 1;
            }
            else
            {
                // Отпрыгивание от стены(если игрок всё ещё рядом с ней)
                Vector3 wallNormal = GetWallNormal();
                if (wallNormal != Vector3.zero && !IsGrounded())
                {
                    Debug.Log("Wall jump!");
                    //// Добавляем вертикальную и горизонтальную скорость отталкивания
                    //// Вертикальная часть – вверх
                    vel.y += _playerConfig.WallJumpForce;
                    // Горизонтальная часть – вдоль нормали стены (в сторону, от стены)
                    vel.x += wallNormal.x * _playerConfig.WallHorizontalPush;
                }


            }
            if (!IsGrounded())
                vel.y -= _playerConfig.Gravity * Time.fixedDeltaTime;

            _controller.Move(vel * Time.fixedDeltaTime);
        }

        private Vector3 GetWallNormal()
        {
            // Проверяем справа
            if (Physics.Raycast(_controller.transform.position, Vector3.right,
                                _rayDistance))
                //Debug.Log("NORMAL-Right");
                return -Vector3.right;   // противоположная нормаль – влево

            // Проверяем слева
            if (Physics.Raycast(_controller.transform.position, Vector3.left,
                                _rayDistance))
                //Debug.Log("NORMAL-Left");
                return Vector3.right;     // нормаль вправо

            //Debug.Log("NORMAL-Zero!");
            return Vector3.zero;          // нет стены
        }
    }
}