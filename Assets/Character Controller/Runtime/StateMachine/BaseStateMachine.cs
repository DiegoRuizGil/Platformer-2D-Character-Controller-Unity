using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character_Controller.Runtime.StateMachine
{
    public abstract class BaseStateMachine<EState> : MonoBehaviour where EState : Enum
    {
        private Dictionary<EState, BaseState<EState>> _states = new Dictionary<EState, BaseState<EState>>();
        public Dictionary<EState, BaseState<EState>> States => _states;

        protected BaseState<EState> _currentState;
        private bool _isTransitioningState;

        /// <summary>
        /// Make sure to call base.Start() in override if you need Start.
        /// </summary>
        protected virtual void Start()
        {
            SetStates();
            _currentState?.EnterState();
        }
        
        /// <summary>
        /// Make sure to call base.Update() in override if you need Update.
        /// </summary>
        protected virtual void Update()
        {
            UpdateState();
        }

        /// <summary>
        /// Make sure to call base.FixedUpdate() in override if you need Update.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            _currentState.FixedUpdateState();
        }

        protected abstract void SetStates();
        
        private void UpdateState()
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
