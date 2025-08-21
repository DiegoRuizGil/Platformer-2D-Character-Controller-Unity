namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerCrouchState : PlayerBaseState
    {
        public PlayerCrouchState(PlayerStates key, PlayerController context)
            : base(key, context) { }

        public override void EnterState()
        {
            Context.Animator.Play("Crouch");
        }

        public override void FixedUpdateState()
        {
            
        }

        public override PlayerStates GetNextState()
        {
            if (!Context.CrouchModule.Request)
                return PlayerStates.Grounded;
            
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