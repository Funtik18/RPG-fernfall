using Game.Core.Systems.SheetSystem;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ System.Serializable ]
    public sealed class GridCellEffect
    {
        [ field: SerializeField ] public GridCellObject Cell { get; private set; }
        [ field: SerializeField ] public CellEffectConfig Effect { get; private set; }

        public void SetCell( GridCellObject cell )
        {
            Cell = cell;
        }
    }
}
