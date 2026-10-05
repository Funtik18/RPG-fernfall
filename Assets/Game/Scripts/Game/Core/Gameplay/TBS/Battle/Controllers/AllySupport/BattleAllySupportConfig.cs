using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ CreateAssetMenu( fileName = "BattleAllySupportConfig", menuName = "Game/TBS/Battle/Ally Support Config" ) ]
    public sealed class BattleAllySupportConfig : ScriptableObject
    {
        [ field: SerializeField ] public AllySupportEffectSettings EffectSettings { get; private set; }
    }
}