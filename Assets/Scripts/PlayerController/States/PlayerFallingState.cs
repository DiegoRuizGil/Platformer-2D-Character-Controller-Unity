using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController.States
{
    public class PlayerFallingState : PlayerBaseState
    {
        private readonly float _lerpAmount;
        private float _timeInState;

        public PlayerFallingState(PlayerStates key, PlayerController context)
            : base(key, context)
        {
            _lerpAmount = 1f;
        }

        public override void EnterState()
        {
            float gravityScale = Context.Data.gravityScale * Context.Data.fallGravityMult;
            Context.SetGravityScale(gravityScale);

            _timeInState = 0f;
        }

        public override void UpdateState()
        {
            // coyote time
            if (_timeInState <= Context.Data.coyoteTime)
            {
                _timeInState += Time.deltaTime;
            }
            else
            {
                Context.IsActiveCoyoteTime = false;
            }
            
            // probar a implementar el fastFallGravityMult cuando se pulsa hacia abajo
            if (Context.updateInPlayMode)
            {
                float gravityScale = Context.Data.gravityScale;
                if (Context.MovementDirection.y < 0) // higher gravity if holding down
                    gravityScale *= Context.Data.fastFallGravityMult;
                else
                    gravityScale *= Context.Data.fallGravityMult;
                
                Context.SetGravityScale(gravityScale);
            }
        }

        public override void FixedUpdateState()
        {
            // limit vertical velocity
            float terminalVelocity = -Context.Data.maxFallSpeed;
            // higher fall velocity if holding down
            if (Context.MovementDirection.y < 0)
                terminalVelocity = -Context.Data.maxFastFallSpeed;
            
            Context.Velocity = new Vector2(
                Context.Velocity.x,
                Mathf.Max(Context.Velocity.y, terminalVelocity));
            
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
            if (Context.IsGrounded)
                return PlayerStates.Grounded;
            
            // implementar coyote time => jumping
            if (Context.JumpRequest && Context.IsActiveCoyoteTime)
            {
                Context.IsActiveCoyoteTime = false;
                return PlayerStates.Jumping;
            }
            
            return StateKey;
        }
    }
}