using Game.Core.Gameplay.TBS.Actions;
using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleActionMenuController
    {
        public event Action< UnitController > OnUnitActionMenuRequested;

        public bool IsOpened => _unit != null;

        private Battle _battle;
        private UnitController _unit;

        private readonly BattleActionFactory _actionFactory;
        private readonly BattleSelectionController _selectionController;

        public BattleActionMenuController(
            BattleActionFactory actionFactory,
            BattleSelectionController selectionController
            )
        {
            _actionFactory = actionFactory ?? throw new ArgumentNullException( nameof(actionFactory) );
            _selectionController = selectionController ?? throw new ArgumentNullException( nameof(selectionController) );
        }

        public void Initialize( Battle battle )
        {
            _battle = battle ?? throw new ArgumentNullException( nameof(battle) );

            _selectionController.OnUnitDeselected += UnitDeselectedHandler;
        }

        public void Dispose()
        {
            _selectionController.OnUnitDeselected -= UnitDeselectedHandler;

            _battle = null;
            Close();
        }

        public void Open( UnitController unit )
        {
            if ( unit == null ) return;
            if ( _selectionController.SelectedUnit != unit ) return;

            _unit = unit;
            _selectionController.ClearAction();

            OnUnitActionMenuRequested?.Invoke( unit );
        }

        public void Close()
        {
            _unit = null;
        }

        public bool SelectAttackAction()
        {
            if ( !TryGetOpenedSelectedUnit( out var selectedUnit ) ) return false;
            if ( !selectedUnit.CanAttack ) return false;

            var action = _actionFactory.Create< RangeAttackAction >( selectedUnit );
            _selectionController.SelectAction( action );
            Close();

            return true;
        }

        public bool WaitSelectedUnit()
        {
            if ( !TryGetOpenedSelectedUnit( out var selectedUnit ) ) return false;
            if ( _battle == null ) return false;

            var turn = _battle.CurrentRound?.CurrentTurn;
            if ( turn?.ActiveUnit != selectedUnit ) return false;

            turn.CompleteActiveUnit();
            Close();
            _selectionController.ClearAction();
            _selectionController.ClearUnit();

            return true;
        }

        private bool TryGetOpenedSelectedUnit( out UnitController unit )
        {
            unit = _selectionController.SelectedUnit;

            if ( _unit == null ) return false;
            if ( unit == null ) return false;

            return _unit == unit;
        }

        private void UnitDeselectedHandler( UnitController unit )
        {
            if ( unit != _unit ) return;

            Close();
        }
    }
}
