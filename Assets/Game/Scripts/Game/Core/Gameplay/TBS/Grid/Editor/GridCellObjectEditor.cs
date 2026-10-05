using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.Core.Gameplay.TBS.Editor
{
    [ CustomEditor( typeof(GridCellObject) ) ]
    [ CanEditMultipleObjects ]
    public sealed class GridCellObjectEditor : OdinEditor
    {
        private const float LineHeightOffset = 0.18f;
        private const float LineWidth = 4.0f;
        private const float TerrainFillHeightOffset = 0.03f;
        private const float DefaultCellSize = 1.0f;

        private static readonly Vector3[] TerrainPolygonVertices = new Vector3[4];

        [ DrawGizmo( GizmoType.Selected | GizmoType.NonSelected ) ]
        private static void DrawGridCellConnections( GridCellObject cell, GizmoType gizmoType )
        {
            DrawTerrain( cell );
            DrawConnections( cell );
        }

        private static void DrawTerrain( GridCellObject cell )
        {
            if ( cell.Terrain != null )
            {
                Handles.color = cell.Terrain.EditorColor;
                Handles.DrawAAConvexPolygon( GetCellPolygonVertices( cell ) );
            }
        }

        private static void DrawConnections( GridCellObject cell )
        {
            if ( cell.Connections == null )
            {
                return;
            }

            var from = GetLinePosition( cell );
            foreach ( var connection in cell.Connections )
            {
                if ( connection == null || connection == cell )
                {
                    continue;
                }

                DrawConnection( cell, connection, from );
            }
        }

        private static void DrawConnection( GridCellObject fromCell, GridCellObject toCell, Vector3 from )
        {
            var to = GetLinePosition( toCell );

            Handles.color = CanReachByHeight( fromCell, toCell ) ? GridEditorPalette.ConnectionReachable : GridEditorPalette.ConnectionBlocked;
            Handles.DrawAAPolyLine( LineWidth, from, to );
        }

        private static bool CanReachByHeight( GridCellObject from, GridCellObject to )
        {
            var grid = from.GetComponentInParent< GridObject >();
            if ( grid != null && grid.Contains( to ) )
            {
                return grid.CanWalk( from, to );
            }

            return Mathf.Abs( from.Position.Y - to.Position.Y ) <= 1;
        }

        private static Vector3 GetLinePosition( GridCellObject cell )
        {
            return cell.transform.position + Vector3.up * LineHeightOffset;
        }

        private static Vector3[] GetCellPolygonVertices( GridCellObject cell )
        {
            var collider = cell.GetComponentInChildren< Collider >();
            if ( collider != null )
            {
                var bounds = collider.bounds;
                var y = bounds.max.y + TerrainFillHeightOffset;

                TerrainPolygonVertices[ 0 ] = new Vector3( bounds.min.x, y, bounds.min.z );
                TerrainPolygonVertices[ 1 ] = new Vector3( bounds.min.x, y, bounds.max.z );
                TerrainPolygonVertices[ 2 ] = new Vector3( bounds.max.x, y, bounds.max.z );
                TerrainPolygonVertices[ 3 ] = new Vector3( bounds.max.x, y, bounds.min.z );

                return TerrainPolygonVertices;
            }

            return GetDefaultCellPolygonVertices( cell );
        }

        private static Vector3[] GetDefaultCellPolygonVertices( GridCellObject cell )
        {
            var center = cell.transform.position + Vector3.up * TerrainFillHeightOffset;
            var halfSize = DefaultCellSize * 0.5f;
            var right = cell.transform.right * halfSize;
            var forward = cell.transform.forward * halfSize;

            TerrainPolygonVertices[ 0 ] = center - right - forward;
            TerrainPolygonVertices[ 1 ] = center - right + forward;
            TerrainPolygonVertices[ 2 ] = center + right + forward;
            TerrainPolygonVertices[ 3 ] = center + right - forward;

            return TerrainPolygonVertices;
        }
    }
}
