using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core.Gameplay.TBS
{
    public sealed class ChasePlayerAIBehaviour : IAIBehaviour
    {
        public async UniTask Execute( AIContext context )
        {
            var players = context.GetAlivePlayerUnits();
            if ( players.Count == 0 )
            {
                context.CompleteUnitTurn();
                return;
            }

            if ( await TryAttackAnyPlayer( context, players ) )
            {
                return;
            }

            await MoveTowardsPlayers( context, players );

            if ( await TryAttackAnyPlayer( context, players ) )
            {
                return;
            }

            context.CompleteUnitTurn();
        }

        private async UniTask< bool > TryAttackAnyPlayer( AIContext context, IReadOnlyList< UnitController > players )
        {
            var orderedPlayers = players
                .Where( x => x.IsAlive() )
                .Where( x => context.TryGetUnitCell( x, out _ ) )
                .OrderBy( x => GetDistanceToUnit( context, x ) )
                .ToArray();

            foreach ( var player in orderedPlayers )
            {
                if ( await context.TryAttack( player ) )
                {
                    return true;
                }
            }

            return false;
        }

        private async UniTask MoveTowardsPlayers( AIContext context, IReadOnlyList< UnitController > players )
        {
            var destination = context.GetReachableMoveCells()
                .OrderBy( x => GetDistanceToPlayers( context, x, players ) )
                .FirstOrDefault();

            if ( destination != null )
            {
                await context.TryMove( destination );
            }
        }

        private float GetDistanceToUnit( AIContext context, UnitController target )
        {
            if ( !context.TryGetUnitCell( context.Unit, out var unitCell ) ) return float.MaxValue;
            if ( !context.TryGetUnitCell( target, out var targetCell ) ) return float.MaxValue;

            return GetSqrDistance( unitCell, targetCell );
        }

        private float GetDistanceToPlayers( AIContext context, BattleCell cell, IReadOnlyList< UnitController > players )
        {
            var bestDistance = float.MaxValue;

            foreach ( var player in players )
            {
                if ( !player.IsAlive() ) continue;
                if ( !context.TryGetUnitCell( player, out var playerCell ) ) continue;

                var distance = GetSqrDistance( cell, playerCell );
                if ( distance < bestDistance )
                {
                    bestDistance = distance;
                }
            }

            return bestDistance;
        }

        private float GetSqrDistance( BattleCell left, BattleCell right )
        {
            return ( left.View.transform.position - right.View.transform.position ).sqrMagnitude;
        }
    }
}
