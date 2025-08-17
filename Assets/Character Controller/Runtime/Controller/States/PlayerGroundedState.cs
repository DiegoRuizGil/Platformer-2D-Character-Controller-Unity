namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerGroundedState : PlayerBaseState
    {
        public PlayerGroundedState(PlayerStates key, PlayerController context)
            : base(key, context)
        {
            _lerpAmount = 1f;
            _canAddBonusJumpApex = false;
        }

        public override void EnterState()
        {
            // reset additional jumps and dash
            Context.ResetAdditionalJumps();
            Context.IsDashActive = true;
            
            Context.SetGravityScale(Context.Data.gravityScale);
            
            if (!Context.JumpRequest)
                Context.InstantiateFallDustVFX();
        }

        public override void UpdateState() { }

        public override void FixedUpdateState()
        {
            Context.Run(_lerpAmount, _canAddBonusJumpApex);
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            // set coyote time just when falling
            if (!Context.IsGrounded)
            {
                Context.IsActiveCoyoteTime = true;
                return PlayerStates.Falling;
            }
            
            if (Context.JumpRequest)
            {
                Context.IsActiveCoyoteTime = false;
                return PlayerStates.Jumping;
            }

            if (Context.DashRequest && Context.CanDash)
                return PlayerStates.Dashing;
            
            return StateKey;
        }
    }
}