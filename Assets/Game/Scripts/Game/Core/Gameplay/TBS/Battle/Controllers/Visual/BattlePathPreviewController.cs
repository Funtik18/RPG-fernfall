using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattlePathPreviewController
    {
        private LineRenderer LineRenderer
        {
            get
            {
                if ( _lineRenderer == null )
                {
                    _lineRenderer = GameObject.Instantiate( _config.LineRendererPrefab );
                    _lineRenderer.enabled = false;
                }

                return _lineRenderer;
            }
        }
        private LineRenderer _lineRenderer;

        private bool _isActionExecuting;
        private Battle _battle;
        
        private readonly TBSGameplayConfig _config;
        private readonly BattlePathfinder _pathfinder;
        private readonly BattleInputController _inputController;
        private readonly BattleGridController _gridController;
        private readonly BattleSelectionController _selectionController;
        private readonly BattleActionController _actionController;

        public BattlePathPreviewController(
            TBSGameplayConfig config,
            BattlePathfinder pathfinder,
            BattleInputController inputController,
            BattleGridController gridController,
            BattleSelectionController selectionController,
            BattleActionController actionController
            )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _pathfinder = pathfinder ?? throw new ArgumentNullException( nameof(pathfinder) );
            _inputController = inputController ?? throw new ArgumentNullException( nameof(inputController) );
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _selectionController = selectionController ?? throw new ArgumentNullException( nameof(selectionController) );
            _actionController = actionController ?? throw new ArgumentNullException( nameof(actionController) );
        }

        public void Initialize( Battle battle )
        {
            _battle = battle ?? throw new ArgumentNullException( nameof(battle) );

            _actionController.OnActionExecute += ActionExecuteHandler;
            _actionController.OnActionExecuted += ActionExecutedHandler;
            _inputController.OnPointerOverUIChanged += PointerOverUIChangedHandler;
            _inputController.OnCellHovered += CellHoveredHandler;
            _inputController.OnCellHoverExited += CellHoverExitedHandler;
            _selectionController.OnUnitSelected += UnitSelectedHandler;
            _selectionController.OnUnitDeselected += UnitDeselectedHandler;
        }

        public void Dispose()
        {
            _actionController.OnActionExecute -= ActionExecuteHandler;
            _actionController.OnActionExecuted -= ActionExecutedHandler;
            _inputController.OnPointerOverUIChanged -= PointerOverUIChangedHandler;
            _inputController.OnCellHovered -= CellHoveredHandler;
            _inputController.OnCellHoverExited -= CellHoverExitedHandler;
            _selectionController.OnUnitSelected -= UnitSelectedHandler;
            _selectionController.OnUnitDeselected -= UnitDeselectedHandler;

            _battle = null;

            if ( _lineRenderer != null )
            {
                GameObject.Destroy( _lineRenderer.gameObject );

                _lineRenderer = null;
            }
        }

        public void ShowLine( BattleCell target )
        {
            if ( _isActionExecuting || _inputController.IsPointerOverUI )
            {
                HideLine();
                return;
            }

            var unit = _selectionController.SelectedUnit;
            if ( unit == null )
            {
                HideLine();
                return;
            }

            if ( target == null )
            {
                HideLine();
                return;
            }

            if ( !unit.CanMove )
            {
                HideLine();
                return;
            }

            var start = _gridController.Registry.GetUnitCell( unit );
            if ( start == target )
            {
                HideLine();
                return;
            }

            var path = GetPreviewPath( unit, start, target );
            if ( path == null || path.Count == 0 )
            {
                HideLine();
                return;
            }

            ShowLine( unit, path );
        }

        private IReadOnlyList< BattleCell > GetPreviewPath( UnitController unit, BattleCell start, BattleCell target )
        {
            if ( _gridController.Registry.IsOccupied( target ) )
            {
                return GetOccupiedTargetPreviewPath( unit, start, target );
            }

            return GetReachableTargetPreviewPath( unit, start, target );
        }

        private IReadOnlyList< BattleCell > GetReachableTargetPreviewPath( UnitController unit, BattleCell start, BattleCell target )
        {
            var directPath = _pathfinder.FindPath( start, target, unit, unit.RemainingMovePoints );
            if ( directPath != null && directPath.Count > 0 )
            {
                return directPath;
            }

            var attackRange = unit.GetAttackRangeAfterMovement();
            if ( CanAttackFrom( unit, start, target, attackRange ) )
            {
                return Array.Empty< BattleCell >();
            }

            var reachableCells = _pathfinder.GetReachableCells( start, unit, unit.RemainingMovePoints );
            IReadOnlyList< BattleCell > bestPath = null;
            var bestMoveCost = int.MaxValue;

            foreach ( var cell in reachableCells )
            {
                if ( !CanAttackFrom( unit, cell, target, attackRange ) )
                {
                    continue;
                }

                var path = _pathfinder.FindPath( start, cell, unit, unit.RemainingMovePoints );
                if ( path == null || path.Count == 0 )
                {
                    continue;
                }

                var moveCost = _pathfinder.GetPathMovementCost( unit, path, start );
                if ( moveCost >= bestMoveCost )
                {
                    continue;
                }

                bestPath = path;
                bestMoveCost = moveCost;
            }

            return bestPath ?? Array.Empty< BattleCell >();
        }

        private IReadOnlyList< BattleCell > GetOccupiedTargetPreviewPath( UnitController unit, BattleCell start, BattleCell target )
        {
            var targetUnit = _gridController.Registry.GetUnit( target );
            if ( !CanPreviewAttackTarget( unit, targetUnit ) )
            {
                return Array.Empty< BattleCell >();
            }

            var attackRange = unit.GetAttackRangeAfterMovement();
            if ( CanAttackFrom( unit, start, target, attackRange ) )
            {
                return Array.Empty< BattleCell >();
            }

            var reachableCells = _pathfinder.GetReachableCells( start, unit, unit.RemainingMovePoints );
            IReadOnlyList< BattleCell > bestPath = null;
            var bestMoveCost = int.MaxValue;

            foreach ( var cell in reachableCells )
            {
                if ( !CanAttackFrom( unit, cell, target, attackRange ) )
                {
                    continue;
                }

                var path = _pathfinder.FindPath( start, cell, unit, unit.RemainingMovePoints );
                if ( path == null || path.Count == 0 )
                {
                    continue;
                }

                var moveCost = _pathfinder.GetPathMovementCost( unit, path, start );
                if ( moveCost >= bestMoveCost )
                {
                    continue;
                }

                bestPath = path;
                bestMoveCost = moveCost;
            }

            return bestPath ?? Array.Empty< BattleCell >();
        }

        private bool CanPreviewAttackTarget( UnitController unit, UnitController target )
        {
            if ( target == null ) return false;
            if ( target == unit ) return false;

            return _battle.GetTeam( unit ) != _battle.GetTeam( target );
        }

        private bool CanAttackFrom( UnitController unit, BattleCell source, BattleCell target, int attackRange )
        {
            if ( attackRange <= 0 ) return false;

            var attackCells = _pathfinder.GetAttackCells( source, unit, attackRange );
            foreach ( var cell in attackCells )
            {
                if ( cell == target )
                {
                    return true;
                }
            }

            return false;
        }

        private void ShowLine( UnitController unit, IReadOnlyList< BattleCell > path )
        {
            if ( unit == null || path == null || path.Count == 0 )
            {
                HideLine();
                return;
            }

            LineRenderer.positionCount = path.Count + 1;
            LineRenderer.SetPosition( 0, GetPosition( unit ) );

            for ( int i = 0; i < path.Count; i++ )
            {
                LineRenderer.SetPosition( i + 1, GetPosition( path[ i ] ) );
            }

            LineRenderer.enabled = true;
        }

        public void HideLine()
        {
            if ( _lineRenderer == null )
                return;

            _lineRenderer.positionCount = 0;
            _lineRenderer.enabled = false;
        }

        private Vector3 GetPosition( BattleCell cell ) => cell.View.transform.position + Vector3.up * 0.1f;

        private Vector3 GetPosition( UnitController unit ) => unit.View.transform.position + Vector3.up * 0.1f;

        private void CellHoveredHandler( BattleCell cell )
        {
            ShowLine( cell );
        }

        private void CellHoverExitedHandler( BattleCell cell )
        {
            HideLine();
        }

        private void UnitSelectedHandler( UnitController unit )
        {
            ShowLineForHoveredCell();
        }

        private void UnitDeselectedHandler( UnitController unit )
        {
            HideLine();
        }

        private void ActionExecuteHandler()
        {
            _isActionExecuting = true;
            HideLine();
        }

        private void ActionExecutedHandler()
        {
            _isActionExecuting = false;
        }

        private void PointerOverUIChangedHandler( bool isPointerOverUI )
        {
            if ( isPointerOverUI )
            {
                HideLine();
                return;
            }

            ShowLineForHoveredCell();
        }

        private void ShowLineForHoveredCell()
        {
            var hoveredCell = _inputController.HoveredCell;
            if ( hoveredCell == null )
                return;

            ShowLine( _gridController.GetCell( hoveredCell ) );
        }
    }
}
