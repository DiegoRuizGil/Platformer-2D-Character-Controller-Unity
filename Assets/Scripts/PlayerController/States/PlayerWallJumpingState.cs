using UnityEngine;

namespace PlayerController.States
{
    public class PlayerWallJumpingState : PlayerBaseState, IHandleGravity
    {
        public PlayerWallJumpingState(PlayerStates key, PlayerStateMachine context)
            : base(key, context)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            PerformJump();
        }

        public override void UpdateState()
        {
            if (_context.IsTouchingCeiling)
                _context.SetVerticalVelocity(0f);
            
            HandleGravity();
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (_context.Velocity.y <= 0)
                return PlayerStates.Falling;

            // if (_context.CanWallSlide() && _subStateInitialized)
            //     return PlayerStates.WallSliding;

            return StateKey;
        }

        protected override void InitializeSubState()
        {
            SetSubState(_context.MovementDirection == Vector2.zero
                ? _context.States[PlayerStates.Idle]
                : _context.States[PlayerStates.Moving]);
        }

        public void HandleGravity()
        {
            float ySpeed = _context.Velocity.y + _context.Gravity * Time.deltaTime;
            _context.SetVerticalVelocity(ySpeed);
        }
        
        private void PerformJump()
        {
            float xSpeed = _context.WallJumpHorizontalSpeed;
            if (_context.RightWallHit) xSpeed *= -1f;
            _context.SetHorizontalVelocity(xSpeed);

            float ySpeed = Mathf.Abs(xSpeed) * _context.verticalVelocityMultiplier;
            _context.SetVerticalVelocity(ySpeed);
        }
    }
}