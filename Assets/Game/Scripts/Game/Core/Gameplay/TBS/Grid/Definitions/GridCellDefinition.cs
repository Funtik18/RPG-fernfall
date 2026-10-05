using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ System.Serializable ]
    public sealed class GridCellDefinition
    {
        [ field: SerializeField ] public GridCellObject Cell { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public UnitConfig Unit { get; private set; }
        [ field: SerializeField ] public UnitTransform UnitTransform { get; private set; }
        
        public void SetCell( GridCellObject cell )
        {
            Cell = cell;
        }
        
        public void SetCharacter( UnitConfig config )
        {
            Unit = config;
        }
    }
}