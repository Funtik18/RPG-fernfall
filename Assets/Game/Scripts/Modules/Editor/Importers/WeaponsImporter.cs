using Game.Core.Systems.SheetSystem;
using Modules.Extensions;
using SoosvetGames.CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Modules.Editor
{
    public static class WeaponsImporter
    {
        public static void Import( GoogleSheetTable table )
        {
            var weaponsFamilies = AssetDatabaseUtils.LoadAssets< WeaponFamilyConfig >();
            var weaponsByUid = LoadWeaponsByUid();
            List< string > completed = new();
            foreach ( var row in table.Rows )
            {
                var uid = row.Get( "UID" ).Trim();
                if ( string.IsNullOrWhiteSpace( uid ) )
                {
                    Debug.LogError( "[WeaponsImporter] Row skipped because UID is empty." );
                    continue;
                }

                if ( !weaponsByUid.TryGetValue( uid, out _ ) )
                {
                    Debug.LogError( $"[WeaponsImporter] Config with UID '{uid}' was not found." );
                    continue;
                }

                UpdateConfig( weaponsByUid[ uid ], weaponsFamilies, row );
                completed.Add( uid );
            }
            
            foreach ( var uid in weaponsByUid.Keys )
            {
                if( completed.Contains( uid ) ) continue;
                Debug.LogError( $"[WeaponsImporter] Config with UID '{uid}' was not found in the table." );
            }

            AssetDatabase.SaveAssets();
        }
        
        private static Dictionary< string, WeaponItemConfig > LoadWeaponsByUid()
        {
            var result = new Dictionary< string, WeaponItemConfig >();
            var configs = AssetDatabaseUtils.LoadAssets< WeaponItemConfig >();

            foreach ( var config in configs )
            {
                result.Add( config.UID, config );
            }

            return result;
        }
        
        private static void UpdateConfig( WeaponItemConfig config, WeaponFamilyConfig[] families, GoogleSheetRow row )
        {
            var serializedObject = new SerializedObject( config );

            var name = row.Get( "Name" ).Trim();

            var targetTypeName = row.Get( "Target" ).Trim();
            if ( !Enum.TryParse( targetTypeName, true, out WeaponTargetType targetType ) ||
                 !Enum.IsDefined( typeof( WeaponTargetType ), targetType ) )
            {
                Debug.LogError( $"[WeaponsImporter] Row skipped because Target '{targetTypeName}' is not a valid {nameof( WeaponTargetType )}." );
                targetType = default;
            }

            var familyTypeName = row.Get( "Family" );
            var family = families.FirstOrDefault( (f) => f.Name == familyTypeName );
            if ( family == null )
            {
                Debug.LogError( $"[WeaponsImporter] Row skipped because Family '{familyTypeName}' is not found." );
                family = families.First( ( f ) => f.Name == "Unarmed" );
            }
            
            var triangleTypeName = row.Get( "Triangle Type" ).Trim();
            if ( !Enum.TryParse( triangleTypeName, true, out WeaponType triangleType ) ||
                 !Enum.IsDefined( typeof( WeaponType ), triangleType ) )
            {
                Debug.LogError( $"[WeaponsImporter] Row skipped because Triangle Type '{triangleTypeName}' is not a valid {nameof( WeaponType )}." );
                triangleType = default;
            }
            
            var might = row.GetInt( "Might" );
            var hit = row.GetFloat( "Hit" ) * 100f;
            var critical = row.GetFloat( "Critical" ) * 100f;
            var avoid = row.GetInt( "Avoid" );
            var range = row.GetInt( "Range" );
            var cost = row.GetInt( "Cost" );
            var weight = row.GetInt( "Weight" );

            serializedObject.SetString( name, nameof(WeaponItemConfig.Name) );
            serializedObject.SetEnum( targetType, nameof(WeaponItemConfig.TargetType) );
            serializedObject.SetObject( family, nameof(WeaponItemConfig.Family) );
            serializedObject.SetEnum( triangleType, nameof(WeaponItemConfig.TriangleType) );
            serializedObject.SetInt( might, nameof(WeaponItemConfig.Might) );
            serializedObject.SetFloat( hit, nameof(WeaponItemConfig.Hit) );
            serializedObject.SetFloat( critical, nameof(WeaponItemConfig.Critical) );
            serializedObject.SetInt( avoid, nameof(WeaponItemConfig.Avoid) );
            serializedObject.SetInt( range, nameof(WeaponItemConfig.Range) );
            serializedObject.SetInt( cost, nameof(WeaponItemConfig.Cost) );
            serializedObject.SetInt( weight, nameof(WeaponItemConfig.Weight) );
            
            var isInfinity = row.Get( "Uses" ) == "infinity";
            serializedObject.SetBool( isInfinity, nameof(WeaponItemConfig.IsInfinity) );            
            serializedObject.SetInt( isInfinity ? -1 : row.GetInt( "Uses" ), nameof(WeaponItemConfig.Uses) );
            serializedObject.SetInt( isInfinity ? row.GetInt( "Special Uses" ) : -1 , nameof(WeaponItemConfig.SpecialUses) );

            serializedObject.ApplyModifiedProperties();

            config.RenameConfigAsset( name );
            EditorUtility.SetDirty( config );
        }
    }
}
