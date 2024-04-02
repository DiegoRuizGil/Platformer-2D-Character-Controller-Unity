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
            
            _context.AnimSetIdleVariable(true);
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            _context.AnimSetIdleVariable(false);
        }

        public override PlayerStates GetNextState()
        {
            if (_context.MovementDirection.x != 0)
                return PlayerStates.Moving;
            
            return StateKey;
        }

        protected override void InitializeSubState() { }
        
        public void HandleHorizontalMovement()
        {
            _context.SetHorizontalVelocity(0f);
        }
    }
}