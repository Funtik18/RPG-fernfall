using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    public abstract class EffectConfig : ScriptableObject, IEffectSettings
    {
        [ field: SerializeField ] public string UID { get; private set; }
        [ field: SerializeField, Min( 0 ) ] public int X { get; private set; } = 1;
        [ field: SerializeField, Min( 0 ) ] public int Y { get; private set; } = 1;
        [ field: SerializeField, Min( 1 ) ] public int Z { get; private set; } = 1;
        
        public abstract Type GetEffectType();
    }
}
