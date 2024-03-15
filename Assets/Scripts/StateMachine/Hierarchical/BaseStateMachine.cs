using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Hierarchical
{
    public abstract class BaseStateMachine<EState> : MonoBehaviour where EState : Enum
    {
        private Dictionary<EState, BaseState<EState>> _states = new Dictionary<EState, BaseState<EState>>();
        public Dictionary<EState, BaseState<EState>> States => _states;

        protected BaseState<EState> _currentRootState;
        private bool _isTransitioningState;

        protected virtual void Start()
        {
            SetStates();
            _currentRootState?.EnterState();
        }

        protected abstract void SetStates();
        
        protected void UpdateState()
        {
            // EState nextStateKey = _currentState.GetNextState();
            //
            // if (!_isTransitioningState && nextStateKey.Equals(_currentState.StateKey))
            // {
            //     _currentState.UpdateState();
            // }
            // else if (!_isTransitioningState)
            // {
            //     TransitionState(nextStateKey);
            // }
            
            // añadir mensaje de que no se ha seteado el current state, en un else
            if (_currentRootState != null) UpdateState(_currentRootState);
        }

        private void UpdateState(BaseState<EState> state)
        {
            if (state.CurrentSubState != null)
            {
                UpdateState(state.CurrentSubState);
            }
            
            EState nextStateKey = state.GetNextState();
            if (!_isTransitioningState && nextStateKey.Equals(state.StateKey))
            {
                state.UpdateState();
            }
            else if (!_isTransitioningState)
            {
                TransitionState(state, nextStateKey);
            }
        }

        private void TransitionState(BaseState<EState> state, EState stateKey)
        {
            _isTransitioningState = true;

            BaseState<EState> newSubState = _states[stateKey];
            state.ExitState();
            
            if (state.IsRootState)
                _currentRootState = newSubState;
            else
                state.CurrentSuperState.SetSubState(newSubState);
            
            newSubState.EnterState();

            _isTransitioningState = false;
        }
    }
}
