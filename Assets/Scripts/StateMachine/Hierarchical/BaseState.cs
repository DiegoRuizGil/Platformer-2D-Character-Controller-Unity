using System;
using UnityEngine.TestTools;

namespace StateMachine.Hierarchical
{
    public abstract class BaseState<EState> where EState : Enum
    { 
        public EState StateKey { get; private set; }
        public String Name => StateKey.ToString();
        
        public bool IsRootState { get; protected set; }
        private BaseState<EState> _currentSuperState;
        
        public BaseState<EState> CurrentSubState { get; protected set; }
        public BaseState<EState> CurrentSuperState { get; protected set; }
        public bool IsTransitioningSubState;
        
        public BaseState(EState key)
        {
            StateKey = key;
        }
        
        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract EState GetNextState();
        protected abstract void InitializeSubState();

        private void SetSuperState(BaseState<EState> newSuperState)
        {
            CurrentSuperState = newSuperState;
        }
        
        public void SetSubState(BaseState<EState> newSubState)
        {
            CurrentSubState?.ExitState();
            if (newSubState == null) return;
            
            CurrentSubState = newSubState;
            CurrentSubState.EnterState();
            CurrentSubState.SetSuperState(this);
        }
    }
}
