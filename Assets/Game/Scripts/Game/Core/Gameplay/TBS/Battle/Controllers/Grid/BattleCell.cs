using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleCell
    {
        public GridCellObject View { get; }
        public GridObject Grid { get; }

        public GridCellTerrainConfig Terrain => View.Terrain;
        public float Avoid => View.Terrain?.Avoid ?? Grid.DefaultTerrainConfig.Avoid;
        public int MoveCost => View.Terrain?.MoveCost ?? Grid.DefaultTerrainConfig.MoveCost;
        
        public IReadOnlyList< BattleCell > Neighbours => _neighbours;
        public IReadOnlyList< BattleCellConnection > Connections => _connections;

        private List< BattleCell > _neighbours = new();
        private List< BattleCellConnection > _connections = new();
            
        public BattleCell( GridCellObject view, GridObject grid )
        {
            View = view ?? throw new ArgumentNullException( nameof(view) );
            Grid = grid ?? throw new ArgumentNullException( nameof(grid) );
        }

        public void AddNeighbour( BattleCell cell )
        {
            if ( cell == null ) return;

            AddConnection( new BattleCellConnection( cell ) );
        }

        public void AddLink( BattleCell cell, GridCellTransitionType transitionType )
        {
            if ( cell == null ) return;

            AddConnection( new BattleCellConnection( cell, transitionType ) );
        }

        public bool TryGetConnection( BattleCell cell, out BattleCellConnection connection )
        {
            foreach ( var current in _connections )
            {
                if ( current.Cell != cell )
                {
                    continue;
                }

                connection = current;
                return true;
            }

            connection = null;
            return false;
        }

        private void AddConnection( BattleCellConnection connection )
        {
            if ( connection == null ) return;
            if ( connection.Cell == null ) return;
            if ( connection.Cell == this ) return;

            for ( int i = 0; i < _connections.Count; i++ )
            {
                if ( _connections[ i ].Cell != connection.Cell )
                {
                    continue;
                }

                if ( connection.IsGridLink )
                {
                    _connections[ i ] = connection;
                }

                return;
            }

            _connections.Add( connection );
            _neighbours.Add( connection.Cell );
        }
    }
}
