using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ Serializable ]
    public sealed class EffectsSettings
    {
        [ field: SerializeReference ] public List< EffectSettings > Effects { get; private set; } = new();

        public T Get< T >() where T : EffectSettings
        {
            return Effects.OfType< T >().FirstOrDefault();
        }
    }
}
