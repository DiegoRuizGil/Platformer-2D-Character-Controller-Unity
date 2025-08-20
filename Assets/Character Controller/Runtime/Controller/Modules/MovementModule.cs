using UnityEngine;
using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class MovementModule
    {
        public Vector2 Direction => _inputAction.ReadValue<Vector2>();

        public bool IsFacingRight;

        private readonly Rigidbody2D _body;
        private readonly PlayerVFX _playerVFX;
        private readonly InputAction _inputAction;
        
        public MovementModule(Rigidbody2D body, PlayerVFX playerVFX)
        {
            _body = body;
            _playerVFX = playerVFX;
            _inputAction = InputManager.PlayerActions.Movement;
            
            IsFacingRight = true;
        }

        public void SetGravityScale(float scale) => _body.gravityScale = scale;

        public void Move(float speed, float acceleration)
        {
            float increment = Direction.x * acceleration;
            float newSpeed = Mathf.Clamp(_body.velocity.x + increment, -speed, speed);
            _body.velocity = new Vector2(newSpeed, _body.velocity.y);
        }

        public void Slide(float speed, float acceleration)
        { 
            if (_body.velocity.y > 0)
                _body.velocity = new Vector2(_body.velocity.x, 0);
            
            float newSpeed = Mathf.Clamp(_body.velocity.y - acceleration, -speed, 0);
            _body.velocity = new Vector2(_body.velocity.x, newSpeed);
        }
        
        public void ApplyFriction(float decay) => _body.velocity *= decay;
        public void ApplyHorizontalFriction(float decay) => _body.velocity = new Vector2(_body.velocity.x * decay, _body.velocity.y);

        public void SetDirectionToFace(bool isMovingRight, bool isGrounded)
        {
            if (isMovingRight != IsFacingRight)
            {
                IsFacingRight = !IsFacingRight;
                if (isGrounded)
                    _playerVFX.InstantiateFlipDirectionVFX(IsFacingRight);
            }
        }
    }
}