using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace PlayerController.States
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

            Context.IsDashActive = false;
            Context.Sleep(Context.Data.dashSleepTime); // add small reaction time to the player

            if (Context.MovementDirection.x != 0f)
                _direction = Context.MovementDirection.x < 0 ? Vector2.left : Vector2.right;
            else
                _direction = Context.IsFacingLeft ? Vector2.left : Vector2.right;
        }

        public override void UpdateState()
        {
            _timeInState += Time.deltaTime;
            Context.Velocity = _direction * Context.Data.dashSpeed;
        }

        public override void FixedUpdateState() { }

        public override void ExitState()
        {
            Context.RefillDash();
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