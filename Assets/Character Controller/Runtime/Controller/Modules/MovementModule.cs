using UnityEngine;
using UnityEngine.InputSystem;

namespace Character_Controller.Runtime.Controller.Modules
{
    public class MovementModule
    { 
        public bool IsFacingRight;

        private readonly Rigidbody2D _body;
        private readonly PlayerVFX _playerVFX;
        
        public MovementModule(Rigidbody2D body, PlayerVFX playerVFX)
        {
            _body = body;
            _playerVFX = playerVFX;
            
            IsFacingRight = true;
        }

        public void SetGravityScale(float scale) => _body.gravityScale = scale;

        public void Move(Vector2 direction, float speed, float acceleration)
        {
            float increment = direction.x * acceleration;
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