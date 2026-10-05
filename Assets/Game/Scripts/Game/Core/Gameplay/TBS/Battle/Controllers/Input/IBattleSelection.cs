using System;

namespace Game.Core.Gameplay.TBS
{
    public interface IBattleSelection
    {
        event Action< UnitController > OnUnitSelected;
        event Action< UnitController > OnUnitDeselected;
        
        UnitController SelectedUnit { get; }
        BattleAction SelectedAction { get; }

        void SelectUnit( UnitController unit );
        void ClearUnit();

        void SelectAction( BattleAction action );
        void ClearAction();
    }
}