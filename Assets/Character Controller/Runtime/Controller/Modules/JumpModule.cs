using UnityEngine;
using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class JumpModule
    {
        public bool InputRequest;
        public bool HandleLongJumps;
        public bool IsActiveCoyoteTime;
        
        public int AdditionalJumpsAvailable
        {
            get => _additionalJumpsAvailable;
            set => _additionalJumpsAvailable = Mathf.Clamp(value, 0, _additionalJumps);
        }

        private readonly Rigidbody2D _body;
        private readonly PlayerVFX _playerVFX;

        private int _additionalJumpsAvailable;
        private readonly int _additionalJumps;
        private readonly Timer _inputBuffer;
        
        public JumpModule(Rigidbody2D body, PlayerVFX playerVFX, PlayerMovementData data)
        {
            _body = body;
            _playerVFX = playerVFX;
            _additionalJumps = data.additionalJumps;
            _inputBuffer = new Timer(data.jumpInputBufferTime);
        }

        public void OnInput(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                InputRequest = true;
                _inputBuffer.Reset();
            }
            
            // if still pressing jump button, perform long jump
            HandleLongJumps = context.ReadValueAsButton();
        }

        public void HandleInputBuffer(float delta)
        {
            if (!InputRequest) return;
            
            _inputBuffer.Tick(delta);
            if (_inputBuffer.Finished)
                InputRequest = false;
        }

        public void Jump(float jumpForce)
        {
            _body.velocity = new Vector2(_body.velocity.x, jumpForce);
            InputRequest = false;
            
            _playerVFX.InstantiateJumpDustVFX();
        }
        
        public void WallJump(Vector2 jumpForce, int direction)
        {
            _body.velocity = new Vector2(jumpForce.x * direction, jumpForce.y);
            InputRequest = false;
            
            _playerVFX.InstantiateJumpDustVFX();
        }

        public void ResetAdditionalJumps() => _additionalJumpsAvailable = _additionalJumps;
    }
}