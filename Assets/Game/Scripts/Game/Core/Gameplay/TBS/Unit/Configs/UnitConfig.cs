using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public class UnitConfig : ScriptableObject
    {
        [ field: SerializeField ] public string UID { get; private set; }
        [ field: SerializeField ] public GridCellSpawnGroup SpawnGroup { get; private set; }
        [ field: PropertyOrder( 999 ) ]
        [ field: Space ]
        [ field: SerializeField ] public UnitObject Prefab { get; private set; }
    }
}