using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleTeam
    {
        public string Id { get; }

        public IReadOnlyList< UnitController > Units => _units;
        private readonly List< UnitController > _units;

        public BattleTeam( string id, IEnumerable< UnitController > units )
        {
            Id = id;
            _units = new List< UnitController >( units );
        }

        public bool Contains( UnitController unit )
        {
            return unit != null && _units.Contains( unit );
        }
        
        public bool IsEnemyTeam() => string.Equals( Id, BattleParams.ENEMY_TEAM, StringComparison.InvariantCultureIgnoreCase );
    }
}