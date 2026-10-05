using System;

namespace Game.Core.Systems.CommandSystem
{
    public abstract class CommandPresenter< T > : ICommand
        where T : CommandComponent
    {
        public event Action OnDisposed;

        public T Component { get; }

        public CommandPresenter( T component )
        {
            Component = component;
        }

        public virtual void Dispose()
        {
            OnDisposed?.Invoke();
        }

        public abstract void Execute();
    }
}