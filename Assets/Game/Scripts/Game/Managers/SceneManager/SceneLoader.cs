using Cysharp.Threading.Tasks;
using Game.UI;
using Game.UI.LoadingScreen;
using System;

namespace Game.Managers.SceneManager
{
    public sealed class SceneLoader
    {
        public bool IsLoading { get; private set; }
        
        private readonly SceneSettings _sceneSettings;
        private readonly SceneManager _sceneManager;
        private readonly WorldManager.WorldManager _worldManager;
        private readonly UIManager _uiManager;
        
        public SceneLoader(
            SceneSettings sceneSettings,
            SceneManager sceneManager,
            WorldManager.WorldManager worldManager,
            UIManager uiManager
            )
        {
            _sceneSettings = sceneSettings ?? throw new ArgumentNullException( nameof(sceneSettings) );
            _sceneManager = sceneManager ?? throw new ArgumentNullException( nameof(sceneManager) );
            _worldManager = worldManager ?? throw new ArgumentNullException( nameof(worldManager) );
            _uiManager = uiManager ?? throw new ArgumentNullException( nameof(uiManager) );
        }

        public void LoadMainMenu()
        {
            LoadScene( _sceneSettings.MainMenuScene.SceneName );
        }
        
        public void LoadGameplay()
        {
            LoadScene( _sceneSettings.GameplayScene.SceneName );
        }

        public void LoadCamp()
        {
            LoadScene( _sceneSettings.CampScene.SceneName );
        }

        public void LoadBattle()
        {
            LoadScene( _sceneSettings.BattleScene.SceneName );
        }

        private void LoadScene( string sceneName )
        {
            IsLoading = true;
            
            HideScreens();
            
            var loadingScreen = _uiManager.ScreenAggregator.GetOrCreateIfNotExist< LoadingScreenViewModel >();
            loadingScreen.ShowView( () =>
            {
                _worldManager.Dispose();
                _sceneManager.UnloadLastScene();
                _sceneManager.LoadSceneAdditive( sceneName, () =>
                {
                    IsLoading = false;
                    loadingScreen.HideViewAndDispose();
                } ).Forget();
            });
        }

        private void HideScreens()
        {
            if ( _uiManager.ScreenAggregator.IsAnyShowing() )
            {
                foreach ( var screen in _uiManager.ScreenAggregator.Registers )
                {
                    screen.HideViewAndDispose();
                }
            }
        }
    }
}