using Zenject;

namespace Game.Core.Systems.StateSystem
{
    public sealed class StateFactory
    {
        private readonly DiContainer _diContainer;
        
        public StateFactory( DiContainer diContainer )
        {
            _diContainer = diContainer ?? throw new System.ArgumentNullException( nameof(diContainer) );
        }

        public T Create< T >()
            where T : IState
        {
            return _diContainer.Instantiate< T >();
        }
    }
}