using Game.Core.Gameplay.TBS;
using Modules.Extensions;
using SoosvetGames.CommonTools;
using UnityEditor;

namespace Modules.Editor
{
    public static class FuryImporter
    {
        public static void Import( GoogleSheetTable table )
        {
            BattleFuryConfig config = AssetDatabaseUtils.LoadAsset< BattleFuryConfig >();
            var serializedObject = new SerializedObject( config );
            
            serializedObject.SetInt( table.Rows[ 0 ].GetInt( "Fury" ), nameof(BattleFuryConfig.AllyDyingNear) );
            serializedObject.SetInt( table.Rows[ 0 ].GetInt( "X" ), nameof(BattleFuryConfig.AllyDyingNearRange) );
            serializedObject.SetInt( table.Rows[ 1 ].GetInt( "Fury" ), nameof(BattleFuryConfig.AllyDyingFar) );
            serializedObject.SetInt( table.Rows[ 1 ].GetInt( "X" ), nameof(BattleFuryConfig.AllyDyingFarRange) );
            serializedObject.SetInt( table.Rows[ 2 ].GetInt( "Fury" ), nameof(BattleFuryConfig.LowHealth) );
            serializedObject.SetFloat( table.Rows[ 2 ].GetFloat( "X" ), nameof(BattleFuryConfig.LowHealthPercent) );
            serializedObject.SetInt( table.Rows[ 3 ].GetInt( "Fury" ), nameof(BattleFuryConfig.KillEnemy) );
            serializedObject.SetFloat( table.Rows[ 4 ].GetFloat( "Fury" ) * 100f, nameof(BattleFuryConfig.FuryEndChance) );
            serializedObject.SetInt( table.Rows[ 5 ].GetInt( "Fury" ), nameof(BattleFuryConfig.RageThreshold) );
            serializedObject.SetInt( table.Rows[ 6 ].GetInt( "Fury" ), nameof(BattleFuryConfig.FuryThreshold) );
            
            serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty( config );
        }
    }
}