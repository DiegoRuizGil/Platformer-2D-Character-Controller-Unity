using System;

namespace StateMachine
{
    public abstract class BaseState<EState> where EState : Enum
    { 
        public EState StateKey { get; private set; }
        public String Name => StateKey.ToString();

        protected BaseStateMachine<EState> _context;
        
        public BaseState(EState key, BaseStateMachine<EState> context)
        {
            StateKey = key;
            _context = context;
        }
        
        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract EState GetNextState();
    }
}
