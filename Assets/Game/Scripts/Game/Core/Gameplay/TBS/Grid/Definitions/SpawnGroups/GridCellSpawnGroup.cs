using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public abstract class GridCellSpawnGroup : ScriptableObject
    {
#if UNITY_EDITOR
        [ field: SerializeField ] public Color EditorColor { get; private set; } = Color.white;
#endif
    }
}