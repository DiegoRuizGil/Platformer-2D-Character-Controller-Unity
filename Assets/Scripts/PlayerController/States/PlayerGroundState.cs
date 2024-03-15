using UnityEngine;

namespace PlayerController.States
{
    public class PlayerGroundState : PlayerBaseState, IHandleGravity
    {
        public PlayerGroundState(PlayerStates key, PlayerStateMachine context)
            : base(key, context)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            InitializeSubState();
            HandleGravity();
        }

        public override void UpdateState() { }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (!_context.IsGrounded)
            {
                _context.HasCoyoteTime = true;
                return PlayerStates.Falling;
            }

            if (_context.JumpRequests > 0 && _context.IsGrounded)
            {
                _context.ManageJumpRequest();
                _context.HasCoyoteTime = false;
                return PlayerStates.Jumping;
            }
            
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
            _context.SetVerticalVelocity(0f);
        }
    }
}