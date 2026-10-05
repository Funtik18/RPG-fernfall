using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Core.Gameplay.TBS.Editor
{
    [ CustomEditor( typeof(GridEffects) ) ]
    public sealed class GridEffectsEditor : UnityEditor.Editor
    {
        private const float CellRadius = 0.42f;
        private const float CellHeightOffset = 0.08f;
        private const float ExistingEffectHeightOffset = 0.14f;

        private bool _isEditing;

        private GridEffects Effects => (GridEffects)target;

        public override void OnInspectorGUI()
        {
            var changed = DrawDefaultInspector();
            var grid = GetGrid();

            EditorGUILayout.Space( 12 );
            EditorGUILayout.LabelField( "Cell Effects Editing", EditorStyles.boldLabel );

            if ( grid == null )
            {
                EditorGUILayout.HelpBox( "GridEffects must be placed under a GridObject to edit cells in the Scene view.", MessageType.Warning );
            }
            else if ( grid.Cells == null || grid.Cells.Count == 0 )
            {
                EditorGUILayout.HelpBox( "Parent GridObject has no cells.", MessageType.Warning );
            }

            using ( new EditorGUI.DisabledScope( grid == null || grid.Cells == null || grid.Cells.Count == 0 ) )
            {
                if ( GUILayout.Button( _isEditing ? "Stop Edit" : "Edit", GUILayout.Height( 30 ) ) )
                {
                    _isEditing = !_isEditing;
                    SceneView.RepaintAll();
                }
            }

            if ( _isEditing )
            {
                EditorGUILayout.HelpBox( "Click highlighted cells in the Scene view to add them to CellDefinitions. Click existing cells again to remove them.", MessageType.Info );
            }

            if ( changed )
            {
                SceneView.RepaintAll();
            }
        }

        private void OnSceneGUI()
        {
            if ( !_isEditing )
            {
                return;
            }

            var grid = GetGrid();
            if ( grid == null || grid.Cells == null )
            {
                return;
            }

            if ( Event.current.type == EventType.Layout )
            {
                HandleUtility.AddDefaultControl( GUIUtility.GetControlID( FocusType.Passive ) );
            }

            var previousColor = Handles.color;
            var previousZTest = Handles.zTest;

            Handles.zTest = CompareFunction.Always;

            foreach ( var cell in grid.Cells )
            {
                DrawEditableCell( cell );
            }

            Handles.color = previousColor;
            Handles.zTest = previousZTest;
        }

        private void DrawEditableCell( GridCellObject cell )
        {
            if ( cell == null )
            {
                return;
            }

            var hasDefinition = Effects.GetDefinition( cell ) != null;
            var position = cell.transform.position + Vector3.up * CellHeightOffset;

            DrawCellDisc( position, GetEditCellColor( hasDefinition ), GetEditCellOutlineColor( hasDefinition ) );

            Handles.color = GridEditorPalette.HiddenHandle;

            if ( Handles.Button( position, Quaternion.identity, CellRadius, CellRadius, Handles.DotHandleCap ) )
            {
                ApplyCellEdit( cell, hasDefinition );
            }
        }

        private void ApplyCellEdit( GridCellObject cell, bool hasDefinition )
        {
            Undo.RecordObject( Effects, hasDefinition ? "Remove Grid Effect Cell" : "Add Grid Effect Cell" );

            if ( hasDefinition )
            {
                Effects.RemoveDefinition( cell );
            }
            else
            {
                Effects.AddDefinition( cell );
            }

            EditorUtility.SetDirty( Effects );
            serializedObject.Update();
            Repaint();
            SceneView.RepaintAll();
        }

        private Color GetEditCellColor( bool hasDefinition )
        {
            return hasDefinition ? GridEditorPalette.EffectExistingFill : GridEditorPalette.EffectAvailableFill;
        }

        private Color GetEditCellOutlineColor( bool hasDefinition )
        {
            return hasDefinition ? GridEditorPalette.EffectExistingOutline : GridEditorPalette.EffectAvailableOutline;
        }

        private void DrawCellDisc( Vector3 position, Color fillColor, Color outlineColor )
        {
            Handles.color = fillColor;
            Handles.DrawSolidDisc( position, Vector3.up, CellRadius );
            Handles.color = outlineColor;
            Handles.DrawWireDisc( position, Vector3.up, CellRadius );
        }

        private GridObject GetGrid()
        {
            return Effects.GetComponentInParent< GridObject >();
        }

        [ DrawGizmo( GizmoType.Selected | GizmoType.NonSelected ) ]
        private static void DrawGridEffects( GridEffects effects, GizmoType gizmoType )
        {
            if ( effects == null || effects.CellDefinitions == null )
            {
                return;
            }

            var previousColor = Handles.color;
            var previousZTest = Handles.zTest;

            Handles.zTest = CompareFunction.Always;

            foreach ( var definition in effects.CellDefinitions )
            {
                if ( definition?.Cell == null )
                {
                    continue;
                }

                var position = definition.Cell.transform.position + Vector3.up * ExistingEffectHeightOffset;
                Handles.color = GridEditorPalette.EffectExistingFill;
                Handles.DrawSolidDisc( position, Vector3.up, CellRadius * 0.72f );
                Handles.color = GridEditorPalette.EffectExistingOutline;
                Handles.DrawWireDisc( position, Vector3.up, CellRadius * 0.72f );
            }

            Handles.color = previousColor;
            Handles.zTest = previousZTest;
        }
    }
}
