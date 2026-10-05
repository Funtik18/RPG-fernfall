using Game.Core.Systems.SheetSystem;
using Game.Core.Gameplay.TBS;
using Modules.Extensions;
using SoosvetGames.CommonTools;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Modules.Editor
{
    public static class EffectsImport
    {
        public static void Import( GoogleSheetTable table )
        {
            var effectsByUid = LoadEffectsByUid();
            List< string > completed = new();

            BattleFuryConfig furyConfig = AssetDatabaseUtils.LoadAsset< BattleFuryConfig >();
            var furySO = new SerializedObject( furyConfig );

            BattleAllySupportConfig allySupportConfig = AssetDatabaseUtils.LoadAsset< BattleAllySupportConfig >();
            var allySupportSO = new SerializedObject( allySupportConfig );

            SerializedProperty furyEffects = furySO.FindBackingField( nameof( BattleFuryConfig.Effects ) ).FindBackingFieldRelative( nameof( EffectsSettings.Effects ) );
            SerializedProperty allySupportEffect = allySupportSO.FindBackingField( nameof( BattleAllySupportConfig.EffectSettings ) );

            foreach ( GoogleSheetRow row in table.Rows )
            {
                var uid = row.Get( "UID" ).Trim();

                if ( string.IsNullOrWhiteSpace( uid ) )
                {
                    Debug.LogError( "[EffectsImport] Row skipped because UID is empty." );
                    continue;
                }

                var updated = false;

                if ( effectsByUid.TryGetValue( uid, out EffectConfig effectConfig ) )
                {
                    UpdateConfig( effectConfig, row );
                    completed.Add( uid );
                    updated = true;
                }

                if ( TrySetEffectInList( furyEffects, uid, row ) )
                {
                    completed.Add( uid );
                    updated = true;
                }

                if ( TrySetEffect( allySupportEffect, uid, row ) )
                {
                    completed.Add( uid );
                    updated = true;
                }

                if ( updated )
                    continue;

                Debug.LogError( $"[EffectsImport] Effect settings with UID '{uid}' was not found." );
            }

            foreach ( var uid in effectsByUid.Keys )
            {
                if( completed.Contains( uid ) ) continue;
                Debug.LogError( $"[EffectsImport] Config with UID '{uid}' was not found in the table." );
            }

            furySO.ApplyModifiedProperties();
            allySupportSO.ApplyModifiedProperties();

            EditorUtility.SetDirty( furyConfig );
            EditorUtility.SetDirty( allySupportConfig );

            AssetDatabase.SaveAssets();
        }

        private static Dictionary< string, EffectConfig > LoadEffectsByUid()
        {
            var result = new Dictionary< string, EffectConfig >();
            var configs = AssetDatabaseUtils.LoadAssets< EffectConfig >();

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

        private static void UpdateConfig( EffectConfig config, GoogleSheetRow row )
        {
            var serializedObject = new SerializedObject( config );

            serializedObject.SetInt( row.GetInt( "X" ), nameof( EffectConfig.X ) );
            serializedObject.SetInt( row.GetInt( "Y" ), nameof( EffectConfig.Y ) );
            serializedObject.SetInt( row.GetInt( "Z" ), nameof( EffectConfig.Z ) );

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty( config );
        }

        private static bool TrySetEffectInList( SerializedProperty effects, string uid, GoogleSheetRow row )
        {
            if ( effects == null )
                return false;

            for ( int index = 0; index < effects.arraySize; index++ )
            {
                SerializedProperty effect = effects.GetArrayElementAtIndex( index );

                if ( TrySetEffect( effect, uid, row ) )
                    return true;
            }

            return false;
        }

        private static bool TrySetEffect( SerializedProperty effect, string uid, GoogleSheetRow row )
        {
            if ( effect == null )
                return false;

            if ( effect.propertyType == SerializedPropertyType.ManagedReference && effect.managedReferenceValue == null )
                return false;

            SerializedProperty uidProperty = effect.FindBackingFieldRelative( nameof( EffectSettings.UID ) );

            if ( uidProperty == null || uidProperty.stringValue.Trim() != uid )
                return false;

            effect.SetInt( row.GetInt( "X" ), nameof( EffectSettings.X ) );
            effect.SetInt( row.GetInt( "Y" ), nameof( EffectSettings.Y ) );
            effect.SetInt( row.GetInt( "Z" ), nameof( EffectSettings.Z ) );

            return true;
        }
    }
}
