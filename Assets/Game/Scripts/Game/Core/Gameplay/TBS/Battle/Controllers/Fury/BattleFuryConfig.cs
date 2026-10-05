using Game.Core.Systems.SheetSystem;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ CreateAssetMenu( fileName = "BattleFuryConfig", menuName = "Game/TBS/Battle/Fury Config" ) ]
    public sealed class BattleFuryConfig : ScriptableObject
    {
        [ field: Header( "Rage" ) ]
        [ field: SerializeField, Range( 0f, 100f ) ] public int RageThreshold { get; private set; } = 50;
        [ field: SerializeField, Range( 0f, 100f ) ] public int FuryThreshold { get; private set; } = 100;
        [ field: Header( "Triggers" ) ]
        [ field: SerializeField ] public int AllyDyingNear { get; private set; } = 40;
        [ field: SerializeField ] public int AllyDyingNearRange { get; private set; } = 3;
        [ field: SerializeField ] public int AllyDyingFar { get; private set; } = 15;
        [ field: SerializeField ] public int AllyDyingFarRange { get; private set; } = 4;
        [ field: SerializeField ] public int LowHealth { get; private set; } = 25;
        [ field: Sirenix.OdinInspector.SuffixLabel( "%" ) ]
        [ field: SerializeField, Range( 0f, 100f ) ] public float LowHealthPercent { get; private set; } = 25f;
        [ field: SerializeField ] public int KillEnemy { get; private set; } = 10;
        [ field: Sirenix.OdinInspector.SuffixLabel( "%" ) ]
        [ field: SerializeField, Range( 0f, 100f ) ] public float FuryEndChance { get; private set; } = 40f;
        [ field: SerializeField ] public EffectsSettings Effects { get; private set; } = new();
    }
}
