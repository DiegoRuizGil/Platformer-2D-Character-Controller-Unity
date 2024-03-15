using UnityEngine;

namespace PlayerController.States
{
    public class PlayerMovingState : PlayerBaseState, IHandleHorizontalMovement
    {
        public PlayerMovingState(PlayerStates key, PlayerStateMachine context)
            : base(key, context)
        {
            IsRootState = false;
        }

        public override void EnterState() { }

        public override void UpdateState()
        {
            HandleHorizontalMovement();
            _context.FlipSprite();
            
            _context.Animator.SetBool(
                _context.IdleHash,
                _context.LeftWallHit || _context.RightWallHit);
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (_context.MovementDirection.x == 0f)
                return PlayerStates.Idle;

            return StateKey;
        }

        protected override void InitializeSubState() { }

        public void HandleHorizontalMovement()
        {
            float xSpeed = _context.MaxSpeed * _context.MovementDirection.x;
            if (_context.LeftWallHit && _context.MovementDirection.x < 0)
                xSpeed = 0f;
            else if (_context.RightWallHit && _context.MovementDirection.x > 0)
                xSpeed = 0f;
            
            _context.SetHorizontalVelocity(xSpeed);
        }
    }
}