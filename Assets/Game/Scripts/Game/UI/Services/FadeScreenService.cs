using Cysharp.Threading.Tasks;
using Game.UI.FadeScreen;
using System;

namespace Game.UI.Services
{
    public sealed class FadeScreenService
    {
        private FadeScreenViewModel FadeScreenViewModel
        {
            get
            {
                if ( _fadeScreenViewModel == null )
                {
                    _fadeScreenViewModel = _uiManager.ScreenAggregator.GetOrCreateIfNotExist< FadeScreenViewModel >();
                    _fadeScreenViewModel.EnableView( false );
                }
                return _fadeScreenViewModel;
            }
        }
        private FadeScreenViewModel _fadeScreenViewModel;
        
        private readonly UIManager _uiManager;
        
        public FadeScreenService( UIManager uiManager )
        {
            _uiManager = uiManager ?? throw new System.ArgumentNullException( nameof(uiManager) );
        }

        public async UniTask FadeInOut( Action onFadeIn = null, Action onFadeOut = null, Action onLoaded = null, float waitTime = 0.33f )
        {
            await FadeIn();
            onFadeIn?.Invoke();
            await UniTask.WaitForSeconds( waitTime );
            onLoaded?.Invoke();
            await FadeOut();
            onFadeOut?.Invoke();
        }

        public async UniTask FadeIn( Action callback = null )
        {
            await FadeScreenViewModel.ModelView.Show( callback );
        }

        public async UniTask FadeOut( Action callback = null )
        {
            await FadeScreenViewModel.ModelView.Hide( callback );
        }
    }
}