
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

        private Vector3 _velocity = Vector3.zero;
        private float _externalSpeedModifier = 1f;

        private float _fallStartY;   // «null» — значит падение ещё не началось
        private bool _isFalling;
        private float _fallDistance;

        private float _acceleration = 40f;    // Скорость разгона
        private float _deceleration = 25f;    // Скорость торможения (инерция)
        private float _currentTraction = 1f; // 1.0 — асфальт, 0.2 — лед, 0.05 — супер-лед

        private float _defaultModifire = 1f;
        public bool IsMovementFrozen { get; set; }
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

            // Применяем модификатор к силе прыжка. 
            // Если _externalSpeedModifier = 0.5 (болото), прыжок станет заметно ниже.
            float effectiveJumpForce = _playerConfig.JumpForce * _externalSpeedModifier;

            _velocity.y += Mathf.Sqrt(2 * effectiveJumpForce);

            _jumpCount++;
            _soundBus.Play(SoundType.Jump);

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

        // проверяем наличие стены слева и справа возле персонажа
        private bool IsWallAtSide(float directionX)
        {
            // 1. Центр физической капсулы в мировых координатах
            // Это всегда будет "середина" игрока, как бы ты ни менял масштаб или центр
            Vector3 origin = _controller.bounds.center;

            // 2. Направление (влево или вправо)
            Vector3 dir = _controller.transform.right * directionX;

            // 3. Дистанция: Берем текущий горизонтальный размер (extents.x — это радиус в мире)
            // + небольшой запас 0.1f наружу
            float dynamicDistance = _controller.bounds.extents.x + 0.1f;

            // Рисуем луч из центра игрока сквозь плечо наружу
            Debug.DrawRay(origin, dir * dynamicDistance, Color.red);

            return Physics.Raycast(origin, dir, dynamicDistance, _playerConfig.LayerMaskForWall);
        }


        private bool IsWallAtHead()
        {
            Vector3 origin = _controller.bounds.center + Vector3.up * _controller.bounds.extents.y;
            origin.y -= 0.05f;
            float headCheckDistance = 0.2f;
            Debug.DrawRay(origin, Vector3.up * headCheckDistance, Color.green);
            return Physics.Raycast(origin, Vector3.up, headCheckDistance, _playerConfig.LayerMaskForWall);

        }

        // определяем, облокотился ли персонаж на стену (стена слева или справа + не на земле)
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
            if (IsMovementFrozen) return;

            float baseSpeed = IsGrounded() ? _playerConfig.MoveSpeedGround : _playerConfig.MoveSpeedAir;
            float targetMaxSpeed = _moveInput.x * baseSpeed * _externalSpeedModifier;

            float currentForce;
            bool isTryingToMove = Mathf.Abs(_moveInput.x) > 0.01f;

            if (isTryingToMove)
            {
                currentForce = _acceleration * _currentTraction;
            }
            else
            {
                if (_externalSpeedModifier < 0.9f)
                {
                    currentForce = _deceleration * 2f;
                }
                else
                {
                    currentForce = _deceleration * _currentTraction;
                }
            }

            // применяем горизонтальную силу
            _velocity.x = Mathf.MoveTowards(_velocity.x, targetMaxSpeed, currentForce * Time.fixedDeltaTime);

            // проверка стен (чтобы инерция не толкала сквозь стены)
            bool wallLeft = IsWallAtSide(-1);
            bool wallRight = IsWallAtSide(1);
            if (wallLeft && _velocity.x < 0) _velocity.x = 0;
            if (wallRight && _velocity.x > 0) _velocity.x = 0;

            // вертикальная логика (Стены, Fly, Гравитация)
            if (IsWallClinging() && _velocity.y < _playerConfig.WallSlideSpeed)
            {
                _velocity.y = Mathf.Lerp(_velocity.y, _playerConfig.WallSlideSpeed, _playerConfig.SlowClingFallSpeed);
            }
            else
            {
                if (_flyPressed && _velocity.y < 0)
                {
                    _velocity.y = Mathf.Lerp(_velocity.y - _flyAdvancedSpeed,
                                            -_playerConfig.JumpHoldFallAirSpeed,
                                            _playerConfig.SlowFallAirSpeed);
                    // Добавочное ускорение при парении
                    _velocity.x += _moveInput.x * _moveAdvancedSpeed * Time.fixedDeltaTime;
                }
                else
                {
                    _velocity.y -= _playerConfig.Gravity * Time.fixedDeltaTime;
                }
            }
           
            Vector3 move = new Vector3(_velocity.x, _velocity.y, 0) * Time.fixedDeltaTime;
            _controller.Move(move);

            if (IsWallAtHead() && _velocity.y > 0)
            {
                Debug.Log("Head!!!");
                _velocity.y = -0.1f; // Небольшой импульс вниз, чтобы "отлепиться"
            }
           
            if (_controller.isGrounded && _velocity.y < 0)
            {
                _velocity.y = 0f;
            }
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
            _velocity += impulse;
        }
        public void SetSurfaceEffect(float speedModifier, float traction)
        {
            _externalSpeedModifier = speedModifier;
            _currentTraction = traction;
        }

        public void ResetSurfaceEffect()
        {
            _externalSpeedModifier = _defaultModifire;
            _currentTraction = _defaultModifire;
        }
    }
}

