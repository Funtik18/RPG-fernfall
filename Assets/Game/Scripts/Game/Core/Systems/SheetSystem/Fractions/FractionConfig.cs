using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = "FractionConfig", menuName = "Game/Sheet/FractionConfig" ) ]
    public sealed class FractionConfig : ScriptableObject
    {
        [ field: SerializeField ] public string Name { get; private set; }
        [ field: SerializeField ] public bool IsFeelFury { get; private set; }
        [ field: SerializeField ] public bool IsFeelFear { get; private set; }
    }
}