using System;
using UnityEngine;

namespace Game.UI.HUDBattleScreen
{
    public sealed class HUDBattleScreen : UIScreen
    {
        public event Action OnTurnButtonClicked;
        public event Action OnRunButtonClicked;
        
        [ field: Space ]
        [ field: SerializeField ] public UIUnitInfo UnitInfo { get; private set; }
        [ field: SerializeField ] public UICellInfo CellInfo { get; private set; }
        [ field: SerializeField ] public UIActionMenu ActionMenu { get; private set; }
        [ field: SerializeField ] public UICombatForecast CombatForecast { get; private set; }
        
        public void OnTurnButtonClick() => OnTurnButtonClicked?.Invoke();
        
        public void OnRunButtonClick() => OnRunButtonClicked?.Invoke();
    }
}