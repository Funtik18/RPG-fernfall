using System;
using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Utilities;

namespace Fire.Tests.TestSupport
{
    // Minimal ICell test double: only GridCoordinates/GetDistance carry real behaviour (delegated
    // to TBSF's own SquareHelper/CellHelper so distance and equality semantics exactly match what
    // production Cell instances do), everything else is inert. Not a general-purpose ICell fake -
    // just enough surface for logic that only cares about grid position and distance.
    public class FakeCell : ICell
    {
        public event Action<ICell> CellHighlighted;
        public event Action<ICell> CellDehighlighted;
        public event Action<ICell> CellClicked;

        public IVector2Int GridCoordinates { get; set; }
        public bool IsTaken { get; set; }
        public IList<IUnit> CurrentUnits { get; } = new List<IUnit>();
        public float MovementCost { get; set; } = 1f;
        public IVector3 WorldPosition { get; set; }

        public FakeCell(int x, int y)
        {
            GridCoordinates = new Vector2IntImpl(x, y);
        }

        public int GetDistance(ICell otherCell) => SquareHelper.GetDistance(this, otherCell);

        public IEnumerable<ICell> GetNeighbours(ICellManager cellManager) => SquareHelper.GetNeighbours(this, cellManager);

        public void InvokeCellHighlighted() => CellHighlighted?.Invoke(this);
        public void InvokeCellDehighlighted() => CellDehighlighted?.Invoke(this);
        public void InvokeCellClicked() => CellClicked?.Invoke(this);

        public bool Equals(ICell other) => CellHelper.Equals(this, other);
        public override bool Equals(object obj) => CellHelper.Equals(this, obj);
        public override int GetHashCode() => CellHelper.GetHashCode(this);
        public override string ToString() => $"({GridCoordinates.x},{GridCoordinates.y})";
    }
}
