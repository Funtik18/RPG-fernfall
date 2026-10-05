using Game.Managers.SceneManager;
using SoosvetGames.VVM;

namespace Game.UI.HUDCampScreen
{
    public sealed class HUDCampScreenViewModel : ViewModel< HUDCampScreen >
    {
        private readonly SceneLoader _sceneLoader;
        
        public HUDCampScreenViewModel( SceneLoader sceneLoader )
        {
            _sceneLoader = sceneLoader ?? throw new System.ArgumentNullException( nameof(sceneLoader) );
        }
        
        protected override void SubscribeView()
        {
            base.SubscribeView();
            
            ModelView.OnMapButtonClicked += MapButtonClickedHandler;
        }

        protected override void UnSubscribeView()
        {
            base.UnSubscribeView();
            
            ModelView.OnMapButtonClicked -= MapButtonClickedHandler;
        }

        private void MapButtonClickedHandler()
        {
            if ( _sceneLoader.IsLoading ) return;
            
            _sceneLoader.LoadGameplay();
            HideViewAndDispose();
        }
    }
}