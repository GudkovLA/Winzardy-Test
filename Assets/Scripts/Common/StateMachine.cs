#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Common
{
    public class StateMachine<T> : IDisposable
    {
        private readonly Dictionary<T, State> _states = new();
        private State? _currentState;

        public void Dispose()
        {
            _states.Clear();
        }
        
        public void AddState(T state, Action? onEnter, Action? onUpdate, Action? onLeave)
        {
            if (_states.ContainsKey(state))
            {
                Debug.LogError($"State already defined (StateId={state})");
                return;
            }
            
            _states.Add(state, new State(onEnter, onUpdate, onLeave));
        }

        public void SetState(T state)
        {
            if (!_states.TryGetValue(state, out var newState))
            {
                Debug.LogError($"State is not defined (StateId={state})");
                return;
            }

            if (newState == _currentState)
            {
                Debug.LogError($"State already active (StateId={state})");
                return;
            }

            _currentState?.OnLeave?.Invoke();
            _currentState = newState;
            _currentState?.OnEnter?.Invoke();
        }

        public void Update()
        {
            _currentState?.OnUpdate?.Invoke();
        }

        private class State
        {
            public readonly Action? OnEnter;
            public readonly Action? OnUpdate;
            public readonly Action? OnLeave;

            public State(Action? onEnter, Action? onUpdate, Action? onLeave)
            {
                OnEnter = onEnter;
                OnUpdate = onUpdate;
                OnLeave = onLeave;
            }
        }
    }
}