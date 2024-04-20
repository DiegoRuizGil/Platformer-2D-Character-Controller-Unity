using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public abstract class BaseStateMachine<EState> : MonoBehaviour where EState : Enum
    {
        private Dictionary<EState, BaseState<EState>> _states = new Dictionary<EState, BaseState<EState>>();
        public Dictionary<EState, BaseState<EState>> States => _states;

        protected BaseState<EState> _currentState;
        private bool _isTransitioningState;

        protected virtual void Start()
        {
            SetStates();
            _currentState?.EnterState();
        }
        
        protected virtual void Update()
        {
            UpdateState();
        }

        protected virtual void FixedUpdate()
        {
            _currentState.FixedUpdateState();
        }

        protected abstract void SetStates();
        
        protected void UpdateState()
        {
            EState nextStateKey = _currentState.GetNextState();
            
            if (!_isTransitioningState && nextStateKey.Equals(_currentState.StateKey))
            {
                _currentState.UpdateState();
            }
            else if (!_isTransitioningState)
            {
                TransitionState(nextStateKey);
            }
        }

        private void TransitionState(EState stateKey)
        {
            _isTransitioningState = true;

            _currentState.ExitState();
            _currentState = States[stateKey];
            _currentState.EnterState();

            _isTransitioningState = false;
        }
    }
}
