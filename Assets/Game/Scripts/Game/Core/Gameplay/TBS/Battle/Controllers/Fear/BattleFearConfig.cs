using Game.Core.Systems.SheetSystem;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ CreateAssetMenu( fileName = "BattleFearConfig", menuName = "Game/TBS/Battle/Fear Config" ) ]
    public sealed class BattleFearConfig : ScriptableObject
    {
        [ field: Header( "Fear" ) ]
        [ field: SerializeField, Range( 0f, 100f ) ] public int FearThreshold { get; private set; } = 50;
        [ field: SerializeField, Range( 0f, 100f ) ] public int EscapeThreshold { get; private set; } = 100;
        [ field: Header( "Triggers" ) ]
        [ field: SerializeField ] public int AllyDyingNear { get; private set; } = 40;
        [ field: SerializeField ] public int AllyDyingNearRange { get; private set; } = 3;
        [ field: SerializeField ] public int AllyDyingFar { get; private set; } = 15;
        [ field: SerializeField ] public int AllyDyingFarRange { get; private set; } = 5;
        [ field: SerializeField ] public int EnemyFurious { get; private set; } = 10;
        [ field: SerializeField ] public int BeesDamage { get; private set; } = 15;
        [ field: SerializeField ] public int LowHealth { get; private set; } = 25;
        [ field: Sirenix.OdinInspector.SuffixLabel( "%" ) ]
        [ field: SerializeField, Range( 0f, 100f ) ] public float LowHealthPercent { get; private set; } = 25f;

        [ field: SerializeField ] public EffectsSettings Effects { get; private set; } = new();
    }
}
