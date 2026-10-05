using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleUnitPreviewController
    {
        private readonly BattleSelectionController _selectionController;
        private readonly BattleActionController _actionController;
        
        public BattleUnitPreviewController(
            BattleSelectionController selectionController,
            BattleActionController actionController
            )
        {
            _selectionController = selectionController ?? throw new ArgumentNullException( nameof(selectionController) );
            _actionController = actionController ?? throw new ArgumentNullException( nameof(actionController) );
        }
        
        public void Initialize()
        {
            _actionController.OnActionExecute += ActionExecuteHandler;
            _actionController.OnActionExecuted += ActionExecutedHandler;
            _selectionController.OnUnitSelected += UnitSelectedHandler;
            _selectionController.OnUnitDeselected += UnitDeselectedHandler;
        }

        public void Dispose()
        {
            _actionController.OnActionExecute -= ActionExecuteHandler;
            _actionController.OnActionExecuted -= ActionExecutedHandler;
            _selectionController.OnUnitSelected -= UnitSelectedHandler;
            _selectionController.OnUnitDeselected -= UnitDeselectedHandler;

            ClearPreview();
        }
        
        private void UnitSelectedHandler( UnitController unit )
        {
            ClearPreview();

            unit.Highlighter.Highlight();
        }

        private void UnitDeselectedHandler( UnitController unit )
        {
            unit.Highlighter.Clear();
            
            ClearPreview();
        }
        
        private void ActionExecuteHandler()
        {
            ClearPreview();
        }
        
        private void ActionExecutedHandler()
        {
            if ( _selectionController.SelectedUnit == null ) return;
            UnitSelectedHandler( _selectionController.SelectedUnit );
        }
        
        private void ClearPreview()
        {
            
        }
    }
}