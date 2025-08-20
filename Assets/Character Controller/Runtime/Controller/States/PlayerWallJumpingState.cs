using UnityEngine;

namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerWallJumpingState : PlayerBaseState
    {
        public PlayerWallJumpingState(PlayerStates key, PlayerController context)
            : base(key, context) { }

        public override void EnterState()
        {
            // set wall jump direction
            int dir = Context.LeftWallHit ? 1 : -1;
            Context.JumpModule.WallJump(Context.Data.wallJumpForce, dir);
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
            Context.MovementModule.Move(Context.Data.runMaxSpeed, Context.Data.acceleration);
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (Context.Velocity.y < 0)
                return PlayerStates.Falling;
            
            if (Context.DashModule.Request && Context.DashModule.CanDash)
                return PlayerStates.Dashing;
            
            return PlayerStates.WallJumping;
        }
    }
}