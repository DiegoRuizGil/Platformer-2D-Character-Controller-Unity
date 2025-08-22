namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerCrouchState : PlayerBaseState
    {
        public PlayerCrouchState(PlayerStates key, PlayerController context)
            : base(key, context) { }

        public override void EnterState()
        {
            Context.Animator.Play("Crouch");
            Context.CrouchModule.SetCrouchCollider();
        }

        public override void FixedUpdateState()
        {
            Context.MovementModule.Move(Context.Direction, Context.Data.crouchSpeed, Context.Data.acceleration);
            if (Context.Direction.x == 0)
                Context.MovementModule.ApplyFriction(Context.Data.groundDecay);
        }

        public override void ExitState()
        {
            Context.CrouchModule.SetDefaultCollider();
        }

        public override PlayerStates GetNextState()
        {
            if (Context.CrouchModule.CeilingAbove)
                return PlayerStates.Crouching;
            
            if (!Context.CrouchModule.InputRequest)
                return PlayerStates.Grounded;
            
            // set coyote time just when falling
            if (!Context.IsGrounded)
            {
                Context.JumpModule.IsActiveCoyoteTime = true;
                return PlayerStates.Falling;
            }
            
            if (Context.JumpModule.InputRequest)
            {
                Context.JumpModule.IsActiveCoyoteTime = false;
                return PlayerStates.Jumping;
            }

            if (Context.DashModule.InputRequest && Context.DashModule.CanDash)
                return PlayerStates.Dashing;
            
            return StateKey;
        }
    }
}