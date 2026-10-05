using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core.Gameplay.TBS
{
    public sealed class RetreatAIBehaviour : IAIBehaviour
    {
        private readonly BattleEnemyRetreatController _retreatController;

        public RetreatAIBehaviour( BattleEnemyRetreatController retreatController )
        {
            _retreatController = retreatController ?? throw new ArgumentNullException( nameof(retreatController) );
        }

        public async UniTask Execute( AIContext context )
        {
            if ( !context.TryGetUnitCell( context.Unit, out var currentCell ) )
            {
                _retreatController.CompleteRetreat( context.Unit );
                context.CompleteUnitTurn();
                return;
            }

            if ( IsMapEdgeCell( currentCell ) )
            {
                CompleteRetreat( context );
                return;
            }

            var targetCell = GetRetreatMoveTarget( context, currentCell );
            if ( targetCell != null )
            {
                await context.TryMove( targetCell );
            }

            if ( context.TryGetUnitCell( context.Unit, out currentCell ) && IsMapEdgeCell( currentCell ) )
            {
                CompleteRetreat( context );
                return;
            }

            context.CompleteUnitTurn();
        }

        private BattleCell GetRetreatMoveTarget( AIContext context, BattleCell currentCell )
        {
            var path = GetPathToNearestMapEdge( context, currentCell );
            if ( path.Count == 0 ) return null;

            BattleCell targetCell = null;

            for ( int i = 0; i < path.Count; i++ )
            {
                var currentPath = path.Take( i + 1 ).ToArray();
                var moveCost = context.Pathfinder.GetPathMovementCost( context.Unit, currentPath, currentCell );

                if ( !context.Unit.CanSpendMovePoints( moveCost ) )
                {
                    break;
                }

                targetCell = path[ i ];
            }

            return targetCell;
        }

        private IReadOnlyList< BattleCell > GetPathToNearestMapEdge( AIContext context, BattleCell currentCell )
        {
            IReadOnlyList< BattleCell > path = Array.Empty< BattleCell >();
            var pathCost = int.MaxValue;

            foreach ( var edgeCell in GetMapCells( currentCell ).Where( IsMapEdgeCell ) )
            {
                if ( context.GridController.Registry.IsOccupied( edgeCell ) )
                {
                    continue;
                }

                var currentPath = context.Pathfinder.FindPath( currentCell, edgeCell, context.Unit );
                if ( currentPath.Count == 0 )
                {
                    continue;
                }

                var currentPathCost = context.Pathfinder.GetPathMovementCost( context.Unit, currentPath, currentCell );
                if ( currentPathCost >= pathCost )
                {
                    continue;
                }

                path = currentPath;
                pathCost = currentPathCost;
            }

            return path;
        }

        private IReadOnlyCollection< BattleCell > GetMapCells( BattleCell startCell )
        {
            var cells = new HashSet< BattleCell >();
            var open = new Queue< BattleCell >();

            cells.Add( startCell );
            open.Enqueue( startCell );

            while ( open.Count > 0 )
            {
                var cell = open.Dequeue();
                foreach ( var connection in cell.Connections )
                {
                    if ( connection?.Cell == null ) continue;
                    if ( !cells.Add( connection.Cell ) ) continue;

                    open.Enqueue( connection.Cell );
                }
            }

            return cells;
        }

        private void CompleteRetreat( AIContext context )
        {
            var unit = context.Unit;
            context.GridController.Registry.Remove( unit );
            unit.Retreat();
            unit.View.gameObject.SetActive( false );
            _retreatController.CompleteRetreat( unit );

            context.CompleteUnitTurn();
        }

        private bool IsMapEdgeCell( BattleCell cell )
        {
            if ( cell?.Grid?.Cells == null ) return false;

            var position = cell.View.Position;
            return !HasCellAt( cell.Grid, position.X + 1, position.Z )
                   || !HasCellAt( cell.Grid, position.X - 1, position.Z )
                   || !HasCellAt( cell.Grid, position.X, position.Z + 1 )
                   || !HasCellAt( cell.Grid, position.X, position.Z - 1 );
        }

        private bool HasCellAt( GridObject grid, int x, int z )
        {
            foreach ( var cell in grid.Cells )
            {
                if ( cell == null ) continue;
                if ( cell.Position.X != x ) continue;
                if ( cell.Position.Z != z ) continue;

                return true;
            }

            return false;
        }
    }
}
