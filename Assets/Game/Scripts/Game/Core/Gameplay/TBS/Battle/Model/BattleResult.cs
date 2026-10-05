namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleResult
    {
        public BattleResultType Type { get; }
        public BattleTeam Winner { get; }
        public int CompletedRounds { get; }

        public BattleResult( BattleResultType type, BattleTeam winner, int completedRounds )
        {
            Type = type;
            Winner = winner;
            CompletedRounds = completedRounds;
        }
    }
}