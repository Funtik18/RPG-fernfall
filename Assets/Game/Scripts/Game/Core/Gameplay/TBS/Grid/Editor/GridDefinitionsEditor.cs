using Game.Core.Gameplay.TBS;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Core.Gameplay.TBS.Editor
{
    [ CustomEditor( typeof(GridDefinitions) ) ]
    public sealed class GridDefinitionsEditor : UnityEditor.Editor
    {
        private UnitConfig _characaterConfig;

        private GridDefinitions Definitions => (GridDefinitions)target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space( 12 );
            EditorGUILayout.LabelField( "Cell Marking", EditorStyles.boldLabel );

            _characaterConfig = (UnitConfig)EditorGUILayout.ObjectField( "Character", _characaterConfig, typeof(UnitConfig), false );

            EditorGUILayout.Space( 4 );

            using ( new EditorGUI.DisabledScope( _characaterConfig == null ) )
            {
                var previousColor = GUI.backgroundColor;

                GUI.backgroundColor = _characaterConfig != null ? _characaterConfig.SpawnGroup.EditorColor : GridEditorPalette.DefaultControl;

                if ( GUILayout.Button( "Apply Marker To Selected Cells", GUILayout.Height( 32 ) ) )
                {
                    ApplyMarker();
                }

                GUI.backgroundColor = previousColor;
            }

            EditorGUILayout.Space( 4 );

            {
                var previousColor = GUI.backgroundColor;

                GUI.backgroundColor = GridEditorPalette.ClearAction;

                if ( GUILayout.Button( "Clear Selected Cells", GUILayout.Height( 28 ) ) )
                {
                    ClearSelected();
                }

                GUI.backgroundColor = previousColor;
            }
        }

        private void ApplyMarker()
        {
            var cells = GetSelectedCells();

            if ( cells.Length == 0 )
            {
                Debug.LogWarning( "[Grid] No GridCellObject selected." );

                return;
            }

            Undo.RecordObject( Definitions, "Mark Grid Cells" );

            foreach ( var cell in cells )
            {
                Definitions.SetMarker( cell, _characaterConfig );
            }

            EditorUtility.SetDirty( Definitions );
            SceneView.RepaintAll();
        }

        private void ClearSelected()
        {
            var cells = GetSelectedCells();

            if ( cells.Length == 0 )
                return;

            Undo.RecordObject( Definitions, "Clear Grid Cell Markers" );

            foreach ( var cell in cells )
            {
                Definitions.RemoveDefinition( cell );
            }

            EditorUtility.SetDirty( Definitions );
            SceneView.RepaintAll();
        }

        private static GridCellObject[] GetSelectedCells()
        {
            return Selection.gameObjects.Select( x => x.GetComponent< GridCellObject >() ).Where( x => x != null ).Distinct().ToArray();
        }

        [ DrawGizmo( GizmoType.Selected | GizmoType.NonSelected ) ]
        private static void DrawGridDefinitions( GridDefinitions definitions, GizmoType gizmoType )
        {
            if ( definitions == null )
                return;

            foreach ( var definition in definitions.CellDefinitions )
            {
                if ( definition.Cell == null )
                    continue;

                if ( definition.Unit == null )
                    continue;

                DrawCell( definition );
                DrawUnitTransform( definition.UnitTransform, definition.Cell.transform.position + Vector3.up * 0.5f );
            }
        }

        private static void DrawCell( GridCellDefinition definition )
        {
            var cell = definition.Cell;
            var character = definition.Unit;

            var position = cell.transform.position + Vector3.up * 0.05f;
            var color1 = character.SpawnGroup.EditorColor;
            var color2 = color1;
            color2.a = GridEditorPalette.MarkerFillAlpha;
            Handles.color = color2;
            Handles.DrawSolidDisc( position, Vector3.up, 0.4f );
            Handles.color = color1;
            Handles.DrawWireDisc( position, Vector3.up, 0.4f );
            
            GUIStyle style = new GUIStyle( EditorStyles.boldLabel );
            style.normal.textColor = GridEditorPalette.MarkerLabel;
            Handles.Label( position, definition.Unit.UID, style );
        }

        private static void DrawUnitTransform( UnitTransform unitTransform, Vector3 position )
        {
            var forward = unitTransform.Forward;
            forward.y = 0.0f;

            if ( forward.sqrMagnitude < 0.0001f )
                forward = Vector3.forward;

            forward.Normalize();

            var right = Vector3.Cross( Vector3.up, forward ).normalized;
            var up = Vector3.Cross( forward, right ).normalized;

            var origin = position + up * 0.03f;

            const float forwardLength = 0.55f;
            const float sideLength = 0.22f;
            const float lineWidth = 4.0f;

            // Forward
            Handles.color = GridEditorPalette.UnitForwardAxis;
            var forwardEnd = origin + forward * forwardLength;
            Handles.DrawAAPolyLine( lineWidth, origin, forwardEnd );
            Handles.ConeHandleCap( 0, forwardEnd, Quaternion.LookRotation( forward, up ), 0.12f, EventType.Repaint );

            // Right
            Handles.color = GridEditorPalette.UnitRightAxis;
            Handles.DrawAAPolyLine( lineWidth, origin, origin + right * sideLength );

            // Up
            Handles.color = GridEditorPalette.UnitUpAxis;
            Handles.DrawAAPolyLine( lineWidth, origin, origin + up * sideLength );
        }
    }
}
