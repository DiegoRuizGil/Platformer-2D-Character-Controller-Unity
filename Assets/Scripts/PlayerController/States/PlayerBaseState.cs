using StateMachine.Hierarchical;

namespace PlayerController.States
{
    public abstract class PlayerBaseState : BaseState<PlayerStates>
    {
        protected PlayerStateMachine _context;
        
        protected PlayerBaseState(PlayerStates key, PlayerStateMachine context)
            : base(key)
        {
            _context = context;
        }
    }
}