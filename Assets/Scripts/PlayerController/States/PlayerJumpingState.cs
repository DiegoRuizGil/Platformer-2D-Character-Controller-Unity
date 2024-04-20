using UnityEditor.Timeline;
using UnityEngine;

namespace PlayerController.States
{
    public class PlayerJumpingState : PlayerBaseState
    {
        private readonly float _lerpAmount;

        public PlayerJumpingState(PlayerStates key, PlayerController context)
            : base(key, context)
        {
            _lerpAmount = 1f;
        }

        public override void EnterState()
        {
            Context.SetGravityScale(Context.Data.gravityScale);
            Context.Jump();
        }

        public override void UpdateState()
        {
            float gravityScale = Context.Data.gravityScale;
            if (!Context.HandleLongJumps)
            {
                // set higher gravity when releasing the jump button
                gravityScale *= Context.Data.jumpCutGravity;
            }
            else if (Mathf.Abs(Context.Velocity.y) < Context.Data.jumpHangTimeThreshold)
            {
                gravityScale *= Context.Data.jumpHangGravityMult;
            }
            
            Context.SetGravityScale(gravityScale);
        }

        public override void FixedUpdateState()
        {
            float accelRate = Mathf.Abs(Context.MovementDirection.x) > 0.01f
                ? Context.Data.runAccelAmount * Context.Data.accelInAirMult
                : Context.Data.runDecelAmount * Context.Data.decelInAirMult;

            bool addBonusJumpApex =
                Mathf.Abs(Context.Velocity.y) < Context.Data.jumpHangTimeThreshold; 
            
            Context.Run(_lerpAmount, accelRate, addBonusJumpApex);
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (Context.Velocity.y < 0)
                return PlayerStates.Falling;
            
            return StateKey;
        }
    }
}