using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
    public sealed class JumpComponent : IInitializable
    {
        private readonly Rigidbody _rigidbody;
        private readonly Transform _feetPosition;
        private readonly LayerMask _groundLayer;
        private readonly Controls _controls;      // Input Actions

        private readonly PlayerConfig _playerConfig;

        private int _jumpCount;
        
        public JumpComponent(Rigidbody rigidbody,
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
        }


        public void Dispose()
        {
            _controls.Player.Jump.performed -= OnJumpPerformed;
        }
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (!CanJump()) return;

            Jump();
        }

        private bool CanJump()
        {
            if (IsGrounded())
            {
                _jumpCount = 0;
                return true;
            }
            return _jumpCount < 2;            // в воздухе Ц только один двойной прыжок
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

    }
}