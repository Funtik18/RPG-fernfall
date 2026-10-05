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
    public static class CombatArtsImport
    {
        public static void Import( GoogleSheetTable table )
        {
            var artsByUid = LoadArtsByUid();
            var familiesByName = LoadWeaponFamiliesByName();
            List< string > completed = new();
            foreach ( var row in table.Rows )
            {
                var uid = row.Get( "UID" ).Trim();
                if ( string.IsNullOrWhiteSpace( uid ) )
                {
                    Debug.LogError( "[CombatArtsImport] Row skipped because UID is empty." );
                    continue;
                }

                if ( !artsByUid.TryGetValue( uid, out _ ) )
                {
                    Debug.LogError( $"[CombatArtsImport] Config with UID '{uid}' was not found." );
                    continue;
                }

                UpdateConfig( artsByUid[ uid ], familiesByName, row );
                completed.Add( uid );
            }

            foreach ( var uid in artsByUid.Keys )
            {
                if( completed.Contains( uid ) ) continue;
                Debug.LogError( $"[CombatArtsImport] Config with UID '{uid}' was not found in the table." );
            }

            AssetDatabase.SaveAssets();
        }
        
        private static Dictionary< string, CombatArtConfig > LoadArtsByUid()
        {
            var result = new Dictionary< string, CombatArtConfig >();
            var configs = AssetDatabaseUtils.LoadAssets< CombatArtConfig >();

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
        
        private static Dictionary< string, WeaponFamilyConfig > LoadWeaponFamiliesByName()
        {
            var result = new Dictionary< string, WeaponFamilyConfig >();
            var configs = AssetDatabaseUtils.LoadAssets< WeaponFamilyConfig >();

            foreach ( var config in configs )
            {
                if ( config == null || string.IsNullOrWhiteSpace( config.Name ) ) continue;

                var uid = config.Name.Trim();
                if ( !result.TryGetValue( uid, out _ ) )
                {
                    result.Add( uid, config );
                }
            }

            return result;
        }
        
        private static void UpdateConfig( CombatArtConfig config, Dictionary< string, WeaponFamilyConfig > familiesByName, GoogleSheetRow row )
        {
            var serializedObject = new SerializedObject( config );

            var name = row.Get( "Name" ).Trim();
            var damage = row.GetInt( "Damage" );
            var hit = row.GetInt( "Hit" );
            var uses = row.GetInt( "Uses" );
            var followUp = row.GetBool( "Follow Up" );
            var x = row.GetInt( "X" );
            var y = row.GetInt( "Y" );
            var z = row.GetInt( "Z" );
            var familyName = row.Get( "Weapon Family" ).Trim();
            var family = GetWeaponFamily( familiesByName, familyName );
            
            serializedObject.SetString( name, nameof( CombatArtConfig.Name ) );
            serializedObject.SetInt( damage, nameof( CombatArtConfig.Damage ) );
            serializedObject.SetInt( hit, nameof( CombatArtConfig.Hit ) );
            serializedObject.SetInt( uses, nameof( CombatArtConfig.Uses ) );
            serializedObject.SetBool( followUp, nameof( CombatArtConfig.FollowUp ) );
            serializedObject.SetInt( x, nameof( CombatArtConfig.X ) );
            serializedObject.SetInt( y, nameof( CombatArtConfig.Y ) );
            serializedObject.SetInt( z, nameof( CombatArtConfig.Z ) );

            serializedObject.ApplyModifiedProperties();
            SetWeaponFamily( config, familiesByName.Values, family );

            config.RenameConfigAsset( name );
            EditorUtility.SetDirty( config );
        }

        private static WeaponFamilyConfig GetWeaponFamily( Dictionary< string, WeaponFamilyConfig > familiesByName, string familyName )
        {
            if ( familiesByName.TryGetValue( familyName, out var family ) )
            {
                return family;
            }

            Debug.LogError( $"[CombatArtsImport] Weapon Family '{familyName}' is not found." );

            return familiesByName.TryGetValue( "Unarmed", out var fallbackFamily ) ? fallbackFamily : null;
        }

        private static void SetWeaponFamily( CombatArtConfig config, IEnumerable< WeaponFamilyConfig > families, WeaponFamilyConfig targetFamily )
        {
            foreach ( var family in families )
            {
                RemoveCombatArt( family, config );
            }

            if ( targetFamily == null ) return;

            AddCombatArt( targetFamily, config );
        }

        private static void RemoveCombatArt( WeaponFamilyConfig family, CombatArtConfig config )
        {
            var serializedObject = new SerializedObject( family );
            var combatArts = serializedObject.FindBackingField( nameof( WeaponFamilyConfig.CombatArts ) );
            for ( var index = combatArts.arraySize - 1; index >= 0; index-- )
            {
                var combatArt = combatArts.GetArrayElementAtIndex( index );
                if ( combatArt.objectReferenceValue != config ) continue;

                var arraySize = combatArts.arraySize;
                combatArts.DeleteArrayElementAtIndex( index );
                if ( combatArts.arraySize == arraySize )
                {
                    combatArts.DeleteArrayElementAtIndex( index );
                }
            }

            if ( serializedObject.ApplyModifiedProperties() )
            {
                EditorUtility.SetDirty( family );
            }
        }

        private static void AddCombatArt( WeaponFamilyConfig family, CombatArtConfig config )
        {
            var serializedObject = new SerializedObject( family );
            var combatArts = serializedObject.FindBackingField( nameof( WeaponFamilyConfig.CombatArts ) );
            if ( ContainsCombatArt( combatArts, config ) )
            {
                return;
            }

            var index = combatArts.arraySize;
            combatArts.InsertArrayElementAtIndex( index );
            combatArts.GetArrayElementAtIndex( index ).objectReferenceValue = config;

            if ( serializedObject.ApplyModifiedProperties() )
            {
                EditorUtility.SetDirty( family );
            }
        }

        private static bool ContainsCombatArt( SerializedProperty combatArts, CombatArtConfig config )
        {
            return Enumerable.Range( 0, combatArts.arraySize ).Any( index => combatArts.GetArrayElementAtIndex( index ).objectReferenceValue == config );
        }
    }
}
