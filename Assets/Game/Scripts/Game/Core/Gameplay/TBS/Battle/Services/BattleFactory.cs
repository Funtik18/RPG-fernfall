using System.Collections.Generic;
using Zenject;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleFactory
    {
        private readonly DiContainer _diContainer;
        
        public BattleFactory( DiContainer diContainer )
        {
            _diContainer = diContainer ?? throw new System.ArgumentNullException( nameof(diContainer) );
        }
        
        public Battle Create( IReadOnlyList< UnitController > playerUnits, IReadOnlyList< UnitController > enemyUnits )
        {
            ResetSpecialUses( playerUnits );
            ResetSpecialUses( enemyUnits );

            var playerTeam = new BattleTeam( BattleParams.PLAYER_TEAM, playerUnits );
            var enemyTeam = new BattleTeam( BattleParams.ENEMY_TEAM, enemyUnits );
            var teams = new List< BattleTeam > { playerTeam, enemyTeam };
            
            return _diContainer.Instantiate< Battle >( new object[]{ teams } );
        }

        private static void ResetSpecialUses( IReadOnlyList< UnitController > units )
        {
            if ( units == null ) return;

            foreach ( var unit in units )
            {
                unit?.Sheet?.Equipment?.ResetSpecialUses();
            }
        }
    }
}
