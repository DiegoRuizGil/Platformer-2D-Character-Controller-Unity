using UnityEngine;

namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerWallJumpingState : PlayerBaseState
    {
        private float _initialXPosition;
        
        public PlayerWallJumpingState(PlayerStates key, PlayerController context)
            : base(key, context) { }

        public override void EnterState()
        {
            Context.Animator.Play("Jumping");
            
            _initialXPosition = Context.transform.position.x;
            
            int jumpDirection = Context.LeftWallHit ? 1 : -1;
            Context.JumpModule.WallJump(Context.Data.wallJumpForce, jumpDirection);
        }

        public override void UpdateState()
        {
            SetGravityScale();
        }

        public override void FixedUpdateState()
        {
            if (HasPassedMinDistance())
            {
                float acceleration = Context.Data.acceleration * Context.Data.wallJumpAccelerationMult;
                Context.MovementModule.Move(Context.Direction, Context.Data.runMaxSpeed, acceleration);
            }
        }

        public override PlayerStates GetNextState()
        {
            if (Context.Velocity.y < 0)
                return PlayerStates.Falling;
            
            if (Context.DashModule.Request && Context.DashModule.CanDash)
                return PlayerStates.Dashing;
            
            return PlayerStates.WallJumping;
        }
        
        private void SetGravityScale()
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
        
        private bool HasPassedMinDistance()
        {
            float distance = Mathf.Abs(Context.transform.position.x - _initialXPosition);
            return distance >= Context.Data.wallJumpMinDistance;
        }
    }
}