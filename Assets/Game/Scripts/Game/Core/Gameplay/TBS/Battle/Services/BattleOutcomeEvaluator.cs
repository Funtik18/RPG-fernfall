using System;
using System.Linq;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleOutcomeEvaluator
    {
        public bool TryEvaluate( Battle battle, out BattleResult result )
        {
            if ( battle == null ) throw new ArgumentNullException( nameof(battle) );

            result = null;

            var playerTeam = GetTeam( battle, BattleParams.PLAYER_TEAM );
            var enemyTeam = GetTeam( battle, BattleParams.ENEMY_TEAM );

            if ( playerTeam == null || enemyTeam == null )
            {
                return false;
            }

            var playerAlive = HasAliveUnits( playerTeam );
            var enemyAlive = HasAliveUnits( enemyTeam );

            if ( !playerAlive )
            {
                result = new BattleResult( BattleResultType.Defeat, enemyTeam, battle.RoundNumber.Value );

                return true;
            }

            if ( !enemyAlive )
            {
                result = new BattleResult( BattleResultType.Victory, playerTeam, battle.RoundNumber.Value );

                return true;
            }

            return false;
        }

        private BattleTeam GetTeam( Battle battle, string teamId )
        {
            return battle.Teams.FirstOrDefault( x => string.Equals( x.Id, teamId, StringComparison.InvariantCultureIgnoreCase ) );
        }

        private bool HasAliveUnits( BattleTeam team )
        {
            return team.Units.Any( ( x ) => x.IsInBattle() );
        }
    }
}
