using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleSelectionController : IBattleSelection
    {
        public event Action< UnitController > OnUnitSelected;
        public event Action< UnitController > OnUnitDeselected;

        public UnitController SelectedUnit { get; private set; }
        public BattleAction SelectedAction { get; private set; }

        public void SelectUnit( UnitController unit )
        {
            if ( SelectedUnit == unit ) return;

            ClearUnit();

            SelectedUnit = unit;
            OnUnitSelected?.Invoke( unit );
        }
        public void ClearUnit()
        {
            if ( SelectedUnit == null ) return;

            var unit = SelectedUnit;
            SelectedUnit = null;

            OnUnitDeselected?.Invoke( unit );
        }

        public void SelectAction( BattleAction action )
        {
            if ( action == null ) throw new ArgumentNullException( nameof(action) );
            if ( action.Owner != SelectedUnit ) throw new InvalidOperationException( "Action belongs to another unit." );

            SelectedAction = action;
        }
        public void ClearAction()
        {
            SelectedAction = null;
        }
    }
}