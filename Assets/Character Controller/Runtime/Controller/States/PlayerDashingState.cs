using UnityEngine;

namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerDashingState : PlayerBaseState
    {
        private float _timeInState;

        public PlayerDashingState(PlayerStates key, PlayerController context)
            : base(key, context) { }

        public override void EnterState()
        {
            Context.Animator.Play("Dash");
            
            Context.MovementModule.SetGravityScale(0);
            Context.Sleep(Context.Data.dashSleepTime); // add small reaction time to the player

            Vector2 direction = GetDirection();
            Context.DashModule.Dash(direction, Context.Data.dashSpeed);
            
            Context.MovementModule.SetDirectionToFace(direction.x > 0, false);
        }

        public override void UpdateState()
        {
            Context.DashModule.UpdateTimer(Time.deltaTime);
        }

        public override void ExitState()
        {
            Context.MovementModule.SetGravityScale(Context.Data.gravityScale);
            
            _ = Context.DashModule.Refill();
        }

        public override PlayerStates GetNextState()
        {
            if (Context.DashModule.Completed)
            {
                if (Context.IsGrounded)
                    return PlayerStates.Grounded;
                
                return PlayerStates.Falling;
            }
            
            return StateKey;
        }
        
        private Vector2 GetDirection()
        {
            if (Context.Direction.x != 0f)
                return Context.Direction.x < 0 ? Vector2.left : Vector2.right;
            
            return Context.MovementModule.IsFacingRight ? Vector2.right : Vector2.left;
        }
    }
}