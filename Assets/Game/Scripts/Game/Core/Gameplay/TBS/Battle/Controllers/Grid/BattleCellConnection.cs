using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleCellConnection
    {
        public BattleCell Cell { get; }
        public bool IsGridLink { get; }
        public GridCellTransitionType TransitionType { get; }

        public BattleCellConnection( BattleCell cell )
        {
            Cell = cell ?? throw new ArgumentNullException( nameof(cell) );
        }

        public BattleCellConnection( BattleCell cell, GridCellTransitionType transitionType )
        {
            Cell = cell ?? throw new ArgumentNullException( nameof(cell) );
            IsGridLink = true;
            TransitionType = transitionType;
        }
    }
}
