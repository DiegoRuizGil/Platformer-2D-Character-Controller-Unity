using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public abstract class BaseStateMachine<EState> : MonoBehaviour where EState : Enum
    {
        protected Dictionary<EState, BaseState<EState>> states = new Dictionary<EState, BaseState<EState>>();
        public Dictionary<EState, BaseState<EState>> States => states;

        protected BaseState<EState> CurrentState;
        private bool _isTransitioningState;

        void Start()
        {
            CurrentState.EnterState();
        }
        
        protected void UpdateState()
        {
            EState nextStateKey = CurrentState.GetNextState();

            if (!_isTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
            {
                CurrentState.UpdateState();
            }
            else if (!_isTransitioningState)
            {
                TransitionState(nextStateKey);
            } 
        }

        private void TransitionState(EState stateKey)
        {
            _isTransitioningState = true;

            CurrentState.ExitState();
            CurrentState = states[stateKey];
            CurrentState.EnterState();

            _isTransitioningState = false;
        }
    }
}
