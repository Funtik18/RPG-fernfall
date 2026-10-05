using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = "Item", menuName = "Game/Sheet/Inventory/WeaponFamily" ) ]
    public sealed class WeaponFamilyConfig : ScriptableObject
    {
        [ field: SerializeField ] public string Name { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public List< CombatArtConfig > CombatArts { get; private set; } = new();
    }
}