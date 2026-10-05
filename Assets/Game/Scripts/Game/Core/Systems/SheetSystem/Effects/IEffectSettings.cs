using System;

namespace Game.Core.Systems.SheetSystem
{
    public interface IEffectSettings
    {
        string UID { get; }

        Type GetEffectType();
    }
}