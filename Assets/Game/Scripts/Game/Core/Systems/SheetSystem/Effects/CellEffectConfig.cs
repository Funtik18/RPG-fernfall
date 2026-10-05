using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    public abstract class CellEffectConfig : EffectConfig
    {
        [ field: SerializeField ] public bool RemoveOnCellExit { get; private set; } = true;
    }
}