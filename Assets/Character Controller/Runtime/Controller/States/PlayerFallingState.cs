using UnityEngine;

namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerFallingState : PlayerBaseState
    {
        private float _timeInState;

        public PlayerFallingState(PlayerStates key, PlayerController context)
            : base(key, context)
        {
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
                Context.JumpModule.IsActiveCoyoteTime = false;
            }
            
            float gravityScale = Context.Data.gravityScale;
            if (Context.Direction.y < 0) // higher gravity if holding down
                gravityScale *= Context.Data.fastFallGravityMult;
            else
                gravityScale *= Context.Data.fallGravityMult;
                
            Context.MovementModule.SetGravityScale(gravityScale);
        }

        public override void FixedUpdateState()
        {
            LimitVerticalVelocity();

            Context.MovementModule.Move(Context.Direction, Context.Data.runMaxSpeed, Context.Data.acceleration);
            if (Context.Direction.x == 0)
                Context.MovementModule.ApplyHorizontalFriction(Context.Data.airDecay);
        }

        public override void ExitState()
        {
            Context.JumpModule.IsActiveCoyoteTime = false;
        }

        public override PlayerStates GetNextState()
        {
            if (Context.IsGrounded)
                return PlayerStates.Grounded;
            
            if (Context.JumpModule.Request)
            {
                if (Context.JumpModule.IsActiveCoyoteTime)
                    return PlayerStates.Jumping;

                if (Context.JumpModule.AdditionalJumpsAvailable > 0)
                {
                    Context.JumpModule.AdditionalJumpsAvailable--;
                    return PlayerStates.Jumping;
                }
            }

            if ((Context.LeftWallHit || Context.RightWallHit)
                && Context.Direction != Vector2.zero)
                return PlayerStates.WallSliding;
            
            if (Context.DashModule.Request && Context.DashModule.CanDash)
                return PlayerStates.Dashing;
            
            return StateKey;
        }
        
        private void LimitVerticalVelocity()
        {
            float terminalVelocity = -Context.Data.maxFallSpeed;
            // higher fall velocity if holding down
            if (Context.Direction.y < 0)
                terminalVelocity = -Context.Data.maxFastFallSpeed;
            
            Context.Velocity = new Vector2(
                Context.Velocity.x,
                Mathf.Max(Context.Velocity.y, terminalVelocity));
        }
    }
}