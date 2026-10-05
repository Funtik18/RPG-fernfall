using SoosvetGames.VVM;
using System;

namespace Game.UI.Dialogs.MenuDialog
{
    public sealed class MenuDialog : UIViewQuart
    {
        public event Action OnBackButtonClicked;
        public event Action OnContinueButtonClicked;
        public event Action OnSaveButtonClicked;
        public event Action OnExitButtonClicked;
        
        public void OnBackButtonClick() => OnBackButtonClicked?.Invoke();
        public void OnContinueButtonClick() => OnContinueButtonClicked?.Invoke();
        public void OnSaveButtonClick() => OnSaveButtonClicked?.Invoke();
        public void OnExitButtonClick() => OnExitButtonClicked?.Invoke();
    }
}