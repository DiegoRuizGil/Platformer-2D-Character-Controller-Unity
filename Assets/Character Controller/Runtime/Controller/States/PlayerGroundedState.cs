using UnityEngine;

namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerGroundedState : PlayerBaseState
    {
        enum Status { Idle, Moving }

        private Status _currentStatus = Status.Idle;
        private Status _previousStatus = Status.Idle;
        
        public PlayerGroundedState(PlayerStates key, PlayerController context)
            : base(key, context) { }

        public override void EnterState()
        {
            UpdateStatus();
            PlayAnimation();
            
            // reset additional jumps and dash
            Context.JumpModule.ResetAdditionalJumps();
            Context.DashModule.IsActive = true;
            
            Context.MovementModule.SetGravityScale(Context.Data.gravityScale);
            
            if (!Context.JumpModule.InputRequest)
                Context.VFX.InstantiateFallDustVFX();
        }

        public override void UpdateState()
        {
            UpdateStatus();
            HandleAnimation();
        }

        public override void FixedUpdateState()
        {
            Context.MovementModule.Move(Context.Direction, Context.Data.runMaxSpeed, Context.Data.runAcceleration);
            if (Context.Direction.x == 0)
                Context.MovementModule.ApplyFriction(Context.Data.groundDecay);
        }

        public override PlayerStates GetNextState()
        {
            // set coyote time just when falling
            if (!Context.IsGrounded)
            {
                Context.JumpModule.IsActiveCoyoteTime = true;
                return PlayerStates.Falling;
            }
            
            if (Context.JumpModule.InputRequest)
            {
                Context.JumpModule.IsActiveCoyoteTime = false;
                return PlayerStates.Jumping;
            }

            if (Context.DashModule.InputRequest && Context.DashModule.CanDash)
                return PlayerStates.Dashing;

            if (Context.CrouchModule.InputRequest)
                return PlayerStates.Crouching;

            if (Context.ClimbingModule.InputUpRequest && Context.ClimbingModule.CanClimb)
                return PlayerStates.Climbing;
            
            if (Context.ClimbingModule.InputDownRequest && Context.ClimbingModule.CanDescend)
                return PlayerStates.Climbing;
            
            return StateKey;
        }
        
        private void UpdateStatus()
        {
            _previousStatus = _currentStatus;
            _currentStatus = Mathf.Abs(Context.Velocity.x) < 0.1f ? Status.Idle : Status.Moving;
        }

        private void HandleAnimation()
        {
            if (_previousStatus != _currentStatus)
                PlayAnimation();
        }

        private void PlayAnimation()
        {
            switch (_currentStatus)
            {
                case Status.Idle:
                    Context.Animator.Play("Idle");
                    break;
                case Status.Moving:
                    Context.Animator.Play("Moving");
                    break;
            }
        }
    }
}