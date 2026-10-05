using System;
using Zenject;

namespace Game.Core.Gameplay.TBS
{
    public sealed class AIBehaviourFactory
    {
        private readonly DiContainer _diContainer;

        public AIBehaviourFactory( DiContainer diContainer )
        {
            _diContainer = diContainer ?? throw new ArgumentNullException( nameof(diContainer) );
        }

        public IAIBehaviour Create( AIBehaviourConfig config ) => (IAIBehaviour)_diContainer.Instantiate( config.BehaviourType );

        public T Create< T >() where T : IAIBehaviour => _diContainer.Instantiate< T >();

        public AIContext CreateContext( UnitController unit, Battle battle ) => _diContainer.Instantiate< AIContext >( new object[] { unit, battle } );
    }
}
