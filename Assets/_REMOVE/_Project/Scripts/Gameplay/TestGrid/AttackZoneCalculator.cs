using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Units;

namespace Fire.Gameplay.TestGrid
{
    // The red zone is NOT "attack range from the unit's current cell" - it's the union of attack
    // range from every cell the unit could move to this turn (plus its current cell, since staying
    // put is always an option TBSF's own GetAvailableDestinations excludes - that cell is occupied
    // by the unit itself, so IsCellMovableTo/IsTaken filters it out of the move-range set).
    //
    // Per the confirmed Phase 1 finding, TBSF's own attack-range check
    // (CombatComponent.IsUnitAttackable) is a pure ICell.GetDistance comparison - Manhattan distance
    // over GridCoordinates, no line-of-sight, no obstruction, height-agnostic. This mirrors that
    // exact formula rather than calling IsUnitAttackable itself, which is keyed to an actual enemy
    // unit's cell rather than a plain geometric "would anything standing here be in range" query.
    public static class AttackZoneCalculator
    {
        public static HashSet<ICell> ComputeAttackZone(IUnit unit, IEnumerable<ICell> allCells, IEnumerable<ICell> reachableCells)
        {
            var sourceCells = new HashSet<ICell>(reachableCells);
            if (unit.CurrentCell != null)
            {
                sourceCells.Add(unit.CurrentCell);
            }

            var zone = new HashSet<ICell>();
            foreach (var candidate in allCells)
            {
                foreach (var source in sourceCells)
                {
                    if (source.GetDistance(candidate) <= unit.AttackRange)
                    {
                        zone.Add(candidate);
                        break;
                    }
                }
            }

            // The unit's own cell is a valid distance-0 source for range calculation, but it's never
            // a valid attack candidate - without this it always survives the caller's "minus reachable"
            // subtraction (GetAvailableDestinations excludes the unit's own occupied cell from
            // "reachable", so it's never there to be subtracted out) and renders as a false attack target.
            if (unit.CurrentCell != null)
            {
                zone.Remove(unit.CurrentCell);
            }
            return zone;
        }
    }
}
