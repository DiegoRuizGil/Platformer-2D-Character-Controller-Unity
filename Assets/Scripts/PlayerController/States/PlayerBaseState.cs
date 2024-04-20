using StateMachine;

namespace PlayerController.States
{
    public enum PlayerStates
    {
        Grounded, Jumping, Falling
    }
    
    public abstract class PlayerBaseState : BaseState<PlayerStates>
    {
        public PlayerController Context { get; private set; }
        
        protected PlayerBaseState(PlayerStates key, PlayerController context)
            : base(key)
        {
            Context = context;
        }
    }
}