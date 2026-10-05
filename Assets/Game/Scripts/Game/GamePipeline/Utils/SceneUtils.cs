#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Game.Pipeline
{
    public static class SceneUtils
    {
        public static SceneAsset FindScene( string sceneName )
        {
            string[] guids = AssetDatabase.FindAssets( $"{sceneName} t:Scene" );

            string[] matchingPaths = guids
                .Select( AssetDatabase.GUIDToAssetPath )
                .Where( ( path ) => string.Equals( Path.GetFileNameWithoutExtension( path ), sceneName, StringComparison.OrdinalIgnoreCase ) )
                .ToArray();

            if ( matchingPaths.Length == 0 )
            {
                throw new InvalidOperationException( $"Scene '{sceneName}' was not found." );
            }

            if ( matchingPaths.Length > 1 )
            {
                throw new InvalidOperationException( $"Found multiple scenes named '{sceneName}':\n" + string.Join( "\n", matchingPaths ) );
            }

            return AssetDatabase.LoadAssetAtPath< SceneAsset >( matchingPaths[ 0 ] );
        }
    }
}
#endif