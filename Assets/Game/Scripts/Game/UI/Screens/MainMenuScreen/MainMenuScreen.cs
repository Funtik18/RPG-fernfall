using System;
using UnityEngine;

namespace Game.UI.MainMenuScreen
{
    public sealed class MainMenuScreen : UIScreen
    {
        public event Action OnNewGameClicked;
        public event Action OnContinueClicked;
        public event Action OnSettingsClicked;
        public event Action OnExitClicked;
        
        [ field: SerializeField ] public GameObject ContinueButton { get; private set; }
        
        public void OnNewGameClick()
        {
            OnNewGameClicked?.Invoke();
        }

        public void OnContinueClick()
        {
            OnContinueClicked?.Invoke();
        }

        public void OnSettingsClick()
        {
            OnSettingsClicked?.Invoke();
        }

        public void OnExitClick()
        {
            OnExitClicked?.Invoke();
        }
    }
}