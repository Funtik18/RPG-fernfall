using Game.Core.Systems.SheetSystem;
using Modules.Extensions;
using SoosvetGames.CommonTools;
using System;
using UnityEditor;
using UnityEngine;

namespace Modules.Editor
{
    public static class WeaponsTriangleImporter
    {
        public static void Import( GoogleSheetTable table )
        {
            var config = AssetDatabaseUtils.LoadAsset< WeaponRules >();
            var serializedObject = new SerializedObject( config );
            serializedObject.Update();

            var rules = serializedObject.FindBackingField( nameof( WeaponRules.Rules ) );
            rules.ClearArray();

            foreach ( var row in table.Rows )
            {
                var attackerTypeName = row.Get( "Name" ).Trim();
                if ( !Enum.TryParse( attackerTypeName, true, out WeaponType attackerTriangleType ) ||
                     !Enum.IsDefined( typeof( WeaponType ), attackerTriangleType ) )
                {
                    Debug.LogError( $"[WeaponsTriangleImporter] Row skipped because Triangle Type '{attackerTypeName}' is not a valid {nameof( WeaponType )}." );
                    continue;
                }

                var defenderTypeName = row.Get( "Strikes" ).Trim();
                if ( !Enum.TryParse( defenderTypeName, true, out WeaponType defenderTriangleType ) ||
                     !Enum.IsDefined( typeof( WeaponType ), defenderTriangleType ) )
                {
                    Debug.LogError( $"[WeaponsTriangleImporter] Row skipped because Triangle Type '{defenderTypeName}' is not a valid {nameof( WeaponType )}." );
                    continue;
                }

                var ruleIndex = rules.arraySize;
                rules.InsertArrayElementAtIndex( ruleIndex );
                var rule = rules.GetArrayElementAtIndex( ruleIndex );

                rule.SetEnum( attackerTriangleType, nameof(WeaponRule.Attacker) );
                rule.SetEnum( defenderTriangleType, nameof(WeaponRule.Defender) );
                rule.SetInt( row.GetInt( "Accuracy" ), nameof(WeaponRule.Accuracy) );
                rule.SetInt( row.GetInt( "Damage" ), nameof(WeaponRule.Damage) );
            }

            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty( config );
        }
    }
}
