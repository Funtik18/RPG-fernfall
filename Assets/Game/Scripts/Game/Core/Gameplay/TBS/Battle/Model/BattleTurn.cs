using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleTurn
    {
        public event Action< BattleTurn > OnChanged;
        public event Action< BattleTurn > OnStarted;
        public event Action< BattleTurn > OnFinished;
        public event Action< UnitController > OnUnitSelected;
        public event Action< UnitController > OnUnitCompleted;

        public BattleTeam Team { get; }

        public UnitController ActiveUnit { get; private set; }

        public bool IsActive { get; private set; }

        public bool AllUnitsCompleted => Team.Units.All( HasCompletedUnit );
        
        public IReadOnlyCollection< UnitController > CompletedUnits => _completedUnits;
        private readonly HashSet< UnitController > _completedUnits = new();

        public BattleTurn( BattleTeam team )
        {
            Team = team ?? throw new ArgumentNullException( nameof(team) );
        }

        public void Start()
        {
            if ( IsActive )
                return;

            IsActive = true;

            OnStarted?.Invoke( this );
            OnChanged?.Invoke( this );
        }

        public bool TrySelectUnit( UnitController unit )
        {
            if ( !IsActive ) return false;
            if ( unit == null ) return false;
            if ( !Team.Contains( unit ) ) return false;
            if ( HasCompletedUnit( unit ) ) return false;

            ActiveUnit = unit;

            OnUnitSelected?.Invoke( unit );
            OnChanged?.Invoke( this );

            return true;
        }

        public void CompleteActiveUnit()
        {
            if ( !IsActive ) return;
            if ( ActiveUnit == null ) return;

            var unit = ActiveUnit;

            unit.CompleteTurn();
            _completedUnits.Add( unit );
            ActiveUnit = null;

            OnUnitCompleted?.Invoke( unit );
            OnChanged?.Invoke( this );
        }

        public bool HasCompletedUnit( UnitController unit ) => unit != null && _completedUnits.Contains( unit );

        public void Finish()
        {
            if ( !IsActive )
                return;

            ActiveUnit = null;
            IsActive = false;

            OnChanged?.Invoke( this );
            OnFinished?.Invoke( this );
        }
    }
}
