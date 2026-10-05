using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class GridCell
    {
        public GridPosition Position { get; }

        public Vector3 WorldPosition { get; }

        public int MovementCost { get; }

        public bool IsWalkable { get; }

        public GridCell(
            GridPosition position,
            Vector3 worldPosition,
            int movementCost,
            bool isWalkable
            )
        {
            Position = position;
            WorldPosition = worldPosition;
            MovementCost = movementCost;
            IsWalkable = isWalkable;
        }
    }
}