using Game.Core.Systems.SheetSystem;
using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleCellsPreviewController
    {
        private readonly HashSet< BattleCell > _moveCells = new();
        private readonly HashSet< BattleCell > _attackCells = new();
        
        private readonly BattleGridController _gridController;
        private readonly BattleSelectionController _selectionController;
        private readonly BattleActionController _actionController;
        private readonly BattleCombatController _combatController;
        private readonly BattlePathfinder _pathfinder;

        public BattleCellsPreviewController( 
            BattleGridController gridController,
            BattleSelectionController selectionController,
            BattleActionController actionController,
            BattleCombatController combatController,
            BattlePathfinder pathfinder
            )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _selectionController = selectionController ?? throw new ArgumentNullException( nameof(selectionController) );
            _actionController = actionController ?? throw new ArgumentNullException( nameof(actionController) );
            _combatController = combatController ?? throw new ArgumentNullException( nameof(combatController) );
            _pathfinder = pathfinder ?? throw new ArgumentNullException( nameof(pathfinder) );
        }

        public void Initialize()
        {
            _actionController.OnActionExecute += ActionExecuteHandler;
            _actionController.OnActionExecuted += ActionExecutedHandler;
            _selectionController.OnUnitSelected += UnitSelectedHandler;
            _selectionController.OnUnitDeselected += UnitDeselectedHandler;
        }

        public void Dispose()
        {
            _actionController.OnActionExecute -= ActionExecuteHandler;
            _actionController.OnActionExecuted -= ActionExecutedHandler;
            _selectionController.OnUnitSelected -= UnitSelectedHandler;
            _selectionController.OnUnitDeselected -= UnitDeselectedHandler;

            ClearPreview();
        }

        public void RefreshSelectedUnitPreview()
        {
            if ( _selectionController.SelectedUnit == null )
            {
                ClearPreview();
                return;
            }

            UnitSelectedHandler( _selectionController.SelectedUnit );
        }

        private void UnitSelectedHandler( UnitController unit )
        {
            ClearPreview();

            var startCell = _gridController.Registry.GetUnitCell( unit );

            var movePoints = unit.RemainingMovePoints;
            var moveCells = _pathfinder.GetReachableCells( startCell, unit, movePoints );
            foreach ( var cell in moveCells )
            {
                _moveCells.Add( cell );
                cell.View.EnableSelect( true );
            }

            var attackCells = GetAttackOnlyCells( startCell, unit, moveCells );
            foreach ( var cell in attackCells )
            {
                if ( IsInvalidTargetCell( unit, cell, unit.Sheet.Equipment.Weapon ) ) continue;

                _attackCells.Add( cell );
                cell.View.EnableAttack( true );
            }
        }

        private void UnitDeselectedHandler( UnitController unit )
        {
            ClearPreview();
        }

        private void ActionExecuteHandler()
        {
            ClearPreview();
        }

        private void ActionExecutedHandler()
        {
            if ( _selectionController.SelectedUnit == null ) return;
            UnitSelectedHandler( _selectionController.SelectedUnit );
        }

        private HashSet< BattleCell > GetAttackOnlyCells( BattleCell startCell, UnitController unit, IReadOnlyCollection< BattleCell > moveCells )
        {
            var attackCells = new HashSet< BattleCell >();

            var weapon = unit.Sheet.Equipment.Weapon;
            if ( !weapon.CanUse ) return attackCells;

            var attackPoints = unit.GetAttackRangeAfterMovement( weapon );

            // Attack preview is the threat range after legal movement, not a raw range from the current cell.
            AddAttackCells( attackCells, startCell, unit, attackPoints );
            foreach ( var moveCell in moveCells )
            {
                AddAttackCells( attackCells, moveCell, unit, attackPoints );
            }

            attackCells.Remove( startCell );
            attackCells.ExceptWith( moveCells );

            return attackCells;
        }

        private void AddAttackCells( HashSet< BattleCell > target, BattleCell source, UnitController unit, int attackPoints )
        {
            var attackCells = _pathfinder.GetAttackCells( source, unit, attackPoints );
            foreach ( var cell in attackCells )
            {
                target.Add( cell );
            }
        }

        private bool IsInvalidTargetCell( UnitController unit, BattleCell cell, Weapon weapon )
        {
            var targetUnit = _gridController.Registry.GetUnit( cell );
            if ( targetUnit == null ) return false;

            return !_combatController.IsCanAttack( unit, targetUnit, weapon );
        }

        private void ClearPreview()
        {
            foreach ( var cell in _moveCells )
            {
                cell.View.EnableSelect( false );
            }

            foreach ( var cell in _attackCells )
            {
                cell.View.EnableAttack( false );
            }

            _moveCells.Clear();
            _attackCells.Clear();
        }
    }
}
