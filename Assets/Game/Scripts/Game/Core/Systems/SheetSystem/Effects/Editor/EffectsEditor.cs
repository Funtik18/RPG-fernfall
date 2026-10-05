using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CustomEditor( typeof(Effects) ) ]
    public sealed class EffectsEditor : UnityEditor.Editor
    {
        private SerializedProperty _effectsProperty;

        private readonly Dictionary< UnityEngine.Object, UnityEditor.Editor > _effectEditors = new();

        private void OnEnable()
        {
            _effectsProperty = serializedObject.FindProperty( "_effects" );
        }

        private void OnDisable()
        {
            foreach ( UnityEditor.Editor effectEditor in _effectEditors.Values )
            {
                if ( effectEditor != null )
                    DestroyImmediate( effectEditor );
            }

            _effectEditors.Clear();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawEffects();

            EditorGUILayout.Space();

            if ( GUILayout.Button( "Add Effect" ) )
                ShowEffectContextMenu();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEffects()
        {
            for ( int index = 0; index < _effectsProperty.arraySize; index++ )
            {
                SerializedProperty element = _effectsProperty.GetArrayElementAtIndex( index );
                var effectConfig = element.objectReferenceValue as EffectConfig;

                EditorGUILayout.BeginVertical( EditorStyles.helpBox );

                DrawEffectHeader( effectConfig, index );

                if ( effectConfig != null )
                    DrawEditor( effectConfig );

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space( 4 );
            }
        }

        private void DrawEffectHeader( EffectConfig effectConfig, int index )
        {
            EditorGUILayout.BeginHorizontal();

            string effectName = effectConfig != null
                ? ObjectNames.NicifyVariableName( effectConfig.GetType().Name.Replace( "EffectConfig", string.Empty ) )
                : "Missing Effect";
            string uid = effectConfig != null && !string.IsNullOrWhiteSpace( effectConfig.UID ) ? $" {effectConfig.UID}" : string.Empty;

            EditorGUILayout.LabelField( $"{index + 1}. {effectName}{uid}", EditorStyles.boldLabel );

            GUI.enabled = index > 0;

            if ( GUILayout.Button( "^", GUILayout.Width( 28 ) ) )
                _effectsProperty.MoveArrayElement( index, index - 1 );

            GUI.enabled = index < _effectsProperty.arraySize - 1;

            if ( GUILayout.Button( "v", GUILayout.Width( 28 ) ) )
                _effectsProperty.MoveArrayElement( index, index + 1 );

            GUI.enabled = true;

            if ( GUILayout.Button( "X", GUILayout.Width( 28 ) ) )
            {
                RemoveEffect( index );
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawEditor( EffectConfig effectConfig )
        {
            if ( !_effectEditors.TryGetValue( effectConfig, out UnityEditor.Editor effectEditor ) || effectEditor == null )
            {
                effectEditor = CreateEditor( effectConfig );
                _effectEditors[ effectConfig ] = effectEditor;
            }

            effectEditor.OnInspectorGUI();
        }

        private void ShowEffectContextMenu()
        {
            GenericMenu menu = new();

            IEnumerable< Type > effectTypes = TypeCache.GetTypesDerivedFrom< EffectConfig >()
                .Where( type => !type.IsAbstract && !type.IsGenericType )
                .OrderBy( type => type.Name );

            foreach ( Type effectType in effectTypes )
            {
                string effectName = ObjectNames.NicifyVariableName( effectType.Name.Replace( "EffectConfig", string.Empty ) );
                menu.AddItem( new GUIContent( effectName ), false, () => AddEffect( effectType ) );
            }

            menu.ShowAsContext();
        }

        private void AddEffect( Type effectType )
        {
            var effects = (Effects)target;
            var effectConfig = CreateInstance( effectType ) as EffectConfig;

            if ( effectConfig == null )
                return;

            effectConfig.name = ObjectNames.NicifyVariableName( effectType.Name );
            effectConfig.hideFlags = HideFlags.HideInHierarchy;

            Undo.RegisterCreatedObjectUndo( effectConfig, "Add effect" );

            AssetDatabase.AddObjectToAsset( effectConfig, effects );

            serializedObject.Update();

            int index = _effectsProperty.arraySize;
            _effectsProperty.InsertArrayElementAtIndex( index );
            _effectsProperty.GetArrayElementAtIndex( index ).objectReferenceValue = effectConfig;

            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty( effectConfig );
            EditorUtility.SetDirty( effects );

            AssetDatabase.SaveAssets();
        }

        private void RemoveEffect( int index )
        {
            serializedObject.Update();

            SerializedProperty element = _effectsProperty.GetArrayElementAtIndex( index );
            var effectConfig = element.objectReferenceValue as EffectConfig;

            element.objectReferenceValue = null;
            _effectsProperty.DeleteArrayElementAtIndex( index );

            serializedObject.ApplyModifiedProperties();

            if ( effectConfig != null )
            {
                if ( _effectEditors.TryGetValue( effectConfig, out UnityEditor.Editor effectEditor ) && effectEditor != null )
                    DestroyImmediate( effectEditor );

                _effectEditors.Remove( effectConfig );
                Undo.DestroyObjectImmediate( effectConfig );
            }

            EditorUtility.SetDirty( target );
            AssetDatabase.SaveAssets();
        }
    }
}
