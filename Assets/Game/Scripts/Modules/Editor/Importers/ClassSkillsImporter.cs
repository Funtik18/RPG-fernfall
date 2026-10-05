using Game.Core.Systems.SheetSystem;
using Modules.Extensions;
using SoosvetGames.CommonTools;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Modules.Editor
{
    public static class ClassSkillsImporter
    {
        public static void Import( GoogleSheetTable table )
        {
            var classesByUid = LoadSkillsByUid();
            List< string > completed = new();
            foreach ( var row in table.Rows )
            {
                var uid = row.Get( "UID" ).Trim();
                if ( string.IsNullOrWhiteSpace( uid ) )
                {
                    Debug.LogError( "[SkillsImporter] Row skipped because UID is empty." );
                    continue;
                }

                if ( !classesByUid.TryGetValue( uid, out _ ) )
                {
                    Debug.LogError( $"[SkillsImporter] Config with UID '{uid}' was not found." );
                    continue;
                }

                UpdateConfig( classesByUid[ uid ], row );
                completed.Add( uid );
            }

            foreach ( var uid in classesByUid.Keys )
            {
                if( completed.Contains( uid ) ) continue;
                Debug.LogError( $"[SkillsImporter] Config with UID '{uid}' was not found in the table." );
            }

            AssetDatabase.SaveAssets();
        }
        
        private static Dictionary< string, SkillConfig > LoadSkillsByUid()
        {
            var result = new Dictionary< string, SkillConfig >();
            var configs = AssetDatabaseUtils.LoadAssets< SkillConfig >();

            foreach ( var config in configs )
            {
                if ( config == null || string.IsNullOrWhiteSpace( config.UID ) ) continue;

                var uid = config.UID.Trim();
                if ( !result.TryGetValue( uid, out _ ) )
                {
                    result.Add( uid, config );
                }
            }

            return result;
        }
        
        private static void UpdateConfig( SkillConfig config, GoogleSheetRow row )
        {
            var serializedObject = new SerializedObject( config );

            var name = row.Get( "Name" ).Trim();
            var x = row.GetInt( "X" );
            var y = row.GetInt( "Y" );
            var z = row.GetInt( "Z" );

            serializedObject.SetString( name, nameof( SkillConfig.Name ) );
            serializedObject.SetInt( x, nameof( SkillConfig.X ) );
            serializedObject.SetInt( y, nameof( SkillConfig.Y ) );
            serializedObject.SetInt( z, nameof( SkillConfig.Z ) );
            
            serializedObject.ApplyModifiedProperties();

            config.RenameConfigAsset( name );
            EditorUtility.SetDirty( config );
        }
    }
}