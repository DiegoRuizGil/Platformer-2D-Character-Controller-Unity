using UnityEngine;

namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerJumpingState : PlayerBaseState
    {
        private bool _fromClimbing;
        
        public PlayerJumpingState(PlayerStates key, PlayerController context)
            : base(key, context) { }

        public override void EnterState()
        {
            Context.Animator.Play("Jumping");
            
            Context.MovementModule.SetGravityScale(Context.Data.gravityScale);
            Context.JumpModule.Jump(Context.Data.jumpForce);
        }

        public override void UpdateState()
        {
            float gravityScale = Context.Data.gravityScale;
            if (Mathf.Abs(Context.Velocity.y) < Context.Data.jumpHangTimeThreshold)
            {
                gravityScale *= Context.Data.jumpHangGravityMult;
            }
            else if (!Context.JumpModule.HandleLongJumps)
            {
                // set higher gravity when releasing the jump button
                gravityScale *= Context.Data.jumpCutGravity;
            }
            
            Context.MovementModule.SetGravityScale(gravityScale);
        }

        public override void FixedUpdateState()
        {
            Context.MovementModule.Move(Context.Direction, Context.Data.runMaxSpeed, Context.Data.runAcceleration);
            if (Context.Direction.x == 0)
                Context.MovementModule.ApplyHorizontalFriction(Context.Data.airDecay);
        }

        public override PlayerStates GetNextState()
        {
            if (Context.Velocity.y < 0)
                return PlayerStates.Falling;
            
            if (Context.DashModule.InputRequest && Context.DashModule.CanDash)
                return PlayerStates.Dashing;
            
            if (Context.ClimbingModule.InputUpRequest && Context.ClimbingModule.CanClimb && Context.PreviousState.StateKey != PlayerStates.Climbing)
                return PlayerStates.Climbing;
            
            return StateKey;
        }
    }
}