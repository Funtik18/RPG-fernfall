using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class GridObject : MonoBehaviour
    {
        [ field: SerializeField ] public GridDefinitions Definitions { get; private set; }
        [ field: SerializeField ] public GridEffects Effects { get; private set; }
        [ field: Sirenix.OdinInspector.Required ]
        [ field: SerializeField ] public GridCellTerrainConfig DefaultTerrainConfig { get; private set; }
        [ field: SerializeField ] public List< GridCellObject > Cells { get; private set; } = new();

        private bool _cacheDirty = true;
        
        private readonly Dictionary< GridPosition, GridCellObject > _cellsByPosition = new();
        private readonly HashSet< GridCellObject > _cellSet = new();

        public void Initialize()
        {
            RebuildCache();
        }
        
        public void SetCells( List< GridCellObject > cells )
        {
            Cells.Clear();
            Cells.AddRange( cells );
            _cacheDirty = true;
        }

        public bool Contains( GridCellObject cell )
        {
            RebuildCacheIfNeeded();

            return cell != null && _cellSet.Contains( cell );
        }

        public bool TryGetCell( GridPosition position, out GridCellObject cell )
        {
            RebuildCacheIfNeeded();

            return _cellsByPosition.TryGetValue( position, out cell );
        }

        public IEnumerable< GridCellObject > GetWalkConnections( GridCellObject cell )
        {
            if ( cell == null || cell.Connections == null )
            {
                yield break;
            }

            foreach ( var connection in cell.Connections )
            {
                if ( !CanWalk( cell, connection ) )
                {
                    continue;
                }

                yield return connection;
            }
        }

        public bool CanWalk( GridCellObject from, GridCellObject to )
        {
            if ( from == null || to == null ) return false;
            if ( from == to ) return false;
            if ( !Contains( from ) || !Contains( to ) ) return false;
            if ( from.Connections == null || !from.Connections.Contains( to ) ) return false;

            return Mathf.Abs( from.Position.Y - to.Position.Y ) <= 1;//MaxWalkHeightDifference
        }

        private void RebuildCacheIfNeeded()
        {
            if ( !_cacheDirty ) return;
            RebuildCache();
        }

        private void RebuildCache()
        {
            _cellsByPosition.Clear();
            _cellSet.Clear();

            if ( Cells == null )
            {
                _cacheDirty = false;
                return;
            }

            foreach ( var cell in Cells )
            {
                if ( cell == null )
                {
                    continue;
                }

                _cellSet.Add( cell );

                if ( !_cellsByPosition.ContainsKey( cell.Position ) )
                {
                    _cellsByPosition.Add( cell.Position, cell );
                }
            }

            _cacheDirty = false;
        }
        
        private void OnValidate()
        {
            _cacheDirty = true;
        }
    }
}
