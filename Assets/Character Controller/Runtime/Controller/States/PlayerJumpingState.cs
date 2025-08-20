using UnityEngine;

namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerJumpingState : PlayerBaseState
    {
        public PlayerJumpingState(PlayerStates key, PlayerController context)
            : base(key, context)
        {
            _lerpAmount = 1f;
            _canAddBonusJumpApex = true;
        }

        public override void EnterState()
        {
            Context.MovementModule.SetGravityScale(Context.Data.gravityScale);
            Context.Jump();
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
            if (Context.MovementModule.Direction.x == 0)
                Context.MovementModule.ApplyHorizontalFriction(Context.Data.airDecay);
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (Context.Velocity.y < 0)
                return PlayerStates.Falling;
            
            if (Context.DashModule.Request && Context.DashModule.CanDash)
                return PlayerStates.Dashing;
            
            return StateKey;
        }
    }
}