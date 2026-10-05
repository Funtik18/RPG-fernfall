using System;

namespace Game.UI.HUDCampScreen
{
    public sealed class HUDCampScreen : UIScreen
    {
        public event Action OnMapButtonClicked;
        
        public void OnMapButtonClick() => OnMapButtonClicked?.Invoke();
    }
}