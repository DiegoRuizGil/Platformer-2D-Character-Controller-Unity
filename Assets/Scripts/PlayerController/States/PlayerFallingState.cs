using UnityEngine;

namespace PlayerController.States
{
    public class PlayerFallingState : PlayerBaseState, IHandleGravity
    {
        private readonly float _fallGravity;
        private float _fallingTimer;
        
        public PlayerFallingState(PlayerStates key, PlayerStateMachine context)
            : base(key, context)
        {
            IsRootState = true;
            _fallGravity = context.Gravity * context.FallGravityMultiplier;
        }

        public override void EnterState()
        {
            InitializeSubState();

            _fallingTimer = 0f;
        }

        public override void UpdateState()
        {
            if (_fallingTimer <= _context.CoyoteTime)
                _fallingTimer += Time.deltaTime;
            else
                _context.HasCoyoteTime = false;
            
            HandleGravity();
        }

        public override void ExitState() { }

        public override PlayerStates GetNextState()
        {
            if (_context.IsGrounded)
                return PlayerStates.Grounded;
            
            if (_context.JumpRequests > 0 && _context.HasCoyoteTime)
            {
                _context.ManageJumpRequest();
                _context.HasCoyoteTime = false;
                return PlayerStates.Jumping;
            }
            
            return StateKey;
        }

        protected override void InitializeSubState()
        {
            SetSubState(_context.MovementDirection == Vector2.zero
                ? _context.States[PlayerStates.Idle]
                : _context.States[PlayerStates.Moving]);
        }

        public void HandleGravity()
        {
            float ySpeed = _context.Velocity.y + _fallGravity * Time.deltaTime;
            _context.SetVerticalVelocity(ySpeed);
        }
    }
}