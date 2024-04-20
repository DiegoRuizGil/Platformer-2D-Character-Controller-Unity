using UnityEngine;

namespace PlayerController.States
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
            Context.ResetAdditionalJumps();
            Context.SetGravityScale(Context.Data.gravityScale);
        }

        public override void UpdateState()
        {
            if (Context.updateInPlayMode)
            {
                Context.SetGravityScale(Context.Data.gravityScale);
            }
        }

        public override void FixedUpdateState()
        {
            Context.Run(_lerpAmount, _canAddBonusJumpApex);
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
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
            
            return StateKey;
        }
    }
}