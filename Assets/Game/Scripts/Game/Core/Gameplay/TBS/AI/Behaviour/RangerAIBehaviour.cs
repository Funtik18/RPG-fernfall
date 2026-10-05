using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class RangerAIBehaviour : IAIBehaviour
    {
        private const int RangeDistanceSearchPadding = 8;

        public async UniTask Execute( AIContext context )
        {
            var targets = context.GetAlivePlayerUnits();
            if ( targets.Count == 0 )
            {
                context.CompleteUnitTurn();
                return;
            }

            if ( !context.TryGetUnitCell( context.Unit, out var currentCell ) )
            {
                context.CompleteUnitTurn();
                return;
            }

            var attackRange = context.Unit.GetAttackRangeAfterMovement();
            if ( attackRange <= 0 )
            {
                context.CompleteUnitTurn();
                return;
            }

            var decision = GetBestDecision( context, currentCell, targets, attackRange );
            if ( decision == null )
            {
                context.CompleteUnitTurn();
                return;
            }

            var canUseDecisionTarget = decision.Cell == currentCell;

            if ( decision.Cell != currentCell )
            {
                canUseDecisionTarget = await context.TryMove( decision.Cell );
            }

            if ( canUseDecisionTarget && decision.Target != null && decision.Target.IsAlive() && await context.TryAttack( decision.Target ) )
            {
                return;
            }

            if ( await TryAttackFromCurrentCell( context, targets, attackRange ) )
            {
                return;
            }

            context.CompleteUnitTurn();
        }

        private PositionDecision GetBestDecision( AIContext context, BattleCell currentCell, IReadOnlyList< UnitController > targets, int attackRange )
        {
            PositionDecision best = null;

            foreach ( var cell in GetCandidateCells( context, currentCell ) )
            {
                var decision = CreateDecision( context, currentCell, cell, targets, attackRange );
                if ( decision == null )
                {
                    continue;
                }

                if ( best == null || IsBetter( decision, best, attackRange ) )
                {
                    best = decision;
                }
            }

            return best;
        }

        private IReadOnlyList< BattleCell > GetCandidateCells( AIContext context, BattleCell currentCell )
        {
            var cells = new List< BattleCell > { currentCell };

            foreach ( var cell in context.GetReachableMoveCells() )
            {
                if ( cell == null || cells.Contains( cell ) )
                {
                    continue;
                }

                cells.Add( cell );
            }

            return cells;
        }

        private PositionDecision CreateDecision(
            AIContext context,
            BattleCell currentCell,
            BattleCell cell,
            IReadOnlyList< UnitController > targets,
            int attackRange
            )
        {
            if ( cell == null )
            {
                return null;
            }

            var target = GetBestAttackTarget( context, cell, targets, attackRange, out var targetDistance );
            var nearestTarget = GetNearestTarget( context, cell, targets, attackRange );

            return new PositionDecision(
                cell,
                target,
                targetDistance,
                nearestTarget.RangeDistance,
                nearestTarget.SqrDistance,
                attackRange > 1 && IsInMeleeRange( context, cell, targets ),
                GetMoveCost( context, currentCell, cell ) );
        }

        private UnitController GetBestAttackTarget(
            AIContext context,
            BattleCell sourceCell,
            IReadOnlyList< UnitController > targets,
            int attackRange,
            out int targetDistance
            )
        {
            UnitController bestTarget = null;
            targetDistance = int.MaxValue;

            foreach ( var target in targets )
            {
                if ( target == null || !target.IsAlive() )
                {
                    continue;
                }

                if ( !context.TryGetUnitCell( target, out var targetCell ) )
                {
                    continue;
                }

                if ( !CanAttackFrom( context, sourceCell, targetCell, attackRange ) )
                {
                    continue;
                }

                var distance = GetRangeDistance( context, sourceCell, targetCell, attackRange );
                if ( bestTarget != null && !IsBetterTarget( distance, targetDistance, attackRange ) )
                {
                    continue;
                }

                bestTarget = target;
                targetDistance = distance;
            }

            return bestTarget;
        }

        private bool IsBetterTarget( int candidateDistance, int currentDistance, int attackRange )
        {
            var candidateError = GetOptimalDistanceError( candidateDistance, attackRange );
            var currentError = GetOptimalDistanceError( currentDistance, attackRange );

            if ( candidateError != currentError )
            {
                return candidateError < currentError;
            }

            return candidateDistance > currentDistance;
        }

        private bool IsBetter( PositionDecision candidate, PositionDecision current, int attackRange )
        {
            if ( candidate.IsInMeleeRange != current.IsInMeleeRange )
            {
                return !candidate.IsInMeleeRange;
            }

            if ( candidate.CanAttack != current.CanAttack )
            {
                return candidate.CanAttack;
            }

            if ( candidate.CanAttack )
            {
                var candidateError = GetOptimalDistanceError( candidate.TargetDistance, attackRange );
                var currentError = GetOptimalDistanceError( current.TargetDistance, attackRange );

                if ( candidateError != currentError )
                {
                    return candidateError < currentError;
                }

                if ( candidate.TargetDistance != current.TargetDistance )
                {
                    return candidate.TargetDistance > current.TargetDistance;
                }
            }
            else if ( IsBetterApproach( candidate, current, attackRange, out var isBetter ) )
            {
                return isBetter;
            }

            if ( candidate.NearestTargetRangeDistance != current.NearestTargetRangeDistance )
            {
                return candidate.NearestTargetRangeDistance > current.NearestTargetRangeDistance;
            }

            if ( Math.Abs( candidate.NearestTargetSqrDistance - current.NearestTargetSqrDistance ) > float.Epsilon )
            {
                return candidate.NearestTargetSqrDistance > current.NearestTargetSqrDistance;
            }

            return candidate.MoveCost < current.MoveCost;
        }

        private bool IsBetterApproach( PositionDecision candidate, PositionDecision current, int attackRange, out bool isBetter )
        {
            var candidateCanEstimateDistance = candidate.NearestTargetRangeDistance != int.MaxValue;
            var currentCanEstimateDistance = current.NearestTargetRangeDistance != int.MaxValue;

            if ( candidateCanEstimateDistance != currentCanEstimateDistance )
            {
                isBetter = candidateCanEstimateDistance;
                return true;
            }

            if ( candidateCanEstimateDistance )
            {
                var candidateError = Math.Abs( candidate.NearestTargetRangeDistance - attackRange );
                var currentError = Math.Abs( current.NearestTargetRangeDistance - attackRange );

                if ( candidateError != currentError )
                {
                    isBetter = candidateError < currentError;
                    return true;
                }
            }
            else if ( Math.Abs( candidate.NearestTargetSqrDistance - current.NearestTargetSqrDistance ) > float.Epsilon )
            {
                isBetter = candidate.NearestTargetSqrDistance < current.NearestTargetSqrDistance;
                return true;
            }

            isBetter = false;
            return false;
        }

        private async UniTask< bool > TryAttackFromCurrentCell( AIContext context, IReadOnlyList< UnitController > targets, int attackRange )
        {
            if ( !context.TryGetUnitCell( context.Unit, out var currentCell ) )
            {
                return false;
            }

            var target = GetBestAttackTarget( context, currentCell, targets, attackRange, out _ );
            return target != null && await context.TryAttack( target );
        }

        private bool CanAttackFrom( AIContext context, BattleCell sourceCell, BattleCell targetCell, int attackRange )
        {
            if ( sourceCell == null || targetCell == null )
            {
                return false;
            }

            foreach ( var cell in context.Pathfinder.GetAttackCells( sourceCell, context.Unit, attackRange ) )
            {
                if ( cell == targetCell )
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsInMeleeRange( AIContext context, BattleCell sourceCell, IReadOnlyList< UnitController > targets )
        {
            foreach ( var target in targets )
            {
                if ( target == null || !target.IsAlive() )
                {
                    continue;
                }

                if ( !context.TryGetUnitCell( target, out var targetCell ) )
                {
                    continue;
                }

                if ( CanAttackFrom( context, sourceCell, targetCell, 1 ) )
                {
                    return true;
                }
            }

            return false;
        }

        private TargetDistance GetNearestTarget( AIContext context, BattleCell sourceCell, IReadOnlyList< UnitController > targets, int attackRange )
        {
            var bestRangeDistance = int.MaxValue;
            var bestSqrDistance = float.MaxValue;
            var maxRangeDistance = Math.Max( attackRange + context.Unit.GetMovePoints() + RangeDistanceSearchPadding, attackRange );

            foreach ( var target in targets )
            {
                if ( target == null || !target.IsAlive() )
                {
                    continue;
                }

                if ( !context.TryGetUnitCell( target, out var targetCell ) )
                {
                    continue;
                }

                var rangeDistance = GetRangeDistance( context, sourceCell, targetCell, maxRangeDistance );
                var sqrDistance = GetSqrDistance( sourceCell, targetCell );

                if ( rangeDistance < bestRangeDistance
                     || rangeDistance == bestRangeDistance && sqrDistance < bestSqrDistance )
                {
                    bestRangeDistance = rangeDistance;
                    bestSqrDistance = sqrDistance;
                }
            }

            return new TargetDistance( bestRangeDistance, bestSqrDistance );
        }

        private int GetRangeDistance( AIContext context, BattleCell sourceCell, BattleCell targetCell, int maxDistance )
        {
            if ( sourceCell == targetCell )
            {
                return 0;
            }

            for ( var distance = 1; distance <= maxDistance; distance++ )
            {
                if ( CanAttackFrom( context, sourceCell, targetCell, distance ) )
                {
                    return distance;
                }
            }

            return int.MaxValue;
        }

        private int GetOptimalDistanceError( int distance, int attackRange )
        {
            if ( distance == int.MaxValue )
            {
                return int.MaxValue;
            }

            return Math.Abs( attackRange - distance );
        }

        private int GetMoveCost( AIContext context, BattleCell currentCell, BattleCell targetCell )
        {
            if ( currentCell == targetCell )
            {
                return 0;
            }

            var path = context.Pathfinder.FindPath( currentCell, targetCell, context.Unit, context.Unit.RemainingMovePoints );
            if ( path == null || path.Count == 0 )
            {
                return int.MaxValue;
            }

            return context.Pathfinder.GetPathMovementCost( context.Unit, path, currentCell );
        }

        private float GetSqrDistance( BattleCell left, BattleCell right )
        {
            return ( left.View.transform.position - right.View.transform.position ).sqrMagnitude;
        }

        private sealed class PositionDecision
        {
            public BattleCell Cell { get; }
            public UnitController Target { get; }
            public int TargetDistance { get; }
            public int NearestTargetRangeDistance { get; }
            public float NearestTargetSqrDistance { get; }
            public bool IsInMeleeRange { get; }
            public bool CanAttack => Target != null;
            public int MoveCost { get; }

            public PositionDecision(
                BattleCell cell,
                UnitController target,
                int targetDistance,
                int nearestTargetRangeDistance,
                float nearestTargetSqrDistance,
                bool isInMeleeRange,
                int moveCost
                )
            {
                Cell = cell;
                Target = target;
                TargetDistance = targetDistance;
                NearestTargetRangeDistance = nearestTargetRangeDistance;
                NearestTargetSqrDistance = nearestTargetSqrDistance;
                IsInMeleeRange = isInMeleeRange;
                MoveCost = moveCost;
            }
        }

        private readonly struct TargetDistance
        {
            public int RangeDistance { get; }
            public float SqrDistance { get; }

            public TargetDistance( int rangeDistance, float sqrDistance )
            {
                RangeDistance = rangeDistance;
                SqrDistance = sqrDistance;
            }
        }
    }
}
