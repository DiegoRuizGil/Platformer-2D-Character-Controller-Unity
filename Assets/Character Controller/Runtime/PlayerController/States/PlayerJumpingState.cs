using UnityEngine;

namespace Character_Controller.Runtime.PlayerController.States
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
            Context.SetGravityScale(Context.Data.gravityScale);
            Context.Jump();
        }

        public override void UpdateState()
        {
            float gravityScale = Context.Data.gravityScale;
            if (Mathf.Abs(Context.Velocity.y) < Context.Data.jumpHangTimeThreshold)
            {
                gravityScale *= Context.Data.jumpHangGravityMult;
            }
            else if (!Context.HandleLongJumps)
            {
                // set higher gravity when releasing the jump button
                gravityScale *= Context.Data.jumpCutGravity;
            }
            
            Context.SetGravityScale(gravityScale);
        }

        public override void FixedUpdateState()
        {
            Context.Run(_lerpAmount, _canAddBonusJumpApex);
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (Context.Velocity.y < 0)
                return PlayerStates.Falling;
            
            if (Context.DashRequest && Context.CanDash)
                return PlayerStates.Dashing;
            
            return StateKey;
        }
    }
}