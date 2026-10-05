using Game.Core.Systems.SheetSystem;
using System;

namespace Game.Core.Gameplay.Party
{
    public class PartyCharacter
    {
        public Sheet Sheet { get; }

        public PartyCharacter( Sheet sheet )
        {
            Sheet = sheet ?? throw new ArgumentNullException( nameof(sheet) );
        }
    }
}