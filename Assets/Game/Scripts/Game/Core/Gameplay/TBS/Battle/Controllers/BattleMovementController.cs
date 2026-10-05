using Cysharp.Threading.Tasks;
using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleMovementController
    {
        private readonly BattleGridController _gridController;
        private readonly BattlePathfinder _pathfinder;

        public BattleMovementController(
            BattleGridController gridController,
            BattlePathfinder pathfinder
            )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _pathfinder = pathfinder ?? throw new ArgumentNullException( nameof(pathfinder) );
        }

        public bool CanMove( UnitController unit, BattleCell target )
        {
            if ( unit == null ) return false;
            if ( target == null ) return false;
            // if ( target.Unit != null ) return false;

            var currentCell = _gridController.Registry.GetUnitCell( unit );
            if ( currentCell == target ) return false;

            var path = _pathfinder.FindPath( currentCell, target, unit );
            if ( path == null || path.Count == 0 ) return false;

            var moveCost = _pathfinder.GetPathMovementCost( unit, path, currentCell );
            return unit.CanSpendMovePoints( moveCost );
        }

        public async UniTask Move( UnitController unit, BattleCell target )
        {
            var currentCell = _gridController.Registry.GetUnitCell( unit );
            var path = _pathfinder.FindPath( currentCell, target, unit, unit.RemainingMovePoints );
            if ( path == null || path.Count == 0 ) return;

            var moveCost = _pathfinder.GetPathMovementCost( unit, path, currentCell );
            if ( !unit.CanSpendMovePoints( moveCost ) ) return;
            
            await unit.Movement.Move( currentCell, path );
            _gridController.Registry.Move( unit, target );
            
            unit.SpendMovePoints( moveCost );
        }
    }
}
