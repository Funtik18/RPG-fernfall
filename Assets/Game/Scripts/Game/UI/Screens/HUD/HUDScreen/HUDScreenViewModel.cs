using Game.Managers.InputManager;
using Game.Managers.SceneManager;
using Game.UI.Dialogs.MenuDialog;
using SoosvetGames.VVM;
using UnityEngine;

namespace Game.UI.HUDScreen
{
    public sealed class HUDScreenViewModel : ViewModel< HUDScreen >
    {
        private InputActionVoid _menuInputAction;
        private MenuDialogViewModel _menuDialogViewModel;
        
        private readonly SceneLoader _sceneLoader;
        private readonly UIManager _uiManager;
        
        public HUDScreenViewModel(
            SceneLoader sceneLoader,
            UIManager uiManager
            )
        {
            _sceneLoader = sceneLoader ?? throw new System.ArgumentNullException( nameof(sceneLoader) );
            _uiManager = uiManager ?? throw new System.ArgumentNullException( nameof(uiManager) );
        }
        
        protected override void SubscribeView()
        {
            base.SubscribeView();
            
            ModelView.OnCampButtonClicked += CampButtonClickedHandler;

            _menuInputAction = new( InputManager.Inputs.UI.Menu, MenuButtonClickedHandler );
            _menuInputAction.Enable();
        }

        protected override void UnSubscribeView()
        {
            base.UnSubscribeView();
            
            ModelView.OnCampButtonClicked -= CampButtonClickedHandler;
        }

        private void CampButtonClickedHandler()
        {
            if ( _sceneLoader.IsLoading ) return;

            _sceneLoader.LoadCamp();
            HideViewAndDispose();
        }

        private void MenuButtonClickedHandler()
        {
            if ( _menuDialogViewModel != null )
            {
                _menuDialogViewModel.OnShowingChanged -= ViewShowingChangedHandler;
                _menuDialogViewModel.HideView();
                _menuDialogViewModel = null;
                return;
            }
            
            _menuDialogViewModel = _uiManager.DialogAggregator.GetOrCreateIfNotExist< MenuDialogViewModel >();
            _menuDialogViewModel.OnShowingChanged += ViewShowingChangedHandler;
            _menuDialogViewModel.ShowView();
        }
        
        private void ViewShowingChangedHandler( IViewModel viewModel )
        {
            if ( viewModel.IsShowing ) return;
            viewModel.OnShowingChanged -= ViewShowingChangedHandler;
            if ( _menuDialogViewModel == viewModel )
            {
                _menuDialogViewModel = null;
            }
        }
    }
}