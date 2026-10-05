using Game.Core.Systems.SheetSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay
{
    [ CreateAssetMenu( fileName = "PartyGameplayConfig", menuName = "Game/PartyGameplayConfig" ) ]
    public sealed class PartyGameplayConfig : ScriptableObject
    {
        [ field: SerializeField ] public List< SheetSettings > Sheets { get; private set; } = new();
    }
}