using UnityEngine;

namespace PlayerController.States
{
    public class PlayerGroundedState : PlayerBaseState
    {
        private readonly float _lerpAmount;
        private readonly bool _addBonusJumpApex;

        public PlayerGroundedState(PlayerStates key, PlayerController context)
            : base(key, context)
        {
            _lerpAmount = 1f;
            _addBonusJumpApex = false;
        }

        public override void EnterState()
        {
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
            float accelRate = Mathf.Abs(Context.MovementDirection.x) > 0.01f
                ? Context.Data.runAccelAmount
                : Context.Data.runDecelAmount;
            
            Context.Run(_lerpAmount, accelRate, _addBonusJumpApex);
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