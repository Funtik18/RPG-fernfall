using SoosvetGames.CommonTools.Extensions;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
#endif

namespace Game.Core.Gameplay.TBS
{
    public sealed class GridBuilder : MonoBehaviour
    {
        [ Title( "Main" ) ]
        [ SerializeField ] private GridObject _grid;
        [ SerializeField ] private Transform _cellsRoot;
        [ SerializeField ] private GridCellObject _cellPrefab;
        
        [ Title( "Size" ) ]
        [ SerializeField ] private int _width = 10;
        [ SerializeField ] private int _depth = 10;

        [ Title( "Grid" ) ]
        [ SerializeField ] private float _cellSize = 1f;
        [ SerializeField ] private int _gridLevel;
        [ SerializeField ] private float _worldHeight;
        [ SerializeField ] private bool _spawnFromCenter;

#if UNITY_EDITOR
        [ Button( "Create Grid", ButtonSizes.Large ) ]
        private void CreateGrid()
        {
            if ( _grid == null )
            {
                Debug.LogError( "[Grid] Grid prefab is not assigned.", this );
                return;
            }

            if ( _cellPrefab == null )
            {
                Debug.LogError( "[Grid] GridCell prefab is not assigned.", this );
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo( gameObject, "Create Grid" );

            _cellsRoot?.DestroyChildren( true );

            Dictionary< GridPosition, GridCellObject > cellsMap = new( _width * _depth );
            List< GridCellObject > cells = new( _width * _depth );
            for ( int x = 0; x < _width; x++ )
            {
                for ( int z = 0; z < _depth; z++ )
                {
                    var cell = CreateCell( x, z );

                    var position = new GridPosition( x, _gridLevel, z );

                    cells.Add( cell );
                    cellsMap.Add( position, cell );
                }
            }
            _grid.SetCells( cells );

            CreateConnections( cellsMap );
        }

        private GridCellObject CreateCell( int x, int z )
        {
            Transform parent = _cellsRoot == null ? transform : _cellsRoot;
            GridCellObject cell = (GridCellObject)PrefabUtility.InstantiatePrefab( _cellPrefab, parent );

            cell.name = $"Cell [{x}, {_gridLevel}, {z}]";
            cell.transform.localPosition = GetCellLocalPosition( x, z );
            cell.SetPosition( new GridPosition( x, _gridLevel, z ) );

            Undo.RegisterCreatedObjectUndo( cell.gameObject, "Create Grid Cell" );
            EditorUtility.SetDirty( cell );

            return cell;
        }
        
        private Vector3 GetCellLocalPosition( int x, int z )
        {
            float positionX = x * _cellSize;
            float positionZ = z * _cellSize;

            if ( _spawnFromCenter )
            {
                positionX -= ( _width - 1 ) * _cellSize * 0.5f;
                positionZ -= ( _depth - 1 ) * _cellSize * 0.5f;
            }

            return new Vector3( positionX, _worldHeight, positionZ );
        }

        private void CreateConnections( Dictionary< GridPosition, GridCellObject > cells )
        {
            foreach ( var pair in cells )
            {
                var position = pair.Key;
                var cell = pair.Value;

                List< GridCellObject > connections = new( 4 );
                TryAddConnection( cells, connections, new GridPosition( position.X + 1, _gridLevel, position.Z ) );
                TryAddConnection( cells, connections, new GridPosition( position.X - 1, _gridLevel, position.Z ) );
                TryAddConnection( cells, connections, new GridPosition( position.X, _gridLevel, position.Z + 1 ) );
                TryAddConnection( cells, connections, new GridPosition( position.X, _gridLevel, position.Z - 1 ) );
                cell.SetConnections( connections );

                EditorUtility.SetDirty( cell );
            }
        }

        private void TryAddConnection( Dictionary< GridPosition, GridCellObject > cells, List< GridCellObject > connections, GridPosition position )
        {
            if ( !cells.TryGetValue( position, out var cell ) )
            {
                return;
            }

            connections.Add( cell );
        }
#endif
    }
}