using Game.Core.Gameplay;
using Game.Core.Gameplay.TBS;
using Modules.Extensions;
using SoosvetGames.CommonTools;
using UnityEditor;

namespace Modules.Editor
{
    public static class ConstantsImporter
    {
        public static void Import( GoogleSheetTable table )
        {
            var tbsConfig = AssetDatabaseUtils.LoadAsset< TBSGameplayConfig >();
            var tbsConfigSO = new SerializedObject( tbsConfig );

            tbsConfigSO.SetFloat( table.Rows[ 0 ].GetFloat( "Value" ) * 100f, nameof(TBSGameplayConfig.EnemyRetreatChance) );
            tbsConfigSO.SetInt( table.Rows[ 1 ].GetInt( "Value" ), nameof(TBSGameplayConfig.FollowUpSpeedDifference) );
            tbsConfigSO.SetFloat( table.Rows[ 4 ].GetFloat( "Value" ) * 100f, nameof(TBSGameplayConfig.SellRatio) );
            
            tbsConfigSO.ApplyModifiedProperties();
            EditorUtility.SetDirty( tbsConfig );
        }
    }
}
