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

        public override void UpdateState()
        {
            throw new System.NotImplementedException();
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
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
            _context.Velocity = new Vector2(_context.Velocity.x, 0f);
        }
    }
}