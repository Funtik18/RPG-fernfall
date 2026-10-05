using Zenject;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleActionFactory
    {
        private readonly DiContainer _diContainer;
        
        public BattleActionFactory( DiContainer diContainer )
        {
            _diContainer = diContainer ?? throw new System.ArgumentNullException( nameof(diContainer) );
        }

        public T Create< T >( UnitController unit )
            where T : BattleAction
        {
            return _diContainer.Instantiate< T >( new object[]{ unit } );
        }
    }
}