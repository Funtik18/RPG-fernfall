using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleRoundFactory
    {
        public BattleRound Create( Battle battle, int roundNumber, IReadOnlyList< UnitController > units )
        {
            var turns = new List< BattleTurn >();
            foreach ( var team in battle.Teams )
            {
                turns.Add( new BattleTurn( team ) );
            }

            return new BattleRound( roundNumber, turns );
        }
    }
}
