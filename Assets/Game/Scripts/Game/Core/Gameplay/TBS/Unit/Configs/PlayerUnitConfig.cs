using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ CreateAssetMenu( fileName = "UnitConfig", menuName = "Game/TBS/PlayerUnitConfig" ) ]
    public sealed class PlayerUnitConfig : UnitConfig
    {
        [ field: Space ]
        [ field: SerializeField ] public int Index { get; private set; }
    }
}