using System;
using System.Collections.Generic;

namespace Game.Core.Systems.StateSystem
{
    public sealed class StateMachine
    {
        private readonly Dictionary< IState, List< IStateTransition > > _transitions = new();
        private readonly List< IStateTransition > _anyTransitions = new();

        public IState CurrentState => _currentState;
        
        private IState _currentState;
        private bool _isTransitioning;

        public void SetInitialState( IState state )
        {
            if ( _currentState != null )
                throw new InvalidOperationException( "[State] Initial state is already set." );

            _currentState = state;
            _currentState.Enter();
        }

        public void Start( IState state )
        {
            if ( _currentState != null )
                throw new InvalidOperationException( "[State] State machine is already started." );

            _currentState = state;
            _currentState.Enter();
        }

        public void Stop()
        {
            if ( _currentState == null )
                return;

            _currentState.Exit();
            _currentState = null;
        }

        public void AddTransition( IState from, IState to, Func< bool > condition )
        {
            if ( !_transitions.TryGetValue( from, out var transitions ) )
            {
                transitions = new List< IStateTransition >();
                _transitions[ from ] = transitions;
            }

            transitions.Add( new StateTransition( from, to, condition ) );
        }

        public void AddAnyTransition( IState to, Func< bool > condition )
        {
            _anyTransitions.Add( new StateTransition( null, to, condition ) );
        }

        public void Tick( float deltaTime )
        {
            if ( _currentState == null )
                return;

            TryTransition();

            _currentState.Tick( deltaTime );
        }

        public void FixedTick( float deltaTime )
        {
            _currentState?.FixedTick( deltaTime );
        }

        public void ChangeState( IState state )
        {
            if ( state == null )
                throw new ArgumentNullException( "[State] " + nameof(state) );

            if ( ReferenceEquals( _currentState, state ) || _isTransitioning)
                return;

            _isTransitioning = true;

            try
            {
                _currentState?.Exit();
                _currentState = state;
                _currentState.Enter();
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        private void TryTransition()
        {
            var transition = GetTransition();

            if ( transition == null )
                return;

            ChangeState( transition.To );
        }

        private IStateTransition GetTransition()
        {
            for ( int i = 0; i < _anyTransitions.Count; i++ )
            {
                var transition = _anyTransitions[ i ];

                if ( transition.CanTransition() )
                {
                    return transition;
                }
            }

            if ( !_transitions.TryGetValue( _currentState, out var transitions ) )
            {
                return null;
            }

            for ( int i = 0; i < transitions.Count; i++ )
            {
                var transition = transitions[ i ];
                if ( transition.CanTransition() )
                {
                    return transition;
                }
            }

            return null;
        }
    }
}