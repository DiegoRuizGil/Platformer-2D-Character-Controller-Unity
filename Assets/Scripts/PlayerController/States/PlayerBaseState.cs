using StateMachine;

namespace PlayerController.States
{
    public enum PlayerStates
    {
        Grounded, Jumping, Falling, WallSliding, WallJumping, Dashing
    }
    
    public abstract class PlayerBaseState : BaseState<PlayerStates>
    {
        protected float _lerpAmount;
        protected bool _canAddBonusJumpApex;
        
        public PlayerController Context { get; private set; }
        
        protected PlayerBaseState(PlayerStates key, PlayerController context)
            : base(key)
        {
            Context = context;
        }
    }
}