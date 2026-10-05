using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Units;
using Fire.Gameplay.TestGrid;
using Fire.Tests.TestSupport;

namespace Fire.Tests.Gameplay.TestGrid
{
    // AttackZoneCalculator is pure logic (a HashSet<ICell> computation over ICell/IUnit interfaces,
    // no MonoBehaviour/scene dependency), which is exactly why it's covered here instead of only
    // via Play Mode screenshots - see the "tests cover logic, not render" rule in CLAUDE.md.
    //
    // Rather than reconstructing Test_SyntyOnGrid's actual 50-cell bridge scene from memory (its
    // exact cell layout isn't guaranteed to be transcribed correctly, and it depends on scene-only
    // gap geometry), these tests use a small, fully-specified synthetic grid where every expected
    // number below is hand-derivable from plain Manhattan-distance arithmetic and independently
    // verifiable by inspection - a stale "39 cells, trust me" magic number from a past session is
    // weaker regression coverage than a scenario a reader can re-derive in the review.
    public class AttackZoneCalculatorTests
    {
        static List<ICell> BuildGrid(int size)
        {
            var cells = new List<ICell>();
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    cells.Add(new FakeCell(x, y));
            return cells;
        }

        static ICell At(IEnumerable<ICell> cells, int x, int y) =>
            cells.Single(c => c.GridCoordinates.x == x && c.GridCoordinates.y == y);

        // 5x5 grid, unit at the center (2,2), AttackRange = 1. "Reachable" is fixed directly to the
        // Manhattan distance {1,2} ring around the center (12 cells) - standing in for whatever a
        // real movement-range calculation would produce, since ComputeAttackZone only ever consumes
        // the reachable set, never computes it.
        //
        // Hand-derivation: the union of every source's AttackRange=1 ball, where sources are
        // reachable (distance {1,2} from center) plus the unit's own cell (distance 0), is exactly
        // the distance<=3 diamond around the center (Minkowski sum of an L1 ball radius 2 and radius
        // 1 is an L1 ball radius 3). Within a 0..4 square, distance<=3 excludes only the 4 corners
        // (each at distance 4), giving a raw zone of 25 - 4 = 21 cells, including the unit's own
        // cell. After AttackZoneCalculator's self-cell exclusion, zone = 20. zone minus reachable(12)
        // leaves exactly the 8 cells at distance == 3 (the ones at distance {1,2} are reachable
        // itself; distance 0 is the now-excluded self-cell) - enumerated explicitly below.
        static (IUnit unit, List<ICell> allCells, HashSet<ICell> reachable) BuildReferenceScenario()
        {
            var allCells = BuildGrid(5);
            var center = At(allCells, 2, 2);
            var unit = new FakeUnit { CurrentCell = center, AttackRange = 1 };

            var reachableCoords = new (int x, int y)[]
            {
                (1,2),(3,2),(2,1),(2,3),                                   // distance 1
                (0,2),(4,2),(2,0),(2,4),(1,1),(1,3),(3,1),(3,3),           // distance 2
            };
            var reachable = new HashSet<ICell>(reachableCoords.Select(c => At(allCells, c.x, c.y)));

            return (unit, allCells, reachable);
        }

        [Test]
        public void ComputeAttackZone_ReferenceScenario_ReturnsExpectedZoneSize()
        {
            var (unit, allCells, reachable) = BuildReferenceScenario();

            var zone = AttackZoneCalculator.ComputeAttackZone(unit, allCells, reachable);

            Assert.AreEqual(20, zone.Count, "5x5 grid minus 4 corners minus the excluded self-cell = 20.");
        }

        [Test]
        public void ComputeAttackZone_ReferenceScenario_AttackOnlySetHasExpectedCoordinates()
        {
            var (unit, allCells, reachable) = BuildReferenceScenario();

            var zone = AttackZoneCalculator.ComputeAttackZone(unit, allCells, reachable);
            var attackOnly = new HashSet<ICell>(zone);
            attackOnly.ExceptWith(reachable); // mirrors AttackZoneHighlightController.OnUnitSelected exactly

            var expectedCoords = new (int x, int y)[] { (1, 0), (1, 4), (3, 0), (3, 4), (0, 1), (0, 3), (4, 1), (4, 3) };
            var actualCoords = attackOnly.Select(c => (c.GridCoordinates.x, c.GridCoordinates.y)).OrderBy(t => t.Item1).ThenBy(t => t.Item2);

            Assert.AreEqual(8, attackOnly.Count);
            CollectionAssert.AreEquivalent(expectedCoords, actualCoords);
        }

        // The regression guard requested for the "purple cell" investigation: if a future change to
        // the controller's subtraction (e.g. ExceptWith swapped for UnionWith, or dropped entirely)
        // ever let the attack-only set re-overlap the reachable set, this fails. Historical note:
        // the actual "purple cells" symptom investigated in this project was NOT caused by this
        // intersection being non-empty - it was confirmed (by disabling each render layer live in
        // Play Mode) to be alpha-blending the reachable fill over a warm-toned bridge material, a
        // rendering effect with zero data overlap. This test protects the invariant the fix
        // discussion was about, not a re-creation of that specific bug.
        [Test]
        public void AttackOnlySet_NeverOverlapsReachableSet()
        {
            var (unit, allCells, reachable) = BuildReferenceScenario();

            var zone = AttackZoneCalculator.ComputeAttackZone(unit, allCells, reachable);
            var attackOnly = new HashSet<ICell>(zone);
            attackOnly.ExceptWith(reachable);

            Assert.IsEmpty(attackOnly.Intersect(reachable));
        }

        // The confirmed root cause of the "unit's own cell painted red" bug: the unit's current cell
        // is a valid distance-0 *source* (staying put is a legitimate attack origin) but was never
        // stripped back out of the *result*, and since GetAvailableDestinations already excludes a
        // unit's own occupied cell from "reachable", the later "zone minus reachable" subtraction
        // could never remove it either. AttackRange = 0 isolates the effect precisely: with the fix,
        // a 0-range unit's attack zone is exactly its reachable set (nothing can hit anything but the
        // cell it's already standing on); without the fix it would be reachable-plus-self.
        [Test]
        public void ComputeAttackZone_ExcludesUnitsOwnCell()
        {
            var allCells = BuildGrid(3);
            var center = At(allCells, 1, 1);
            var unit = new FakeUnit { CurrentCell = center, AttackRange = 0 };
            var reachable = new HashSet<ICell> { At(allCells, 0, 1), At(allCells, 2, 1), At(allCells, 1, 0), At(allCells, 1, 2) };

            var zone = AttackZoneCalculator.ComputeAttackZone(unit, allCells, reachable);

            Assert.IsFalse(zone.Contains(center), "The unit's own cell must never be a member of its own attack zone.");
            CollectionAssert.AreEquivalent(reachable, zone, "With AttackRange 0, the zone must be exactly the reachable set once the self-cell is excluded.");
        }

        [Test]
        public void ComputeAttackZone_EmptyReachableAndNullCurrentCell_ReturnsOnlyCandidatesWithinRangeOfNothing()
        {
            // Defensive edge case: no reachable cells and no current cell (e.g. a unit that hasn't
            // been placed on the grid yet) must not throw and must produce an empty zone, not a
            // NullReferenceException from the CurrentCell-as-source branch.
            var allCells = BuildGrid(3);
            var unit = new FakeUnit { CurrentCell = null, AttackRange = 5 };

            var zone = AttackZoneCalculator.ComputeAttackZone(unit, allCells, Enumerable.Empty<ICell>());

            Assert.IsEmpty(zone);
        }
    }
}
