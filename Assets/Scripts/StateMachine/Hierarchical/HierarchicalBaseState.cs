using System;
using UnityEngine;

namespace StateMachine.Hierarchical
{
    public abstract class HierarchicalBaseState<EState> : BaseState<EState> where EState : Enum
    {
        protected bool _isRootState;
        protected HierarchicalBaseState<EState> _currentSubState;
        protected HierarchicalBaseState<EState> _currentSuperState;

        private bool _isTransitioningSubState;
        
        protected HierarchicalBaseState(EState key, BaseStateMachine<EState> context)
            : base(key, context) { }

        protected abstract void InitializeSubState();

        protected void UpdateSubState()
        {
            if (_currentSubState == null) return;
            
            EState nextStateKey = _currentSubState.GetNextState();
            if (!_isTransitioningSubState && nextStateKey.Equals(_currentSubState.StateKey))
            {
                _currentSubState.UpdateState();
            }
            else if (!_isTransitioningSubState)
            {
                TransitionSubState(nextStateKey);
            } 
        }

        private void TransitionSubState(EState stateKey)
        {
            _isTransitioningSubState = true;

            _currentSubState.ExitState();
            BaseState<EState> nextState = _context.States[stateKey];
            if (nextState is HierarchicalBaseState<EState>)
            {
                _currentSubState = nextState as HierarchicalBaseState<EState>;
                _currentSubState.EnterState();
            }
            else
            {
                Debug.LogWarning($"State {nextState.Name} is not a hierarchical state of type {nameof(EState)}");
            }

            _isTransitioningSubState = false;
        }
    }
}