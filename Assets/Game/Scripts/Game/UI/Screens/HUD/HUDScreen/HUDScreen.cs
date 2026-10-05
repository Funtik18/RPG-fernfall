using System;

namespace Game.UI.HUDScreen
{
    public sealed class HUDScreen : UIScreen
    {
        public event Action OnCampButtonClicked;
        public event Action OnBattleButtonClicked;
        
        public void OnCampButtonClick() => OnCampButtonClicked?.Invoke();
        
        public void OnBattleButtonClick() => OnBattleButtonClicked?.Invoke();
    }
}