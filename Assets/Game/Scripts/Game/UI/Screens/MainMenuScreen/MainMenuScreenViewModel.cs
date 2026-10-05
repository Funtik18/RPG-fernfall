using Game.Managers.CampaignManager;
using SoosvetGames.VVM;
using System;
using UnityEngine;

namespace Game.UI.MainMenuScreen
{
    public sealed class MainMenuScreenViewModel : ViewModel< MainMenuScreen >
    {
        private readonly CampaignManager _campaignManager;
        
        public MainMenuScreenViewModel( CampaignManager campaignManager )
        {
            _campaignManager = campaignManager ?? throw new ArgumentNullException( nameof(campaignManager) );
        }
        
        protected override void SubscribeView()
        {
            base.SubscribeView();

            ModelView.OnNewGameClicked += NewGameClickedHandler;
            ModelView.OnContinueClicked += ContinueClickedHandler;
            ModelView.OnSettingsClicked += SettingsClickedHandler;
            ModelView.OnExitClicked += ExitClickedHandler;
        }

        protected override void UnSubscribeView()
        {
            base.UnSubscribeView();
            
            ModelView.OnNewGameClicked -= NewGameClickedHandler;
            ModelView.OnContinueClicked -= ContinueClickedHandler;
            ModelView.OnSettingsClicked -= SettingsClickedHandler;
            ModelView.OnExitClicked -= ExitClickedHandler;
        }

        protected override void OnViewShowingChanged()
        {
            if ( !ModelView.IsShowing ) return;
            
            ModelView.Canvas.worldCamera = Camera.main;
            ModelView.ContinueButton.SetActive( _campaignManager.IsHasCampaign );
            
            // Показать курсор
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void NewGameClickedHandler()
        {
            _campaignManager.StartNewCampaign();
            EnableView( false );
        }

        private void ContinueClickedHandler()
        {
            _campaignManager.ContinueCampaign();
            EnableView( false );
        }

        private void SettingsClickedHandler()
        {
            
        }
        
        private void ExitClickedHandler()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}