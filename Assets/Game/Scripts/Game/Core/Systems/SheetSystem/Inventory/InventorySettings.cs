using Game.Core.Systems.SheetSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ System.Serializable ]
    public sealed class InventorySettings
    {
        [ field: SerializeField ] public List< ItemConfig > Items { get; private set; } = new();
    }
}