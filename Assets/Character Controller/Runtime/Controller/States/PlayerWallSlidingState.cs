using UnityEngine;

namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerWallSlidingState : PlayerBaseState
    {
        private bool _leftSide;
        private float _movingTimer;
        
        public PlayerWallSlidingState(PlayerStates key, PlayerController context)
            : base(key, context)
        {
            _lerpAmount = 1f;
            _canAddBonusJumpApex = false;
        }

        public override void EnterState()
        {
            _movingTimer = Context.Data.wallSlideReleaseTime;
            _leftSide = Context.LeftWallHit;
            
            Context.JumpModule.ResetAdditionalJumps();
            Context.MovementModule.SetGravityScale(0);
        }

        public override void UpdateState()
        {
            // check time pressing movement input
            if (Context.MovementModule.Direction.x > 0 && _leftSide
                || Context.MovementModule.Direction.x < 0 && !_leftSide)
            {
                _movingTimer -= Time.deltaTime;
            }
            else
            {
                _movingTimer = Context.Data.wallSlideReleaseTime;
            }
        }

        public override void FixedUpdateState()
        {
            Context.MovementModule.Slide(Context.Data.slideSpeed, Context.Data.slideAccel);
            
            // if input has been pressed for long enough,
            // allow the player to move horizontally
            if (_movingTimer <= 0)
                Context.MovementModule.Move(Context.Data.runMaxSpeed, Context.Data.acceleration);
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (Context.IsGrounded)
                return PlayerStates.Grounded;

            if (Context.JumpModule.Request)
                return PlayerStates.WallJumping;

            if (!Context.LeftWallHit && !Context.RightWallHit)
                return PlayerStates.Falling;
            
            return PlayerStates.WallSliding;
        }
    }
}