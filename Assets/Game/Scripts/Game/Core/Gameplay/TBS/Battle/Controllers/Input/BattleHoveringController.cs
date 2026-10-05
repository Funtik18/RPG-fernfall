using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleHoveringController
    {
        private readonly BattleSelectionController _selectionController;
        
        public BattleHoveringController( BattleSelectionController selectionController )
        {
            _selectionController = selectionController ?? throw new ArgumentNullException( nameof(selectionController) );
        }

        public void HoverUnit( UnitController unit )
        {
            unit.Highlighter.Highlight();
        }
        
        public void HoverExitUnit( UnitController unit )
        {
            if ( _selectionController.SelectedUnit == unit ) return;
            
            unit.Highlighter.Clear();
        }
    }
}