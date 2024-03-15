using UnityEngine;

namespace PlayerController.States
{
    public class PlayerJumpingState : PlayerBaseState, IHandleGravity
    {
        private float _timeInState;
        
        public PlayerJumpingState(PlayerStates key, PlayerStateMachine context)
            : base(key, context)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            _timeInState = 0f;
            
            InitializeSubState();
            PerformJump();
        }

        public override void UpdateState()
        {
            if (_timeInState <= _context.CheckGroundAfterJump)
                _timeInState += Time.deltaTime;
            
            // check if touching ceiling -> vertical velocity = 0
            if (_context.IsTouchingCeiling)
                _context.SetVerticalVelocity(0f);
            
            HandleGravity();
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (_context.IsGrounded && _timeInState >= _context.CheckGroundAfterJump)
                return PlayerStates.Grounded;

            if (_context.Velocity.y < 0)
                return PlayerStates.Falling;
            
            return StateKey;
        }

        protected override void InitializeSubState()
        {
            SetSubState(_context.MovementDirection == Vector2.zero
                ? _context.States[PlayerStates.Idle]
                : _context.States[PlayerStates.Moving]);
        }

        private void PerformJump()
        {
            _context.SetVerticalVelocity(_context.JumpVelocity);
        }

        public void HandleGravity()
        {
            float gravity = _context.Gravity;
            if (!_context.HandleLongJumps)
                gravity *= _context.LowJumpGravityMultiplier;

            float ySpeed = _context.Velocity.y + gravity * Time.deltaTime;
            _context.SetVerticalVelocity(ySpeed);
        }
    }
}