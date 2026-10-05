using System.Collections.Generic;

namespace Game.Core.Gameplay.Party
{
    public sealed class Party
    {
        public List< PartyCharacter > Characters { get; } = new();
    }
}