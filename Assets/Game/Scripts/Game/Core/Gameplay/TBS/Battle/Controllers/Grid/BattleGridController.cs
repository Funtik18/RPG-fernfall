using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleGridController
    {
        public BattleGridRegistry Registry { get; } = new();
        
        private Dictionary< GridCellObject, BattleCell > _cells = new();
        
        private BattleCell _hoveredCell;
        private BattleCell _selectedCell;
        private UnitController _selectedUnit;

        private readonly BattleObject _view;
        
        public BattleGridController( BattleObject view )
        {
            _view = view ?? throw new ArgumentNullException( nameof(view) );
        }
        
        public void Dispose()
        {
            foreach ( var grid in _view.Grids )
            {
                if ( grid == null || grid.Cells == null ) continue;

                var cells = grid.Cells;
                foreach ( var cell in cells )
                {
                    if ( cell == null ) continue;

                    cell.OnPointerClicked -= CellClickedHandler;
                    cell.OnPointerEntered -= CellEnteredHandler;
                    cell.OnPointerExited -= CellExitedHandler;
                }
            }
        }
        
        public UniTask Create()
        {
            _cells.Clear();

            foreach ( var grid in _view.Grids )
            {
                if ( grid == null || grid.Cells == null ) continue;

                var cells = grid.Cells;
                foreach ( var cell in cells )
                {
                    if ( cell == null ) continue;

                    cell.OnPointerClicked += CellClickedHandler;
                    cell.OnPointerEntered += CellEnteredHandler;
                    cell.OnPointerExited += CellExitedHandler;
                
                    _cells.Add( cell, new BattleCell( cell, grid ) );
                }
            }

            foreach ( var grid in _view.Grids )
            {
                if ( grid == null || grid.Cells == null ) continue;
                grid.Initialize();

                foreach ( var view in grid.Cells )
                {
                    if ( view == null ) continue;

                    var cell = _cells[ view ];

                    foreach ( var neighbourView in grid.GetWalkConnections( view ) )
                    {
                        if ( !_cells.TryGetValue( neighbourView, out var neighbour ) ) continue;

                        cell.AddNeighbour( neighbour );
                    }
                }
            }

            AddGridLinks();

            return UniTask.CompletedTask;
        }

        private void AddGridLinks()
        {
            if ( _view.Links == null || _view.Links.CellLinks == null )
            {
                return;
            }

            foreach ( var link in _view.Links.CellLinks )
            {
                if ( link == null || link.CellA == null || link.CellB == null ) continue;
                if ( !_cells.TryGetValue( link.CellA, out var cellA ) ) continue;
                if ( !_cells.TryGetValue( link.CellB, out var cellB ) ) continue;

                cellA.AddLink( cellB, link.TransitionType );
                cellB.AddLink( cellA, link.TransitionType );
            }
        }

        public BattleCell GetCell( GridCellObject cell ) => _cells[ cell ];

        private void CellClickedHandler( GridCellObject view )
        {
            view.EnableSelect( true );
            
            var cell = _cells[ view ];

            // if ( _selectedUnit == null )
            // {
            //     TrySelectUnit( cell );
            //     return;
            // }
            //
            // if ( cell.Unit != null )
            // {
            //     if ( CanSelect( cell.Unit ) )
            //     {
            //         SelectUnit( cell.Unit );
            //         return;
            //     }
            //
            //     if ( CanAttack( _selectedUnit, cell.Unit ) )
            //     {
            //         Attack( _selectedUnit, cell.Unit );
            //         return;
            //     }
            //
            //     return;
            // }
            //
            // if ( CanMove( _selectedUnit, cell ) )
            // {
            //     Move( _selectedUnit, cell );
            // }
        }

        private void CellEnteredHandler( GridCellObject view )
        {
            _hoveredCell?.View.EnableHover( false );
            _hoveredCell = _cells[ view ];
            _hoveredCell.View.EnableHover( true );

            if ( _selectedUnit == null )
                return;

            // if ( cell.Unit != null && CanAttack( _selectedUnit, cell.Unit ) )
            // {
            //     cell.View.SetState( GridCellVisualState.Attack );
            //     return;
            // }
            //
            // if ( CanMove( _selectedUnit, cell ) )
            // {
            //     cell.View.SetState( GridCellVisualState.Move );
            // }
        }

        private void CellExitedHandler( GridCellObject view )
        {
            var cell = _cells[ view ];
            
            _hoveredCell?.View.EnableHover( false );
            _hoveredCell = null;

            // RefreshCellVisual( cell );
        }
    }
}
