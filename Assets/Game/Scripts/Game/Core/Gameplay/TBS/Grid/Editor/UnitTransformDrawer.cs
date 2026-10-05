using UnityEditor;
using UnityEngine;

namespace Game.Core.Gameplay.TBS.Editor
{
    [ CustomPropertyDrawer( typeof(UnitTransform) ) ]
    public sealed class UnitTransformDrawer : PropertyDrawer
    {
        private const float ButtonHeight = 24.0f;
        private const float Spacing = 2.0f;

        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            EditorGUI.BeginProperty( position, label, property );

            var forwardProperty = property.FindPropertyRelative( "_forward" );
            var line = new Rect( position.x, position.y, position.width, EditorGUIUtility.singleLineHeight );

            EditorGUI.PropertyField( line, forwardProperty );

            line.y += EditorGUIUtility.singleLineHeight + Spacing;

            var buttonWidth = ( position.width - Spacing * 3.0f ) / 4.0f;
            if ( GUI.Button( new Rect( line.x, line.y, buttonWidth, ButtonHeight ), "Left" ) )
            {
                forwardProperty.vector3Value = Vector3.left;
            }
            if ( GUI.Button( new Rect( line.x + ( buttonWidth + Spacing ) * 2.0f, line.y, buttonWidth, ButtonHeight ), "Forward" ) )
            {
                forwardProperty.vector3Value = Vector3.forward;
            }
            if ( GUI.Button( new Rect( line.x + ( buttonWidth + Spacing ) * 3.0f, line.y, buttonWidth, ButtonHeight ), "Back" ) )
            {
                forwardProperty.vector3Value = Vector3.back;
            }
            if ( GUI.Button( new Rect( line.x + ( buttonWidth + Spacing ), line.y, buttonWidth, ButtonHeight ), "Right" ) )
            {
                forwardProperty.vector3Value = Vector3.right;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
        {
            return EditorGUIUtility.singleLineHeight + Spacing + ButtonHeight;
        }
    }
}