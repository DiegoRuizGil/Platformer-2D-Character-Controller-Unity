using UnityEngine;

namespace Character_Controller.Runtime.Controller.States
{
    public class PlayerDashingState : PlayerBaseState
    {
        private float _timeInState;
        private Vector2 _direction;

        public PlayerDashingState(PlayerStates key, PlayerController context)
            : base(key, context) { }

        public override void EnterState()
        {
            _timeInState = 0f;
            
            Context.DashModule.IsActive = false;
            Context.Sleep(Context.Data.dashSleepTime); // add small reaction time to the player
            
            // set dash direction
            if (Context.MovementModule.Direction.x != 0f)
                _direction = Context.MovementModule.Direction.x < 0 ? Vector2.left : Vector2.right;
            else
                _direction = Context.MovementModule.IsFacingRight ? Vector2.right : Vector2.left;
            
            Context.SetDirectionToFace(_direction.x > 0);
            Context.VFX.InstantiateDashVFX(Context.MovementModule.IsFacingRight);
        }

        public override void UpdateState()
        {
            _timeInState += Time.deltaTime;
            Context.Velocity = _direction * Context.Data.dashSpeed;
        }

        public override void FixedUpdateState() { }

        public override void ExitState()
        {
            _ = Context.DashModule.Refill();
        }

        public override PlayerStates GetNextState()
        {
            if (_timeInState >= Context.Data.dashTime)
            {
                if (Context.IsGrounded)
                    return PlayerStates.Grounded;
                
                return PlayerStates.Falling;
            }
            
            return StateKey;
        }
    }
}