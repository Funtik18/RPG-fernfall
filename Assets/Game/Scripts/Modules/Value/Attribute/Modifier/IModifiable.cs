using System;
using System.Collections.Generic;

namespace Value
{
    public interface IModifiable< M >
        where M : Modifier< float >
    {
        event Action OnModifiersChanged;

        float TotalValue { get; }
        float ModifyAddValue { get; }
        float ModifyPercentValue { get; }

        IReadOnlyList< M > Modifiers { get; }

        bool AddModifier( M modifier );
        bool RemoveModifier( M modifier );

        bool Contains( M modifier );
    }
}