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
            Context.DashModule.IsActive = true;
            
            Context.MovementModule.SetGravityScale(Context.Data.gravityScale);
            
            if (!Context.JumpModule.Request)
                Context.InstantiateFallDustVFX();
        }

        public override void UpdateState() { }

        public override void FixedUpdateState()
        {
            Context.MovementModule.Move(Context.Data.runMaxSpeed, Context.Data.acceleration);
            if (Context.MovementModule.Direction.x == 0)
                Context.MovementModule.ApplyFriction(Context.Data.groundDecay);
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            // set coyote time just when falling
            if (!Context.IsGrounded)
            {
                Context.JumpModule.IsActiveCoyoteTime = true;
                return PlayerStates.Falling;
            }
            
            if (Context.JumpModule.Request)
            {
                Context.JumpModule.IsActiveCoyoteTime = false;
                return PlayerStates.Jumping;
            }

            if (Context.DashModule.Request && Context.DashModule.CanDash)
                return PlayerStates.Dashing;
            
            return StateKey;
        }
    }
}