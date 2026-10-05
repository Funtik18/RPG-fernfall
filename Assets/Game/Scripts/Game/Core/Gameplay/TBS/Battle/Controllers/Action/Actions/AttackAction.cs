using Cysharp.Threading.Tasks;
using System;

namespace Game.Core.Gameplay.TBS.Actions
{
    public sealed class AttackAction : BattleAction
    {
        private readonly BattleGridController _gridController;
        private readonly BattleMovementController _movementController;
        private readonly BattlePathfinder _pathfinder;
        private readonly BattleCombatController _combatController;
        private readonly BattleCombatArtsController _combatArtsController;

        public override bool CompletesUnitTurn => true;

        public AttackAction(
            UnitController unit,
            BattleGridController gridController,
            BattleMovementController movementController,
            BattlePathfinder pathfinder,
            BattleCombatController combatController,
            BattleCombatArtsController combatArtsController
            ) : base( unit )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _movementController = movementController ?? throw new ArgumentNullException( nameof(movementController) );
            _pathfinder = pathfinder ?? throw new ArgumentNullException( nameof(pathfinder) );
            _combatController = combatController ?? throw new ArgumentNullException( nameof(combatController) );
            _combatArtsController = combatArtsController ?? throw new ArgumentNullException( nameof(combatArtsController) );
        }

        public override bool CanExecute( BattleActionTarget target )
        {
            if ( target.CombatArt != null && !_combatArtsController.CanUseCombatArt( Owner, target.CombatArt ) ) return false;

            return TryGetAttackDestination( target, out _ );
        }

        public override async UniTask Execute( BattleActionTarget target )
        {
            if ( !TryGetAttackDestination( target, out var destination ) ) return;

            var currentCell = _gridController.Registry.GetUnitCell( Owner );
            if ( destination != currentCell )
            {
                await _movementController.Move( Owner, destination );

                if ( _gridController.Registry.GetUnitCell( Owner ) != destination )
                {
                    return;
                }

                Owner.CompleteMove();
            }

            await _combatController.Attack( Owner, target.Unit, target.CombatArt );
        }

        public bool TryGetAttackDestination( BattleActionTarget target, out BattleCell destination )
        {
            destination = null;

            if ( target.Cell == null ) return false;

            var targetUnit = target.Unit;
            if ( !_combatController.IsCanAttack( Owner, targetUnit ) ) return false;

            var currentCell = _gridController.Registry.GetUnitCell( Owner );
            var attackRange = Owner.GetAttackRangeAfterMovement();

            if ( CanAttackFrom( currentCell, target.Cell, attackRange ) )
            {
                destination = currentCell;
                return true;
            }

            if ( !Owner.CanMove ) return false;

            var reachableCells = _pathfinder.GetReachableCells( currentCell, Owner, Owner.RemainingMovePoints );
            var bestMoveCost = int.MaxValue;

            foreach ( var cell in reachableCells )
            {
                if ( !CanAttackFrom( cell, target.Cell, attackRange ) )
                {
                    continue;
                }

                var path = _pathfinder.FindPath( currentCell, cell, Owner, Owner.RemainingMovePoints );
                if ( path == null || path.Count == 0 )
                {
                    continue;
                }

                var moveCost = _pathfinder.GetPathMovementCost( Owner, path, currentCell );
                if ( moveCost >= bestMoveCost )
                {
                    continue;
                }

                bestMoveCost = moveCost;
                destination = cell;
            }

            return destination != null;
        }

        private bool CanAttackFrom( BattleCell sourceCell, BattleCell targetCell, int attackRange )
        {
            if ( attackRange <= 0 ) return false;

            var attackCells = _pathfinder.GetAttackCells( sourceCell, Owner, attackRange );
            foreach ( var cell in attackCells )
            {
                if ( cell == targetCell )
                {
                    return true;
                }
            }

            return false;
        }
    }
}
