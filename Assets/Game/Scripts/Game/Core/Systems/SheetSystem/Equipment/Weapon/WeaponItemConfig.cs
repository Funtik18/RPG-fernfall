using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = "Item", menuName = "Game/Sheet/Inventory/Weapon" ) ]
    public sealed class WeaponItemConfig : EquipmentItemConfig
    {
        [ field: Space ]
        [ field: SerializeField ] public WeaponTargetType TargetType { get; private set; }
        [ field: SerializeField ] public WeaponFamilyConfig Family { get; private set; }
        [ field: SerializeField ] public WeaponType TriangleType { get; private set; }
        [ field: SerializeField ] public int Might { get; private set; }
        [ field: Sirenix.OdinInspector.SuffixLabel( "%" ) ]
        [ field: SerializeField ] public float Hit { get; private set; }
        [ field: Sirenix.OdinInspector.SuffixLabel( "%" ) ]
        [ field: SerializeField ] public float Critical { get; private set; }
        [ field: SerializeField ] public int Avoid { get; private set; } = 0;//TODO
        [ field: SerializeField ] public int Range { get; private set; } = 1;
        [ field: SerializeField ] public bool IsInfinity { get; private set; }
        [ field: Sirenix.OdinInspector.HideIf( "IsInfinity" ) ]
        [ field: SerializeField ] public int Uses { get; private set; } = 1;
        [ field: Sirenix.OdinInspector.ShowIf( "IsInfinity" ) ]
        [ field: SerializeField ] public int SpecialUses { get; private set; } = 1;
        [ field: Space ]
        [ field: SerializeField ] public List< EffectConfig > OnHitEffects { get; private set; } = new();
    }
}
