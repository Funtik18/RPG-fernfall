using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ Serializable ]
    public abstract class EffectSettings : IEffectSettings
    {
        [ field: SerializeField ] public string UID { get; private set; }
        [ field: SerializeField ] public int X { get; private set; }
        [ field: SerializeField ] public int Y { get; private set; }
        [ field: SerializeField ] public int Z { get; private set; }
        
        public abstract Type GetEffectType();
    }
}
