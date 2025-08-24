using UnityEngine;

namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerClimbingState : PlayerBaseState
    {
        public PlayerClimbingState(PlayerStates key, PlayerController context)
            : base(key, context) { }

        public override void EnterState()
        {
            Context.Animator.Play("Climbing");
            
            Context.MovementModule.SetGravityScale(0);
            Context.ClimbingModule.SetClimbingPosition();
        }

        public override void UpdateState()
        {
            Context.Animator.enabled = !Mathf.Approximately(Context.Direction.y, Mathf.Epsilon);
        }

        public override void FixedUpdateState()
        {
            Context.ClimbingModule.Climb(
                Context.Direction.y,
                Context.Data.climbSpeed,
                Context.Data.climbAcceleration);
            
            if (Context.Direction.y == 0)
                Context.MovementModule.ApplyFriction(Context.Data.climbDecay);
        }

        public override void ExitState()
        {
            Context.Animator.enabled = true;
            Context.MovementModule.SetGravityScale(Context.Data.gravityScale);
            
            if (Context.ClimbingModule.InputUpRequest && Context.ClimbingModule.OnTopLadder)
                Context.ClimbingModule.SetGroundedPosition();
        }

        public override PlayerStates GetNextState()
        {
            if (Context.Direction.y < 0 && Context.IsGrounded)
                return PlayerStates.Grounded;

            if (Context.ClimbingModule.InputUpRequest && Context.ClimbingModule.OnTopLadder)
                return PlayerStates.Grounded;
            
            if (Context.JumpModule.InputRequest)
                return PlayerStates.Jumping;

            if (Context.ClimbingModule.OnBottomLadder && Context.Direction.y < 0)
                return PlayerStates.Falling;
            
            return StateKey;
        }
    }
}