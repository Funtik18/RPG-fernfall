using Game.UI;
using Game.UI.HUDCampScreen;
using Game.UI.MainMenuScreen;
using System;
using Zenject;

namespace Game.GamePipeline
{
    public sealed class BootstrapCamp : IInitializable, IDisposable
    {
        private readonly UIManager _uiManager;
        
        public BootstrapCamp( UIManager uiManager )
        {
            _uiManager = uiManager ?? throw new ArgumentNullException( nameof(uiManager) );
        }
        
        public void Initialize()
        {
            _uiManager.ScreenAggregator.ShowAndCreateIfNotExist< HUDCampScreenViewModel >();
        }

        public void Dispose()
        {
            
        }
    }
}