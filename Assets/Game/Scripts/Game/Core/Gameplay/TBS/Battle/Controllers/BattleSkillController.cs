using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleSkillController
    {
        private Battle _battle;

        private readonly BattleGridController _gridController;

        public BattleSkillController( BattleGridController gridController )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
        }

        public void Initialize( Battle battle )
        {
            _battle = battle ?? throw new ArgumentNullException( nameof(battle) );

            _battle.OnStarted += BattleStartedHandler;
            _gridController.Registry.OnUnitPlaced += UnitPlacedHandler;
            _gridController.Registry.OnUnitMoved += UnitMovedHandler;
            _gridController.Registry.OnUnitRemoved += UnitRemovedHandler;
        }

        public void Dispose()
        {
            _gridController.Registry.OnUnitPlaced -= UnitPlacedHandler;
            _gridController.Registry.OnUnitMoved -= UnitMovedHandler;
            _gridController.Registry.OnUnitRemoved -= UnitRemovedHandler;

            if ( _battle != null )
            {
                _battle.OnStarted -= BattleStartedHandler;
                _battle = null;
            }
        }

        private void BattleStartedHandler()
        {
            foreach ( var unit in _battle.Units )
            {
                unit.Skills.ApplyInitialSkills();
            }

            RefreshAll();
        }

        private void UnitPlacedHandler( UnitController unit, BattleCell cell )
        {
            RefreshAll();
        }

        private void UnitMovedHandler( UnitController unit, BattleCell from, BattleCell to )
        {
            RefreshAll();
        }

        private void UnitRemovedHandler( UnitController unit, BattleCell cell )
        {
            RefreshAll();
        }

        private void RefreshAll()
        {
            foreach ( var unit in _battle.Units )
            {
                unit.Skills.Refresh();
            }
        }
    }
}
