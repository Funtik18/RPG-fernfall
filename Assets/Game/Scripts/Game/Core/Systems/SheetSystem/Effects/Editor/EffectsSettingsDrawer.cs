using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CustomPropertyDrawer( typeof(EffectsSettings) ) ]
    public sealed class EffectsSettingsDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 28.0f;
        private const float ButtonHeight = 22.0f;
        private const float HelpBoxHeight = 38.0f;
        private const float Padding = 6.0f;
        private const float Spacing = 4.0f;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            EditorGUI.BeginProperty( position, label, property );

            SerializedProperty effectsProperty = FindEffectsProperty( property );
            var line = new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight );

            EditorGUI.LabelField( line, label, EditorStyles.boldLabel );

            EditorGUI.indentLevel++;

            line.y += EditorGUIUtility.singleLineHeight + Spacing;

            if ( effectsProperty == null )
            {
                EditorGUI.HelpBox( new Rect( line.x, line.y, line.width, HelpBoxHeight ), "EffectsSettings list property was not found.", MessageType.Error );
                EditorGUI.indentLevel--;
                EditorGUI.EndProperty();
                return;
            }

            for ( int index = 0; index < effectsProperty.arraySize; index++ )
            {
                SerializedProperty element = effectsProperty.GetArrayElementAtIndex( index );
                float elementHeight = GetElementHeight( element );
                var elementRect = new Rect( position.x, line.y, position.width, elementHeight );

                DrawEffect( elementRect, property, effectsProperty, element, index );
                line.y += elementHeight + Spacing;
            }

            var buttonRect = new Rect( line.x, line.y, line.width, ButtonHeight );

            if ( GUI.Button( buttonRect, "Add Effect" ) )
                ShowEffectContextMenu( property );

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            float height = EditorGUIUtility.singleLineHeight;
            SerializedProperty effectsProperty = FindEffectsProperty( property );

            height += Spacing;

            if ( effectsProperty == null )
                return height + HelpBoxHeight;

            for ( int index = 0; index < effectsProperty.arraySize; index++ )
            {
                height += GetElementHeight( effectsProperty.GetArrayElementAtIndex( index ) ) + Spacing;
            }

            height += ButtonHeight;

            return height;
        }

        private void DrawEffect(
            Rect position,
            SerializedProperty rootProperty,
            SerializedProperty effectsProperty,
            SerializedProperty element,
            int index
            )
        {
            GUI.Box( position, GUIContent.none, EditorStyles.helpBox );

            var contentRect = new Rect(
                position.x + Padding,
                position.y + Padding,
                position.width - Padding * 2.0f,
                position.height - Padding * 2.0f
                );
            var headerRect = new Rect( contentRect.x, contentRect.y, contentRect.width, EditorGUIUtility.singleLineHeight );

            DrawEffectHeader( headerRect, rootProperty, effectsProperty, element, index );

            if ( element.managedReferenceValue == null )
                return;

            float propertyHeight = GetElementPropertiesHeight( element );
            var propertyRect = new Rect(
                contentRect.x,
                headerRect.yMax + Spacing,
                contentRect.width,
                propertyHeight
                );

            DrawElementProperties( propertyRect, element );
        }

        private void DrawEffectHeader(
            Rect position,
            SerializedProperty rootProperty,
            SerializedProperty effectsProperty,
            SerializedProperty element,
            int index
            )
        {
            var removeRect = new Rect( position.xMax - ButtonWidth, position.y, ButtonWidth, position.height );
            var downRect = new Rect( removeRect.x - ButtonWidth - Spacing, position.y, ButtonWidth, position.height );
            var upRect = new Rect( downRect.x - ButtonWidth - Spacing, position.y, ButtonWidth, position.height );
            var labelRect = new Rect( position.x, position.y, upRect.x - position.x - Spacing, position.height );

            EditorGUI.LabelField( labelRect, GetEffectTitle( element, index ), EditorStyles.boldLabel );

            GUI.enabled = index > 0;

            if ( GUI.Button( upRect, "^" ) )
            {
                MoveEffect( rootProperty, effectsProperty, index, index - 1 );
                GUIUtility.ExitGUI();
            }

            GUI.enabled = index < effectsProperty.arraySize - 1;

            if ( GUI.Button( downRect, "v" ) )
            {
                MoveEffect( rootProperty, effectsProperty, index, index + 1 );
                GUIUtility.ExitGUI();
            }

            GUI.enabled = true;

            if ( GUI.Button( removeRect, "X" ) )
            {
                RemoveEffect( rootProperty, effectsProperty, index );
                GUIUtility.ExitGUI();
            }
        }

        private static void ShowEffectContextMenu( SerializedProperty rootProperty )
        {
            GenericMenu menu = new();
            UnityEngine.Object[] targetObjects = rootProperty.serializedObject.targetObjects;
            string propertyPath = rootProperty.propertyPath;

            IEnumerable< Type > effectTypes = TypeCache.GetTypesDerivedFrom< EffectSettings >()
                .Where( type => !type.IsAbstract && !type.IsGenericType )
                .OrderBy( type => type.Name );

            foreach ( Type effectType in effectTypes )
            {
                string effectName = ObjectNames.NicifyVariableName( effectType.Name.Replace( "EffectSettings", string.Empty ) );
                menu.AddItem( new GUIContent( effectName ), false, () => AddEffect( targetObjects, propertyPath, effectType ) );
            }

            menu.ShowAsContext();
        }

        private static void AddEffect( UnityEngine.Object[] targetObjects, string propertyPath, Type effectType )
        {
            Undo.RecordObjects( targetObjects, "Add effect settings" );

            foreach ( UnityEngine.Object targetObject in targetObjects )
            {
                var serializedObject = new SerializedObject( targetObject );

                serializedObject.Update();

                SerializedProperty rootProperty = serializedObject.FindProperty( propertyPath );

                if ( rootProperty == null )
                    continue;

                SerializedProperty effectsProperty = FindEffectsProperty( rootProperty );

                if ( effectsProperty == null )
                    continue;

                int index = effectsProperty.arraySize;

                effectsProperty.InsertArrayElementAtIndex( index );
                effectsProperty.GetArrayElementAtIndex( index ).managedReferenceValue = Activator.CreateInstance( effectType );

                serializedObject.ApplyModifiedProperties();
            }
        }

        private static void MoveEffect(
            SerializedProperty rootProperty,
            SerializedProperty effectsProperty,
            int sourceIndex,
            int destinationIndex
            )
        {
            SerializedObject serializedObject = rootProperty.serializedObject;
            Undo.RecordObjects( serializedObject.targetObjects, "Move effect settings" );

            effectsProperty.MoveArrayElement( sourceIndex, destinationIndex );
            serializedObject.ApplyModifiedProperties();
        }

        private static void RemoveEffect( SerializedProperty rootProperty, SerializedProperty effectsProperty, int index )
        {
            SerializedObject serializedObject = rootProperty.serializedObject;
            Undo.RecordObjects( serializedObject.targetObjects, "Remove effect settings" );

            effectsProperty.DeleteArrayElementAtIndex( index );
            serializedObject.ApplyModifiedProperties();
        }

        private static float GetElementHeight( SerializedProperty element )
        {
            float height = Padding * 2.0f + EditorGUIUtility.singleLineHeight;

            if ( element.managedReferenceValue != null )
                height += Spacing + GetElementPropertiesHeight( element );

            return height;
        }

        private static SerializedProperty FindEffectsProperty( SerializedProperty property )
        {
            return property.FindPropertyRelative( "<Effects>k__BackingField" )
                ?? property.FindPropertyRelative( "_effects" );
        }

        private static void DrawElementProperties( Rect position, SerializedProperty element )
        {
            SerializedProperty property = element.Copy();
            SerializedProperty endProperty = property.GetEndProperty();
            bool enterChildren = true;
            float y = position.y;

            while ( property.NextVisible( enterChildren ) && !SerializedProperty.EqualContents( property, endProperty ) )
            {
                float propertyHeight = EditorGUI.GetPropertyHeight( property, true );
                var propertyRect = new Rect( position.x, y, position.width, propertyHeight );

                EditorGUI.PropertyField( propertyRect, property, GetPropertyLabel( property ), true );

                y += propertyHeight + Spacing;
                enterChildren = false;
            }
        }

        private static float GetElementPropertiesHeight( SerializedProperty element )
        {
            SerializedProperty property = element.Copy();
            SerializedProperty endProperty = property.GetEndProperty();
            bool enterChildren = true;
            float height = 0.0f;

            while ( property.NextVisible( enterChildren ) && !SerializedProperty.EqualContents( property, endProperty ) )
            {
                height += EditorGUI.GetPropertyHeight( property, true ) + Spacing;
                enterChildren = false;
            }

            if ( height > 0.0f )
                height -= Spacing;

            return height;
        }

        private static GUIContent GetPropertyLabel( SerializedProperty property )
        {
            string propertyName = property.name;
            const string backingFieldSuffix = ">k__BackingField";

            if ( propertyName.StartsWith( "<", StringComparison.Ordinal ) && propertyName.EndsWith( backingFieldSuffix, StringComparison.Ordinal ) )
                propertyName = propertyName.Substring( 1, propertyName.Length - backingFieldSuffix.Length - 1 );

            return new GUIContent( ObjectNames.NicifyVariableName( propertyName ) );
        }

        private static string GetEffectTitle( SerializedProperty element, int index )
        {
            string effectName = "Missing Effect";
            string uid = string.Empty;

            if ( element.managedReferenceValue is EffectSettings effectSettings )
            {
                effectName = ObjectNames.NicifyVariableName( effectSettings.GetType().Name.Replace( "EffectSettings", string.Empty ) );

                if ( !string.IsNullOrWhiteSpace( effectSettings.UID ) )
                    uid = $" {effectSettings.UID}";
            }

            return $"{index + 1}. {effectName}{uid}";
        }
    }
}
