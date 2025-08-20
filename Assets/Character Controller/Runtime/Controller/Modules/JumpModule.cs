using UnityEngine;

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
        
        public JumpModule(Rigidbody2D body, int additionalJumps)
        {
            _body = body;
            _additionalJumps = additionalJumps;
        }

        public void Jump(float jumpForce)
        {
            _body.velocity = new Vector2(_body.velocity.x, jumpForce);
            Request = false;
        }

        public void ResetAdditionalJumps() => _additionalJumpsAvailable = _additionalJumps;
    }
}