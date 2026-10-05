using System;

namespace Game.Core.Systems.StateSystem
{
    public sealed class StateTransition : IStateTransition
    {
        public IState From { get; }
        public IState To { get; }

        private readonly Func<bool> _condition;

        public StateTransition( IState from, IState to, Func< bool > condition )
        {
            From = from;
            To = to;
            _condition = condition;
        }

        public bool CanTransition()
        {
            return _condition.Invoke();
        }
    }
}