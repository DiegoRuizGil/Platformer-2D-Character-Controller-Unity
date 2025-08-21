using System;

namespace Character_Controller.Runtime.StateMachine
{
    public abstract class BaseState<EState> where EState : Enum
    { 
        public EState StateKey { get; private set; }
        public string Name => StateKey.ToString();
        
        public BaseState(EState key)
        {
            StateKey = key;
        }
        
        public virtual void EnterState() { }
        public virtual void UpdateState() { }
        public virtual void FixedUpdateState() { }
        public virtual void ExitState() { }
        public abstract EState GetNextState();
    }
}
