using System.Collections.Generic;
using TurnBasedStrategyFramework.Common.Utilities;
using TurnBasedStrategyFramework.Unity.Cells;
using UnityEditor;
using UnityEngine;

namespace Fire.EditorTools.TestGrid
{
    // TBSF's own RectangularSquareGridGenerator hardcodes Y=0 for every cell (see
    // RectangularSquareGridGenerator.GenerateGrid) - it has no concept of terrain height.
    // This is project-side glue, not a framework fork: for each grid coordinate we raycast
    // down against the Ground layer to find the real terrain height, and skip coordinates
    // with no ground underneath (a gap/water column simply gets no Cell - TBSF's own
    // GetCellAt/GetNeighbours already treat a missing coordinate as impassable, so a "hole"
    // in the grid needs no framework changes at all).
    public static class HeightAwareGridBuilder
    {
        public static GameObject Build(
            GameObject squareCellPrefab,
            Transform cellsParent,
            IEnumerable<Vector2Int> coordinates,
            float cellSize,
            float raycastOriginHeight,
            LayerMask groundMask)
        {
            var cellDimensions = squareCellPrefab.GetComponent<Cell>().CellDimensions;
            int placed = 0, skipped = 0;

            foreach (var coord in coordinates)
            {
                var x = (coord.x + 0.5f) * cellSize;
                var z = (coord.y + 0.5f) * cellSize;
                var origin = new Vector3(x, raycastOriginHeight, z);

                if (!Physics.Raycast(origin, Vector3.down, out var hit, raycastOriginHeight * 2f, groundMask))
                {
                    skipped++;
                    continue;
                }

                var instance = (GameObject)PrefabUtility.InstantiatePrefab(squareCellPrefab, cellsParent);
                instance.transform.position = new Vector3(x, hit.point.y, z);
                var cell = instance.GetComponent<Cell>();
                cell.GridCoordinates = new Vector2IntImpl(coord.x, coord.y);
                cell.MovementCost = 1;
                instance.name = $"{squareCellPrefab.name}_({coord.x}, {coord.y})";
                placed++;
            }

            Debug.Log($"HeightAwareGridBuilder: placed {placed} cells, skipped {skipped} (no ground found - gap).");
            return cellsParent.gameObject;
        }

        public static IEnumerable<Vector2Int> Rectangle(int width, int height)
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    yield return new Vector2Int(x, y);
        }
    }
}
