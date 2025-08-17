using UnityEngine;

namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerFallingState : PlayerBaseState
    {
        private float _timeInState;

        public PlayerFallingState(PlayerStates key, PlayerController context)
            : base(key, context)
        {
            _lerpAmount = 1f;
            _canAddBonusJumpApex = true;
        }

        public override void EnterState()
        {
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
            
            float gravityScale = Context.Data.gravityScale;
            if (Context.MovementModule.Direction.y < 0) // higher gravity if holding down
                gravityScale *= Context.Data.fastFallGravityMult;
            else
                gravityScale *= Context.Data.fallGravityMult;
                
            Context.SetGravityScale(gravityScale);
        }

        public override void FixedUpdateState()
        {
            // limit vertical velocity
            float terminalVelocity = -Context.Data.maxFallSpeed;
            // higher fall velocity if holding down
            if (Context.MovementModule.Direction.y < 0)
                terminalVelocity = -Context.Data.maxFastFallSpeed;
            
            Context.Velocity = new Vector2(
                Context.Velocity.x,
                Mathf.Max(Context.Velocity.y, terminalVelocity));
            
            Context.Run(_lerpAmount, _canAddBonusJumpApex);
        }

        public override void ExitState()
        {
            Context.IsActiveCoyoteTime = false;
        }

        public override PlayerStates GetNextState()
        {
            if (Context.IsGrounded)
                return PlayerStates.Grounded;
            
            if (Context.JumpRequest)
            {
                if (Context.IsActiveCoyoteTime)
                    return PlayerStates.Jumping;

                if (Context.AdditionalJumpsAvailable > 0)
                {
                    Context.AdditionalJumpsAvailable--;
                    return PlayerStates.Jumping;
                }
            }

            if ((Context.LeftWallHit || Context.RightWallHit)
                && Context.MovementModule.Direction != Vector2.zero)
                return PlayerStates.WallSliding;
            
            if (Context.DashModule.Request && Context.DashModule.CanDash)
                return PlayerStates.Dashing;
            
            return StateKey;
        }
    }
}