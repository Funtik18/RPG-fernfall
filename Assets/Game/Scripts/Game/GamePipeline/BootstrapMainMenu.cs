using Game.UI;
using Game.UI.MainMenuScreen;
using System;
using Zenject;

namespace Game.GamePipeline
{
    public sealed class BootstrapMainMenu : IInitializable
    {
        private readonly UIManager _uiManager;
        
        public BootstrapMainMenu( UIManager uiManager )
        {
            _uiManager = uiManager ?? throw new ArgumentNullException( nameof(uiManager) );
        }
        
        public void Initialize()
        {
            _uiManager.ScreenAggregator.ShowAndCreateIfNotExist< MainMenuScreenViewModel >();
        }
    }
}