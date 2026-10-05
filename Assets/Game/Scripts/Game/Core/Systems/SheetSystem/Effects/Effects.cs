using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(Effects), menuName = "Game/Sheet/Effects/" + nameof(Effects) ) ]
    public sealed class Effects : ScriptableObject
    {
        [ SerializeField ] private List< EffectConfig > _effects = new();

        public IReadOnlyList< EffectConfig > EffectConfigs => _effects;
    }
}
