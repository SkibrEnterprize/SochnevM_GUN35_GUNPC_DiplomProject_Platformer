
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{

    public sealed class MovementComponent : IInitializable, IFixedTickable, IDisposable, IChangeOfForceHandler
    {
        public event Action<float> OnFallDistanceEvent;

        private readonly CharacterController _controller;
        private readonly Controls _controls;
        private readonly PlayerConfig _playerConfig;
        private readonly ISoundEventBus _soundBus;
        private readonly ICheckPointEventBus _checkPointEventBus;

        private int _jumpCount;
        private bool _flyPressed;
        private float _flyAdvancedSpeed;
        private float _moveAdvancedSpeed;
        private Vector2 _moveInput = Vector2.zero;
        private float _rayDistanceAtWall = 0.6f;
        private float _rayDistanceAtHead = 0.1f;

        private Vector3 _velocity = Vector3.zero;
        private Vector3 _velocitySmoothRef = Vector3.zero;

        private float _fallStartY;   // «null» — значит падение ещё не началось
        private bool _isFalling;
        private float _fallDistance;

        public MovementComponent(
            CharacterController controller,
            Controls controls,
            PlayerConfig playerConfig,
            ISoundEventBus soundBus,
            ICheckPointEventBus checkPointEventBus)
        {
            _controller = controller;
            _controls = controls;
            _playerConfig = playerConfig;
            _soundBus = soundBus;
            _checkPointEventBus = checkPointEventBus;
        }

        public void Initialize()
        {
            _checkPointEventBus.CheckPointReached(_controller.gameObject.transform.position,
                                                _controller.gameObject.transform.rotation);
            _controls.Player.Move.started += OnMoveStarted;
            _controls.Player.Move.canceled += OnMoveCanceled;

            _controls.Player.Jump.started += OnJumpStarted;
            _controls.Player.Jump.canceled += OnJumpCanceled;

            _controls.Player.Fly.started += OnFlyStarted;
            _controls.Player.Fly.canceled += OnFlyCanceled;

        }

        public void Dispose()
        {
            _controls.Player.Move.started -= OnMoveStarted;
            _controls.Player.Move.canceled -= OnMoveCanceled;

            _controls.Player.Jump.started -= OnJumpStarted;
            _controls.Player.Jump.canceled -= OnJumpCanceled;

            _controls.Player.Fly.started -= OnFlyStarted;
            _controls.Player.Fly.canceled -= OnFlyCanceled;

        }
        private void OnFlyStarted(InputAction.CallbackContext context)
        {
            _flyPressed = true;
        }

        private void OnFlyCanceled(InputAction.CallbackContext context)
        {
            _flyPressed = false;
        }


        public void FixedTick()
        {
            ApplyMovement();
            UpdateFallState();
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
            _soundBus.Play(SoundType.Jump);
            //_soundLibrary.RequestPlay(SoundType.Jump);
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
            _soundBus.Play(SoundType.WallJump);
            //_soundLibrary.RequestPlay(SoundType.SideJump);
            
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
            Vector3 origin = _controller.transform.position + Vector3.up * (_controller.height / 2f);
            Vector3 dir = _controller.transform.right * directionX;

            return Physics.Raycast(origin, dir, _rayDistanceAtWall, _playerConfig.LayerMaskForWall);
        }

        private bool IsWallAtHead()
        {
            Vector3 origin = _controller.transform.position + Vector3.up * (_controller.height / 2f);
            return Physics.Raycast(origin, Vector3.up, _rayDistanceAtHead);
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

        private void ApplyMovement()
        {
            float speed = IsGrounded()
        ? _playerConfig.MoveSpeedGround
        : _playerConfig.MoveSpeedAir;

            Vector3 inputDirection = new Vector3(_moveInput.x, 0, 0);

            bool wallLeft = IsWallAtSide(-1);
            bool wallRight = IsWallAtSide(1);

            if (wallLeft && inputDirection.x < 0)
                inputDirection.x = 0;

            if (wallRight && inputDirection.x > 0)
                inputDirection.x = 0;

            float targetSpeed = inputDirection.x * speed;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targetSpeed, ref _velocitySmoothRef.x, 0.1f);

            // Плавное скольжение по стене
            if (IsWallClinging() && _velocity.y < _playerConfig.WallSlideSpeed)
            {
                _velocity.y = Mathf.Lerp(_velocity.y, _playerConfig.WallSlideSpeed, _playerConfig.SlowClingFallSpeed);
            }
            else
            {
                // Если удерживается кнопка прыжка и персонаж в воздухе с падением вниз — плавное снижение скорости падения
                if (_flyPressed && _velocity.y < 0)
                {
                    _velocity.y = Mathf.Lerp(_velocity.y
                        - _flyAdvancedSpeed,
                        -_playerConfig.JumpHoldFallAirSpeed,
                        _playerConfig.SlowFallAirSpeed);
                    _velocity.x += _moveAdvancedSpeed;
                }
                else
                {
                    _velocity.y -= _playerConfig.Gravity * Time.fixedDeltaTime;
                }
            }

            Vector3 move = new Vector3(_velocity.x, _velocity.y, 0) * Time.fixedDeltaTime;
            _controller.Move(move);

            if (_controller.isGrounded && _velocity.y < 0 || IsWallAtHead()) _velocity.y = 0f;

        }

        private void UpdateFallState()
        {
            if (IsWallClinging()) _isFalling = false;

            if (!_controller.isGrounded && !_isFalling)
            {
                _isFalling = true;
                _fallStartY = _controller.transform.position.y;
            }

            if (_controller.isGrounded && _isFalling)
            {
                _isFalling = false;
                _fallDistance = Mathf.Abs(_fallStartY - _controller.transform.position.y);
                OnFallDistanceEvent?.Invoke(_fallDistance);
            }
        }

        public void ChangeForceByTrigger(bool isAddForce, float flySpeed, float moveSpeed)
        {
            if (isAddForce)
            {
                _flyAdvancedSpeed += flySpeed;
                _moveAdvancedSpeed += moveSpeed;
            }
            else
            {
                _flyAdvancedSpeed -= flySpeed;
                _moveAdvancedSpeed -= moveSpeed;
            }
        }

        public void MoveToCheckPoint(Vector3 positoin, Quaternion rotation)
        {
            _controller.transform.position = positoin;
            _controller.transform.rotation = rotation;
        }

        public void ApplyImpulse(Vector3 impulse)
        {
            // «Мгновенно» меняем скорость, но оставляем её на следующем FixedTick()
            _velocity += impulse;
        }
    }
}

