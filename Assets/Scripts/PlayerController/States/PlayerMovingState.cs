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
            // flip sprite
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
            _context.Velocity = new Vector2(xSpeed, _context.Velocity.y);
        }
    }
}