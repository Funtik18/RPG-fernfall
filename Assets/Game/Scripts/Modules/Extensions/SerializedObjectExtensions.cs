#if UNITY_EDITOR
using System;
using UnityEditor;

namespace Modules.Extensions
{
    public static class SerializedObjectExtensions
    {
        public static void SetString( this SerializedObject serializedObject, string value, params string[] propertyPaths )
        {
            var property = serializedObject.FindBackingField( propertyPaths );
            property.stringValue = value;
        }
        
        public static void SetBool( this SerializedObject serializedObject, bool value, params string[] propertyPaths )
        {
            var property = serializedObject.FindBackingField( propertyPaths );
            property.boolValue = value;
        }

        public static void SetInt( this SerializedObject serializedObject, int value, params string[] propertyPaths )
        {
            var property = serializedObject.FindBackingField( propertyPaths );
            property.intValue = value;
        }
        
        public static void SetFloat( this SerializedObject serializedObject, float value, params string[] propertyPaths )
        {
            var property = serializedObject.FindBackingField( propertyPaths );
            property.floatValue = value;
        }

        public static void SetEnum( this SerializedObject serializedObject, Enum value, params string[] propertyPaths )
        {
            var property = serializedObject.FindBackingField( propertyPaths );
            var path = AssetDatabase.GetAssetPath( serializedObject.targetObject );
            SetEnumValue( property, value, $" in '{path}'" );
        }

        public static void SetObject( this SerializedObject serializedObject, UnityEngine.Object value, params string[] propertyPaths )
        {
            var property = serializedObject.FindBackingField( propertyPaths );
            property.objectReferenceValue = value;
        }

        public static void SetString( this SerializedProperty serializedProperty, string value, params string[] propertyPaths )
        {
            var property = serializedProperty.FindBackingFieldRelative( propertyPaths );
            property.stringValue = value;
        }

        public static void SetInt( this SerializedProperty serializedProperty, int value, params string[] propertyPaths )
        {
            var property = serializedProperty.FindBackingFieldRelative( propertyPaths );
            property.intValue = value;
        }

        public static void SetFloat( this SerializedProperty serializedProperty, float value, params string[] propertyPaths )
        {
            var property = serializedProperty.FindBackingFieldRelative( propertyPaths );
            property.floatValue = value;
        }

        public static void SetEnum( this SerializedProperty serializedProperty, Enum value, params string[] propertyPaths )
        {
            var property = serializedProperty.FindBackingFieldRelative( propertyPaths );
            SetEnumValue( property, value );
        }

        public static void SetObject( this SerializedProperty serializedProperty, UnityEngine.Object value, params string[] propertyPaths )
        {
            var property = serializedProperty.FindBackingFieldRelative( propertyPaths );
            property.objectReferenceValue = value;
        }

        public static SerializedProperty FindBackingField( this SerializedObject serializedObject, params string[] propertyPaths )
        {
            foreach ( var propertyPath in propertyPaths )
            {
                var property = serializedObject.FindProperty( ToBackingFieldPath( propertyPath ) );
                if ( property != null ) return property;
            }

            var path = AssetDatabase.GetAssetPath( serializedObject.targetObject );
            throw new MissingFieldException( $"Serialized property '{string.Join( "', '", propertyPaths )}' was not found in '{path}'." );
        }

        public static SerializedProperty FindBackingFieldRelative( this SerializedProperty serializedProperty, params string[] propertyPaths )
        {
            foreach ( var propertyPath in propertyPaths )
            {
                var property = serializedProperty.FindPropertyRelative( ToBackingFieldPath( propertyPath ) );
                if ( property != null ) return property;
            }

            throw new MissingFieldException( $"Serialized property '{string.Join( "', '", propertyPaths )}' was not found in '{serializedProperty.propertyPath}'." );
        }

        private static void SetEnumValue( SerializedProperty property, Enum value, string context = "" )
        {
            var enumIndex = Array.IndexOf( property.enumNames, value.ToString() );
            if ( enumIndex < 0 )
            {
                throw new ArgumentException( $"Enum value '{value}' was not found in serialized property '{property.propertyPath}'{context}." );
            }

            property.enumValueIndex = enumIndex;
        }

        private static string ToBackingFieldPath( string propertyPath )
        {
            var names = propertyPath.Split( '.' );

            for ( var i = 0; i < names.Length; i++ )
            {
                names[ i ] = $"<{names[ i ]}>k__BackingField";
            }

            return string.Join( ".", names );
        }
    }
}
#endif
