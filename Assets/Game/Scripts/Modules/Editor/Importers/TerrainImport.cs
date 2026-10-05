using Game.Core.Gameplay.TBS;
using Modules.Extensions;
using SoosvetGames.CommonTools;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Modules.Editor
{
    public static class TerrainImport
    {
        public static void Import( GoogleSheetTable table )
        {
            var terrainsByUid = LoadTerrainsByUid();
            
            List< string > completed = new();
            foreach ( var row in table.Rows )
            {
                var uid = row.Get( "UID" ).Trim();
                if ( string.IsNullOrWhiteSpace( uid ) )
                {
                    Debug.LogError( "[TerrainImport] Row skipped because UID is empty." );
                    continue;
                }

                if ( !terrainsByUid.TryGetValue( uid, out _ ) )
                {
                    Debug.LogError( $"[TerrainImport] Config with UID '{uid}' was not found." );
                    continue;
                }

                UpdateConfig( terrainsByUid[ uid ], row );
                completed.Add( uid );
            }

            foreach ( var uid in terrainsByUid.Keys )
            {
                if( completed.Contains( uid ) ) continue;
                Debug.LogError( $"[TerrainImport] Config with UID '{uid}' was not found in the table." );
            }

            AssetDatabase.SaveAssets();
        }
        
        private static Dictionary< string, GridCellTerrainConfig > LoadTerrainsByUid()
        {
            var result = new Dictionary< string, GridCellTerrainConfig >();
            var configs = AssetDatabaseUtils.LoadAssets< GridCellTerrainConfig >();

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
        
        private static void UpdateConfig( GridCellTerrainConfig config, GoogleSheetRow row )
        {
            var serializedObject = new SerializedObject( config );
            
            var name = row.Get( "Name" ).Trim();
            var avoid = row.GetFloat( "Avoid" ) * 100f;
            var moveCost = row.GetInt( "Move cost" );
            
            serializedObject.SetString( name, nameof( GridCellTerrainConfig.Name ) );
            serializedObject.SetFloat( avoid, nameof( GridCellTerrainConfig.Avoid ) );
            serializedObject.SetInt( moveCost, nameof( GridCellTerrainConfig.MoveCost ) );
            
            serializedObject.ApplyModifiedProperties();

            config.RenameConfigAsset( name );
            EditorUtility.SetDirty( config );
        }
    }
}