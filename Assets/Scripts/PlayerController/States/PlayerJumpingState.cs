using StateMachine.Hierarchical;

namespace PlayerController.States
{
    public class PlayerJumpingState : PlayerBaseState
    {
        public PlayerJumpingState(PlayerStates key, PlayerStateMachine context)
            : base(key, context)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            throw new System.NotImplementedException();
        }

        public override void ExitState()
        {
            throw new System.NotImplementedException();
        }

        public override PlayerStates GetNextState()
        {
            throw new System.NotImplementedException();
        }

        protected override void InitializeSubState()
        {
            throw new System.NotImplementedException();
        }
    }
}