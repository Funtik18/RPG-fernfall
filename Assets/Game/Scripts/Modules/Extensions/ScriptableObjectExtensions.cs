#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Modules.Extensions
{
    public static class ScriptableObjectExtensions
    {
        public static void RenameConfigAsset( this ScriptableObject config, string name )
        {
            if ( string.IsNullOrWhiteSpace( name ) )
            {
                Debug.LogError( $"Config was not renamed because name is empty." );
                return;
            }

            var assetName = SanitizeAssetName( name );
            if ( string.IsNullOrWhiteSpace( assetName ) )
            {
                Debug.LogError( $"Config was not renamed because name contains only invalid file name characters." );
                return;
            }

            var path = AssetDatabase.GetAssetPath( config );
            if ( string.IsNullOrWhiteSpace( path ) )
            {
                Debug.LogError( $"Config was not renamed because asset path was not found." );
                return;
            }

            var currentName = System.IO.Path.GetFileNameWithoutExtension( path );
            if ( string.Equals( currentName, assetName, System.StringComparison.InvariantCulture ) )
            {
                config.name = assetName;
                return;
            }

            var directory = System.IO.Path.GetDirectoryName( path )?.Replace( '\\', '/' );
            if ( string.IsNullOrWhiteSpace( directory ) )
            {
                Debug.LogError( $"Config was not renamed because asset directory was not found." );
                return;
            }

            var targetPath = $"{directory}/{assetName}.asset";
            var uniqueTargetPath = AssetDatabase.GenerateUniqueAssetPath( targetPath );

            if ( !string.Equals( targetPath, uniqueTargetPath, System.StringComparison.InvariantCulture ) )
            {
                Debug.LogWarning( $"Asset name '{assetName}' is already used. Config will be renamed to '{System.IO.Path.GetFileNameWithoutExtension( uniqueTargetPath )}'." );
            }

            var error = AssetDatabase.MoveAsset( path, uniqueTargetPath );
            if ( !string.IsNullOrEmpty( error ) )
            {
                Debug.LogError( $"Failed to rename Config from '{path}' to '{uniqueTargetPath}'. {error}" );
                return;
            }

            config.name = System.IO.Path.GetFileNameWithoutExtension( uniqueTargetPath );
        }
        
        private static string SanitizeAssetName( string name )
        {
            var assetName = name.Trim();

            foreach ( var invalidChar in System.IO.Path.GetInvalidFileNameChars() )
            {
                assetName = assetName.Replace( invalidChar, '_' );
            }

            return assetName;
        }
    }
}
#endif