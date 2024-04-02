using UnityEngine;

namespace PlayerController.States
{
    public class PlayerWallSlidingState : PlayerBaseState, IHandleGravity
    {
        public PlayerWallSlidingState(PlayerStates key, PlayerStateMachine context)
            : base(key, context)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            _context.SetVerticalVelocity(0f);
            InitializeSubState();
        }

        public override void UpdateState()
        {
            HandleGravity();
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (_context.IsGrounded)
                return PlayerStates.Grounded;

            if (_context.JumpRequests > 0)
            {
                _context.ManageJumpRequest();
                return PlayerStates.WallJumping;
            }

            if (!_context.LeftWallHit && !_context.RightWallHit)
                return PlayerStates.Falling;
            
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
            float ySpeed = _context.WallSlidingVelocity * -1f;
            _context.SetVerticalVelocity(ySpeed);
        }
    }
}