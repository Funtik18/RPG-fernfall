using UnityEngine;
using Game.Core.Gameplay.TBS;

namespace Game.Core.Gameplay
{
    [ CreateAssetMenu( fileName = "TBSGameplayConfig", menuName = "Game/TBSGameplayConfig" ) ]
    public sealed class TBSGameplayConfig : ScriptableObject
    {
        [ field: SerializeField ] public LineRenderer LineRendererPrefab { get; private set; }
        [ field: Sirenix.OdinInspector.SuffixLabel( "%" ) ]
        [ field: SerializeField ] public float EnemyRetreatChance { get; private set; } = 25f;
        [ field: SerializeField ] public int FollowUpSpeedDifference { get; private set; } = 5;
        [ field: Sirenix.OdinInspector.SuffixLabel( "%" ) ]
        [ field: SerializeField ] public float SellRatio { get; private set; } = 50f;
        [ field: SerializeField ] public BattleAllySupportConfig AllySupport { get; private set; }
        [ field: SerializeField ] public BattleFuryConfig Fury { get; private set; }
        [ field: SerializeField ] public BattleFearConfig Fear { get; private set; }
    }
}
