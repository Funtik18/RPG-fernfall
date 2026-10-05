using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core.Gameplay.TBS
{
    public sealed class FuryAIBehaviour : IAIBehaviour
    {
        public async UniTask Execute( AIContext context )
        {
            var enemies = context.GetAliveEnemyUnits();
            if ( enemies.Count == 0 )
            {
                context.CompleteUnitTurn();
                return;
            }

            if ( await TryAttackNearestEnemy( context, enemies ) )
            {
                return;
            }

            await MoveTowardsEnemies( context, enemies );

            enemies = context.GetAliveEnemyUnits();
            if ( await TryAttackNearestEnemy( context, enemies ) )
            {
                return;
            }

            context.CompleteUnitTurn();
        }

        private async UniTask< bool > TryAttackNearestEnemy( AIContext context, IReadOnlyList< UnitController > enemies )
        {
            var orderedEnemies = enemies
                .Where( x => x.IsAlive() )
                .Where( x => context.TryGetUnitCell( x, out _ ) )
                .OrderBy( x => GetDistanceToUnit( context, x ) )
                .ToArray();

            foreach ( var enemy in orderedEnemies )
            {
                if ( await context.TryAttack( enemy ) )
                {
                    return true;
                }
            }

            return false;
        }

        private async UniTask MoveTowardsEnemies( AIContext context, IReadOnlyList< UnitController > enemies )
        {
            var destination = context.GetReachableMoveCells()
                .OrderBy( x => GetDistanceToEnemies( context, x, enemies ) )
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

        private float GetDistanceToEnemies( AIContext context, BattleCell cell, IReadOnlyList< UnitController > enemies )
        {
            var bestDistance = float.MaxValue;

            foreach ( var enemy in enemies )
            {
                if ( !enemy.IsAlive() ) continue;
                if ( !context.TryGetUnitCell( enemy, out var enemyCell ) ) continue;

                var distance = GetSqrDistance( cell, enemyCell );
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
