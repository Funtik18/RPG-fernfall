using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TurnBasedStrategyFramework.Common.Cells;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Cells;
using TurnBasedStrategyFramework.Unity.Highlighters;
using TurnBasedStrategyFramework.Unity.Units;

namespace Fire.Gameplay.TestGrid
{
    // Adds a second, independent highlight zone (red attack-range outline) on top of TBSF's own
    // blue movement-range highlight, without touching Cell/ICellManager at all - it drives the
    // MarkAsAttackZone/UnMarkAttackZone Highlighter components (added to the TestSquareCell prefab
    // alongside, not instead of, the framework's own 4 highlighter slots) directly via Highlighter.Apply(),
    // the same way TacticalCameraController hooks Unit.UnitSelected externally rather than forking
    // TBSF's selection flow.
    public class AttackZoneHighlightController : MonoBehaviour
    {
        public UnityUnitManager UnitManager;
        public UnityCellManager CellManager;

        private readonly HashSet<ICell> _markedAttackCells = new HashSet<ICell>();
        private readonly HashSet<ICell> _markedReachableCells = new HashSet<ICell>();

        void OnEnable()
        {
            if (UnitManager != null)
                UnitManager.UnitAdded += Subscribe;
        }

        void OnDisable()
        {
            if (UnitManager != null)
                UnitManager.UnitAdded -= Subscribe;
        }

        void Subscribe(IUnit unit)
        {
            if (unit is Unit u)
            {
                u.UnitSelected += OnUnitSelected;
                u.UnitDeselected += OnUnitDeselected;
            }
        }

        void OnUnitSelected(IUnit unit)
        {
            // Don't rely on MoveAbilityImpl's own CachePaths timing (GridStateUnitSelected fires
            // UnitSelected before it runs each ability's OnAbilitySelected) - recompute our own.
            unit.CachePaths(CellManager);
            var reachable = new HashSet<ICell>(unit.GetAvailableDestinations(CellManager.GetCells()));
            var fullZone = AttackZoneCalculator.ComputeAttackZone(unit, CellManager.GetCells(), reachable);

            // fullZone is attack range from every reachable cell (plus the unit's own cell) - it
            // overlaps most of the reachable set itself. The FE3H-style two-tone diamond needs the
            // two fills mutually exclusive: reachable cells stay blue (TBSF's own MarkAsReachable),
            // this layer only fills/borders the cells attackable but NOT reachable this turn.
            var attackOnly = new HashSet<ICell>(fullZone);
            attackOnly.ExceptWith(reachable);

            var reachableCoords = ToCoordSet(reachable);
            var attackOnlyCoords = ToCoordSet(attackOnly);

            foreach (var cell in reachable)
            {
                SetVisualActive(cell, "ReachableZoneBorder", true);
                ConfigureBorderEdges(cell, "ReachableZoneBorder", reachableCoords);
                _markedReachableCells.Add(cell);
            }

            foreach (var cell in attackOnly)
            {
                ApplyHighlighter(cell, "MarkAsAttackZone");
                ConfigureBorderEdges(cell, "AttackZoneVisual", attackOnlyCoords);
                _markedAttackCells.Add(cell);
            }
        }

        void OnUnitDeselected(IUnit unit)
        {
            foreach (var cell in _markedAttackCells)
            {
                ApplyHighlighter(cell, "UnMarkAttackZone");
            }
            _markedAttackCells.Clear();

            foreach (var cell in _markedReachableCells)
            {
                SetVisualActive(cell, "ReachableZoneBorder", false);
            }
            _markedReachableCells.Clear();
        }

        static HashSet<(int, int)> ToCoordSet(IEnumerable<ICell> cells) =>
            new HashSet<(int, int)>(cells.Select(c => (c.GridCoordinates.x, c.GridCoordinates.y)));

        static void ApplyHighlighter(ICell cell, string highlighterName)
        {
            if (!(cell is Cell unityCell)) return;
            var highlighterTransform = unityCell.transform.Find("Highlighters/" + highlighterName);
            if (highlighterTransform == null) return;
            var highlighter = highlighterTransform.GetComponent<Highlighter>();
            if (highlighter != null) _ = highlighter.Apply(new NoParam());
        }

        // ReachableZoneBorder has no TBSF Highlighter slot (it's not one of Cell's 4, and it's not
        // wired into AttackZoneVisual's GameObjectActivatorHighlighter either) - toggled directly,
        // same as the border segments below.
        static void SetVisualActive(ICell cell, string visualName, bool active)
        {
            if (!(cell is Cell unityCell)) return;
            var visual = unityCell.transform.Find(visualName);
            if (visual != null) visual.gameObject.SetActive(active);
        }

        // A border segment shows only where the zone actually ends - if the neighbor across that
        // edge is also in the zone, the segment stays off so adjacent zone cells read as one
        // contiguous shape with a contour on its outer edge, not a grid of individually outlined
        // squares. This is purely a visual sub-detail of our own visuals, not a TBSF concept, so
        // it's configured directly rather than through Highlighter.Apply(). Shared by both the
        // attack-only zone (AttackZoneVisual) and the reachable zone (ReachableZoneBorder) - same
        // edge-aware rule, different visual root and different backing coordinate set.
        static void ConfigureBorderEdges(ICell cell, string visualName, HashSet<(int, int)> zoneCoords)
        {
            if (!(cell is Cell unityCell)) return;
            var visual = unityCell.transform.Find(visualName);
            if (visual == null) return;

            int x = cell.GridCoordinates.x, y = cell.GridCoordinates.y;
            SetChildActive(visual, "Border_North", !zoneCoords.Contains((x, y + 1)));
            SetChildActive(visual, "Border_South", !zoneCoords.Contains((x, y - 1)));
            SetChildActive(visual, "Border_East", !zoneCoords.Contains((x + 1, y)));
            SetChildActive(visual, "Border_West", !zoneCoords.Contains((x - 1, y)));
        }

        static void SetChildActive(Transform parent, string childName, bool active)
        {
            var child = parent.Find(childName);
            if (child != null) child.gameObject.SetActive(active);
        }
    }
}
