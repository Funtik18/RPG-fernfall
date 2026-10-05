using Game.Managers.SceneManager;
using Game.Systems.StorageSystem;
using SoosvetGames.VVM;
using System;

namespace Game.UI.Dialogs.MenuDialog
{
    public sealed class MenuDialogViewModel : ViewModel< MenuDialog >
    {
        private readonly SceneLoader _sceneLoader;
        private readonly DataHolder _dataHolder;
        
        public MenuDialogViewModel(
            SceneLoader sceneLoader,
            DataHolder dataHolder
            )
        {
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException( nameof(sceneLoader) );
            _dataHolder = dataHolder ?? throw new ArgumentNullException( nameof(dataHolder) );
        }
        
        protected override void SubscribeView()
        {
            base.SubscribeView();

            ModelView.OnBackButtonClicked += BackButtonClickedHandler;
            ModelView.OnContinueButtonClicked += ContinueButtonClickedHandler;
            ModelView.OnContinueButtonClicked += SaveButtonClickedHandler;
            ModelView.OnExitButtonClicked += ExitButtonClickedHandler;
        }

        protected override void UnSubscribeView()
        {
            base.UnSubscribeView();
            
            ModelView.OnBackButtonClicked -= BackButtonClickedHandler;
            ModelView.OnContinueButtonClicked -= ContinueButtonClickedHandler;
            ModelView.OnContinueButtonClicked -= SaveButtonClickedHandler;
            ModelView.OnExitButtonClicked -= ExitButtonClickedHandler;
        }

        private void BackButtonClickedHandler()
        {
            HideView();
        }

        private void ContinueButtonClickedHandler()
        {
            HideView();
        }
        
        private void SaveButtonClickedHandler()
        {
            _dataHolder.Save();
            HideView();
        }
        
        private void ExitButtonClickedHandler()
        {
            _sceneLoader.LoadMainMenu();
            HideViewAndDispose();
        }
    }
}