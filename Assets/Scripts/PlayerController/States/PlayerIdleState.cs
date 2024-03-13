using UnityEngine;

namespace PlayerController.States
{
    public class PlayerIdleState : PlayerBaseState, IHandleHorizontalMovement
    {
        public PlayerIdleState(PlayerStates key, PlayerStateMachine context)
            : base(key, context)
        {
            IsRootState = false;
        }

        public override void EnterState()
        {
            HandleHorizontalMovement();
        }

        public override void UpdateState() { }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (_context.MovementDirection.x != 0)
                return PlayerStates.Moving;
            
            return StateKey;
        }

        protected override void InitializeSubState() { }
        
        public void HandleHorizontalMovement()
        {
            _context.Velocity = new Vector2(0f, _context.Velocity.y);
        }
    }
}