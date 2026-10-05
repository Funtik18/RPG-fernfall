using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ CreateAssetMenu( fileName = "GridCellTerrainConfig", menuName = "Game/TBS/GridCellTerrainConfig" ) ]
    public sealed class GridCellTerrainConfig : ScriptableObject
    {
        [ field: SerializeField ] public string UID { get; private set; }
        [ field: SerializeField ] public string Name { get; private set; }
        [ field: Sirenix.OdinInspector.SuffixLabel( "%" ) ]
        [ field: SerializeField ] public float Avoid { get; private set; }
        [ field: SerializeField ] public int MoveCost { get; private set; }
        
#if UNITY_EDITOR
        [ field: SerializeField ] public Color EditorColor { get; private set; } = Color.white;
#endif
    }
}