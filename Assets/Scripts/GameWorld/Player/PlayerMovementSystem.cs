
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{

    public sealed class PlayerMovementSystem : IInitializable, ITickable, IFixedTickable, IDisposable, IChangeOfForceHandler
    {
        public event Action<float> OnFallDistanceEvent;

        private readonly CharacterController _controller;
        private readonly Controls _controls;
        private readonly PlayerConfig _playerConfig;
        private readonly ISoundEventBus _soundBus;
        private readonly ICheckPointEventBus _checkPointEventBus;
        private readonly PlayerAnimator _playerAnimator;


        private int _jumpCount;
        private bool _flyPressed;
        private float _flyAdvancedSpeed;
        private float _moveAdvancedSpeed;
        private bool _isSprinting;
        private Vector2 _moveInput = Vector2.zero;

        private Vector3 _velocity = Vector3.zero;
        private float _externalSpeedModifier = 1f;

        private float _fallStartY;   // «null» — значит падение ещё не началось
        private bool _isFalling;
        private float _fallDistance;

        private float _airTime;
        private const float _landThreshold = 0.15f; // Порог времени для "настоящего" падения

        private float _acceleration = 40f;    // Скорость разгона
        private float _deceleration = 25f;    // Скорость торможения (инерция)
        private float _currentTraction = 1f; // 1.0 — асфальт, 0.2 — лед, 0.05 — супер-лед
        private float _defaultModifire = 1f;
        private Quaternion _faceRight = Quaternion.Euler(0, 90, 0);
        public bool IsMovementFrozen { get; set; }

        private float _animSpeedVelocity;

        private float _stepTimer;
        [SerializeField] private float _stepInterval = 0.4f; // Базовая задержка между шагами

        private PlayerStartParameters _startParameters;
        public PlayerMovementSystem(
            CharacterController controller,
            Controls controls,
            PlayerConfig playerConfig,
            ISoundEventBus soundBus,
            ICheckPointEventBus checkPointEventBus,
            PlayerStartParameters playerStartParameters,
            PlayerAnimator playerAnimator)
        {
            _controller = controller;
            _controls = controls;
            _playerConfig = playerConfig;
            _soundBus = soundBus;
            _checkPointEventBus = checkPointEventBus;
            _startParameters = playerStartParameters;
            _playerAnimator = playerAnimator;
        }

        public void Initialize()
        {
            _checkPointEventBus.CheckPointReached(_controller.gameObject.transform.position,
                                                _controller.gameObject.transform.rotation);
            _controls.Player.Move.started += OnMoveStarted;
            _controls.Player.Move.canceled += OnMoveCanceled;

            _controls.Player.Sprint.started += _ => _isSprinting = true;
            _controls.Player.Sprint.canceled += _ => _isSprinting = false;

            _controls.Player.Jump.started += OnJumpStarted;
            _controls.Player.Jump.canceled += OnJumpCanceled;

            _controls.Player.Fly.started += OnFlyStarted;
            _controls.Player.Fly.canceled += OnFlyCanceled;
        }

        public void Dispose()
        {
            _controls.Player.Move.started -= OnMoveStarted;
            _controls.Player.Move.canceled -= OnMoveCanceled;

            _controls.Player.Sprint.started -= _ => _isSprinting = true;
            _controls.Player.Sprint.canceled -= _ => _isSprinting = false;

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

        public void Tick()
        {
            ApplyAnimation();
            ApplyRotation();
        }

        private void ApplyAnimation()
        {
            float currentHorizontalSpeed = Mathf.Abs(_velocity.x);
            bool grounded = IsGrounded();
            bool wallSliding = !grounded && IsWallClinging() && _velocity.y < 0;

            // Расчет нормализованной скорости (нужен нам для частоты шагов)
            float walkSpeed = _playerConfig.MoveSpeedGround;
            float sprintSpeed = walkSpeed * _playerConfig.SprintSpeedMultiplayer;
            float normalizedSpeed;

            if (currentHorizontalSpeed <= walkSpeed)
            {
                normalizedSpeed = currentHorizontalSpeed / walkSpeed;
            }
            else
            {
                float runProgress = (currentHorizontalSpeed - walkSpeed) / (sprintSpeed - walkSpeed);
                normalizedSpeed = 1f + Mathf.Clamp01(runProgress);
            }

            // --- ЛОГИКА ЗЕМЛИ, ПРИЗЕМЛЕНИЯ И ШАГОВ ---
            if (grounded)
            {
                // 1. Звук и анимация приземления
                if (_airTime > _landThreshold)
                {
                    _playerAnimator.PlayLanding();
                    _soundBus.Play(SoundType.Land); // Проигрываем звук приземления
                }
                _airTime = 0;

                // 2. Звук шагов
                if (currentHorizontalSpeed > 0.1f)
                {
                    // Умножаем на normalizedSpeed, чтобы при беге шаги звучали чаще
                    _stepTimer -= Time.deltaTime * normalizedSpeed;

                    if (_stepTimer <= 0)
                    {
                        _soundBus.Play(SoundType.Step); // Проигрываем звук шага
                        _stepTimer = _stepInterval;     // Сброс таймера
                    }
                }
                else
                {
                    _stepTimer = 0; // Сброс таймера при остановке
                }
            }
            else
            {
                _airTime += Time.deltaTime;
                _stepTimer = 0; // Не шагаем в воздухе
            }
            // -------------------------------------------

            // Передаем данные в аниматор
            _playerAnimator.UpdateMovementStates(normalizedSpeed, grounded, wallSliding, _flyPressed);

            //float currentHorizontalSpeed = Mathf.Abs(_velocity.x);
            //bool grounded = IsGrounded();
            //bool wallSliding = !grounded && IsWallClinging() && _velocity.y < 0;

            //// --- НОВАЯ ЛОГИКА ФИЛЬТРАЦИИ ПРИЗЕМЛЕНИЯ ---
            //if (grounded)
            //{
            //    // Если мы коснулись земли и были в воздухе достаточно долго (не кочка)
            //    if (_airTime > _landThreshold)
            //    {
            //        _playerAnimator.PlayLanding(); // Вызываем анимацию приземления
            //    }
            //    _airTime = 0; // Сбрасываем таймер на земле
            //}
            //else
            //{
            //    _airTime += Time.deltaTime; // Копим время, пока мы в воздухе
            //}
            //// -------------------------------------------

            //// Нормализация скорости для Blend Tree (твой код без изменений)
            //float walkSpeed = _playerConfig.MoveSpeedGround;
            //float sprintSpeed = walkSpeed * _playerConfig.SprintSpeedMultiplayer;
            //float normalizedSpeed;

            //if (currentHorizontalSpeed <= walkSpeed)
            //{
            //    normalizedSpeed = currentHorizontalSpeed / walkSpeed;
            //}
            //else
            //{
            //    float runProgress = (currentHorizontalSpeed - walkSpeed) / (sprintSpeed - walkSpeed);
            //    normalizedSpeed = 1f + Mathf.Clamp01(runProgress);
            //}

            //// Передаем отфильтрованные данные в аниматор
            //_playerAnimator.UpdateMovementStates(normalizedSpeed, grounded, wallSliding, _flyPressed);


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

            return _jumpCount < _playerConfig.JumpCountInAir;
        }

        private void Jump()
        {
            if (!IsGrounded())
                _velocity.y = 0f;

            float effectiveJumpForce = _playerConfig.JumpForce * _externalSpeedModifier;

            _velocity.y += Mathf.Sqrt(2 * effectiveJumpForce);

            _jumpCount++;
            _soundBus.Play(SoundType.Jump);

        }

        private void WallJump()
        {
            bool wallOnRight = IsWallAtSide(1);
            bool wallOnLeft = IsWallAtSide(-1);

            float horizontalForce = 0f;
            if (wallOnRight)
            {
                horizontalForce = -_playerConfig.WallJumpForceX;
            }
            else if (wallOnLeft)
            {
                horizontalForce = _playerConfig.WallJumpForceX;
            }

            _velocity.x = horizontalForce;
            _velocity.y = _playerConfig.WallJumpForceY;

            _jumpCount++;
            _soundBus.Play(SoundType.WallJump);
        }

        public bool IsGrounded()
        {
            if (_controller.isGrounded)
            {
                _jumpCount = 0;
                return true;
            }
            return false;
        }

        private bool IsWallAtSide(float directionX)
        {
            Vector3 origin = _controller.bounds.center;

            // направление (влево или вправо)
            Vector3 dir = Vector3.right * directionX;

            float dynamicDistance = _controller.bounds.extents.x + 0.1f;

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

        private bool IsWallInFront()
        {
            Vector3 origin = _controller.bounds.center;

            Vector3 dir = _controller.transform.forward;

            float dynamicDistance = _controller.bounds.extents.x + 0.2f;

            Debug.DrawRay(origin, dir * dynamicDistance, Color.cyan);

            return Physics.Raycast(origin, dir, dynamicDistance, _playerConfig.LayerMaskForWall);
        }
        private bool IsWallClinging()
        {
            if (IsGrounded()) return false;

            return IsWallInFront();
        }
        private void ApplyMovement()
        {
            Vector2 effectiveInput = IsMovementFrozen ? Vector2.zero : _moveInput;

            float walkSpeed = _playerConfig.MoveSpeedGround;
            float sprintSpeed = walkSpeed * _playerConfig.SprintSpeedMultiplayer;
            float speedLimit = IsGrounded() ? (_isSprinting ? sprintSpeed : walkSpeed) : _playerConfig.MoveSpeedAir;

            float targetMaxSpeed = effectiveInput.x * speedLimit * _externalSpeedModifier;

            if (!IsMovementFrozen)
            {
                bool isTryingToMove = Mathf.Abs(effectiveInput.x) > 0.01f;
                float currentForce = isTryingToMove ? (_acceleration * _currentTraction) : (_deceleration * _currentTraction);

                if (!isTryingToMove && _externalSpeedModifier < 0.9f) currentForce *= 2f;

                _velocity.x = Mathf.MoveTowards(_velocity.x, targetMaxSpeed, currentForce * Time.fixedDeltaTime);

                if ((IsWallAtSide(-1) && _velocity.x < 0) || (IsWallAtSide(1) && _velocity.x > 0))
                {
                    _velocity.x = 0;
                }

                if (!isTryingToMove && Mathf.Abs(_velocity.x) < 0.1f)
                {
                    _velocity.x = 0f;
                }
            }
            else
            {
                Debug.Log($"FROZEN! Velocity X is: {_velocity.x}");
            }

            if (IsWallClinging() && _velocity.y < _playerConfig.WallSlideSpeed)
            {
                _velocity.y = Mathf.Lerp(_velocity.y, _playerConfig.WallSlideSpeed, _playerConfig.SlowClingFallSpeed);
            }
            else if (_flyPressed && _velocity.y < 0 && !IsMovementFrozen)
            {
                _velocity.y = Mathf.Lerp(_velocity.y - _flyAdvancedSpeed, -_playerConfig.JumpHoldFallAirSpeed, _playerConfig.SlowFallAirSpeed);

                _velocity.x += effectiveInput.x * _moveAdvancedSpeed * Time.fixedDeltaTime;
            }
            else
            {
                _velocity.y -= _playerConfig.Gravity * Time.fixedDeltaTime;
            }

            if (IsWallAtHead() && _velocity.y > 0)
            {
                _velocity.y = -0.1f;
            }

            Vector3 move = new Vector3(_velocity.x, _velocity.y, 0) * Time.fixedDeltaTime;
            if (_controller != null && _controller.enabled)
            {
                _controller.Move(move);
            }

            if (_controller.isGrounded && _velocity.y < -0.1f)
            {
                _velocity.y = 0f;
            }
        }
        private void ApplyRotation()
        {
            if (Mathf.Abs(_moveInput.x) > 0.1f)
            {
                float targetY = (_moveInput.x > 0) ? 90f : 270f;
                _startParameters.ViewTransform.localRotation = Quaternion.Euler(0, targetY, 0);
            }
        }

        private void UpdateFallState()
        {
            bool grounded = IsGrounded();
            bool wallClinging = IsWallClinging();
            bool isHeightControlled = grounded || wallClinging || (_flyPressed && _velocity.y < 0) || _velocity.y > 0;

            if (isHeightControlled)
            {
                if (grounded && _isFalling)
                {
                    _fallDistance = Mathf.Max(0, _fallStartY - _controller.transform.position.y);

                    if (_fallDistance > 0.1f)
                    {
                        if (_fallDistance > 1.5f)
                        {
                            _playerAnimator.PlayLanding();
                        }

                        OnFallDistanceEvent?.Invoke(_fallDistance);

                        Debug.Log($"Fall Distance Success: {_fallDistance}");
                    }
                    _isFalling = false;
                }

                _fallStartY = _controller.transform.position.y;
            }
            else
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                }
                _fallDistance = _fallStartY - _controller.transform.position.y;
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

        public void SetMovementLock(bool isLocked)
        {
            IsMovementFrozen = isLocked;
        }
        public void StopImmediately()
        {
            _velocity = Vector3.zero;     
            _moveInput = Vector2.zero;  
            IsMovementFrozen = true;     
        }


        public void MoveToCheckPoint(Vector3 positoin)
        {
            _controller.transform.position = positoin;
            _controller.transform.rotation = _faceRight;
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

        public async void ApplyKnockback(Vector3 sourcePosition, float force)
        {
            _controller.Move(Vector3.up * 0.1f);
            float diffX = _controller.transform.position.x - sourcePosition.x;
            float direction = Mathf.Sign(diffX);
            _velocity.x = direction * force;
            _velocity.y = force * 0.8f;

            await Task.Delay(250);
        }
    }
}

