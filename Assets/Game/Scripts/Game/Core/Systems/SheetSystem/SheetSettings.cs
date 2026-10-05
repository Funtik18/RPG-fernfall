using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ System.Serializable ]
    public sealed class SheetSettings
    {
        [ field: SerializeField ] public Information Information { get; private set; }
        [ field: SerializeField ] public FractionConfig Fraction { get; private set; }
        [ field: SerializeField ] public ClassConfig Class { get; private set; }
        [ field: SerializeField ] public InventorySettings Inventory { get; private set; }
    }
}