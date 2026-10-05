using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class GridLinks : MonoBehaviour
    {
        [ field: SerializeField ] public List< GridCellLink > CellLinks { get; private set; } = new();
    }
}