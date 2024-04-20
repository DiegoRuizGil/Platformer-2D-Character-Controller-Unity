using System;

namespace StateMachine
{
    public abstract class BaseState<EState> where EState : Enum
    { 
        public EState StateKey { get; private set; }
        public String Name => StateKey.ToString();
        
        public BaseState(EState key)
        {
            StateKey = key;
        }
        
        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void FixedUpdateState();
        public abstract void ExitState();
        public abstract EState GetNextState();
    }
}
