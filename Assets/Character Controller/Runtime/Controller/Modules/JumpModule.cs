using UnityEngine;
using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class JumpModule
    {
        public bool Request;
        public bool HandleLongJumps;
        public bool IsActiveCoyoteTime;
        public float LastPressedJumpTime;
        
        public int AdditionalJumpsAvailable
        {
            get => _additionalJumpsAvailable;
            set => _additionalJumpsAvailable = Mathf.Clamp(value, 0, _additionalJumps);
        }

        private readonly Rigidbody2D _body;

        private int _additionalJumpsAvailable;
        private readonly int _additionalJumps;
        private float _inputBuffer;
        
        public JumpModule(Rigidbody2D body, int additionalJumps, float inputBufferDuration)
        {
            _body = body;
            _additionalJumps = additionalJumps;
            _inputBuffer = inputBufferDuration;
        }

        public void OnInput(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                Request = true;
                LastPressedJumpTime = _inputBuffer;
            }
            
            // if still pressing jump button, perform long jump
            HandleLongJumps = context.ReadValueAsButton();
        }

        public void Jump(float jumpForce)
        {
            _body.velocity = new Vector2(_body.velocity.x, jumpForce);
            Request = false;
        }

        public void ResetAdditionalJumps() => _additionalJumpsAvailable = _additionalJumps;
    }
}