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

            if (_context.IsTouchingLeftCorner)
                _context.transform.position -= new Vector3(_context.CornerDistanceCorrection, 0f, 0f);
            else if (_context.IsTouchingRightCorner)
                _context.transform.position += new Vector3(_context.CornerDistanceCorrection, 0f, 0f);
            else if (_context.IsTouchingCeiling)
                _context.SetVerticalVelocity(0f);
            
            HandleGravity();
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            // poder pasar al estado graunded desde jumping tiene sentido?
            // if (_context.IsGrounded && _timeInState >= _context.CheckGroundAfterJump)
            //     return PlayerStates.Grounded;

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