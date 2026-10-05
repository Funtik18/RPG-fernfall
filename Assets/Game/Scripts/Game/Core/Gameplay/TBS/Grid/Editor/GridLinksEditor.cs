using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Core.Gameplay.TBS.Editor
{
    [ CustomEditor( typeof(GridLinks) ) ]
    public sealed class GridLinksEditor : UnityEditor.Editor
    {
        private const float ArcHeightOffset = 0.2f;
        private const float ArcHeightDistanceFactor = 0.35f;
        private const float MinArcHeight = 0.45f;
        private const float LineWidth = 4.0f;
        private const float LabelYOffset = 0.25f;

        public override void OnInspectorGUI()
        {
            var changed = DrawDefaultInspector();

            DrawGridNameLabels();

            if ( changed )
            {
                SceneView.RepaintAll();
            }
        }

        [ DrawGizmo( GizmoType.Selected | GizmoType.NonSelected ) ]
        private static void DrawGridLinks( GridLinks links, GizmoType gizmoType )
        {
            if ( links == null || links.CellLinks == null )
            {
                return;
            }

            var previousColor = Handles.color;
            var previousZTest = Handles.zTest;

            Handles.color = GridEditorPalette.GridLink;
            Handles.zTest = CompareFunction.Always;

            for ( int i = 0; i < links.CellLinks.Count; i++ )
            {
                DrawLink( links.CellLinks[i], i );
            }

            Handles.color = previousColor;
            Handles.zTest = previousZTest;
        }

        private static void DrawLink( GridCellLink link, int index )
        {
            if ( link == null || link.CellA == null || link.CellB == null || link.CellA == link.CellB )
            {
                return;
            }

            var start = GetArcPoint( link.CellA );
            var end = GetArcPoint( link.CellB );
            var height = Mathf.Max( MinArcHeight, Vector3.Distance( start, end ) * ArcHeightDistanceFactor );
            var startTangent = start + Vector3.up * height;
            var endTangent = end + Vector3.up * height;

            Handles.DrawBezier( start, end, startTangent, endTangent, GridEditorPalette.GridLink, null, LineWidth );
            DrawLinkLabel( start, end, startTangent, endTangent, index, link.TransitionType );
        }

        private static Vector3 GetArcPoint( GridCellObject cell )
        {
            return cell.transform.position + Vector3.up * ArcHeightOffset;
        }

        private static void DrawLinkLabel( Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, int index, GridCellTransitionType transitionType )
        {
            var position = GetBezierPoint( start, end, startTangent, endTangent, 0.5f ) - Vector3.up * LabelYOffset;
            var style = new GUIStyle( EditorStyles.boldLabel )
            {
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = GridEditorPalette.GridLink;

            Handles.Label( position, $"Link {index} {transitionType}", style );
        }

        private static Vector3 GetBezierPoint( Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, float t )
        {
            var inverseT = 1.0f - t;

            return inverseT * inverseT * inverseT * start
                + 3.0f * inverseT * inverseT * t * startTangent
                + 3.0f * inverseT * t * t * endTangent
                + t * t * t * end;
        }

        private void DrawGridNameLabels()
        {
            var links = (GridLinks)target;
            if ( links == null || links.CellLinks == null || links.CellLinks.Count == 0 )
            {
                return;
            }

            EditorGUILayout.Space( 4 );

            for ( int i = 0; i < links.CellLinks.Count; i++ )
            {
                var link = links.CellLinks[i];
                if ( link == null )
                {
                    EditorGUILayout.LabelField( $"Link {i}", "None" );
                    continue;
                }

                EditorGUILayout.LabelField( $"Link {i} {link.TransitionType}", $"Cell A: {GetGridName( link.CellA )} | Cell B: {GetGridName( link.CellB )}" );
            }
        }

        private static string GetGridName( GridCellObject cell )
        {
            if ( cell == null )
            {
                return "None";
            }

            var parentGrid = cell.GetComponentInParent< GridObject >();
            if ( parentGrid != null )
            {
                return parentGrid.name;
            }

            foreach ( var grid in Resources.FindObjectsOfTypeAll< GridObject >() )
            {
                if ( grid == null || grid.Cells == null )
                {
                    continue;
                }

                if ( grid.Cells.Contains( cell ) )
                {
                    return grid.name;
                }
            }

            return "Unknown";
        }
    }
}
