using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ System.Serializable ]
    public sealed class GridCellLink
    {
        [ field: SerializeField ] public GridCellObject CellA { get; private set; }
        [ field: SerializeField ] public GridCellObject CellB { get; private set; }
        [ field: SerializeField ] public GridCellTransitionType TransitionType { get; private set; } = GridCellTransitionType.Jump;
    }
}
