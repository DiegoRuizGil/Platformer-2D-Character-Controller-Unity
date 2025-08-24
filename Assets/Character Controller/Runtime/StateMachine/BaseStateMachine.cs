using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character_Controller.Runtime.StateMachine
{
    public abstract class BaseStateMachine<EState> : MonoBehaviour where EState : Enum
    {
        private Dictionary<EState, BaseState<EState>> _states = new Dictionary<EState, BaseState<EState>>();
        public Dictionary<EState, BaseState<EState>> States => _states;

        public BaseState<EState> CurrentState { get; protected set; }
        public BaseState<EState> PreviousState { get; protected set; }
        private bool _isTransitioningState;

        /// <summary>
        /// Make sure to call base.Start() in override if you need Start.
        /// </summary>
        protected virtual void Start()
        {
            SetStates();
            CurrentState?.EnterState();
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
            CurrentState.FixedUpdateState();
        }

        protected abstract void SetStates();
        
        private void UpdateState()
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
            PreviousState = CurrentState;
            CurrentState = States[stateKey];
            CurrentState.EnterState();

            _isTransitioningState = false;
        }
    }
}
