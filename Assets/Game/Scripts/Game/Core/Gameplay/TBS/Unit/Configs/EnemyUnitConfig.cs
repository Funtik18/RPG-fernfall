using Game.Core.Systems.SheetSystem;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ CreateAssetMenu( fileName = "UnitConfig", menuName = "Game/TBS/EnemyUnitConfig" ) ]
    public sealed class EnemyUnitConfig : UnitConfig
    {
        [ field: SerializeField ] public AIBehaviourConfig AIBehaviour { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public SheetSettings SheetSettings { get; private set; }
    }
}
