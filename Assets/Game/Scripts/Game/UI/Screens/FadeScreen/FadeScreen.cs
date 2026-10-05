using Cysharp.Threading.Tasks;
using DG.Tweening;
using SoosvetGames.CommonTools.Extensions;
using System;

namespace Game.UI.FadeScreen
{
    public sealed class FadeScreen : UIScreen
    {
        public async UniTask Show( Action callback = null )
        {
            IsInProcess = true;
            CanvasGroup.alpha = 0f;
            CanvasGroup.Enable( true, false );
            IsShowing = true;

            _sequence?.Kill( true );
            _sequence = DOTween.Sequence();
            await _sequence
                .Append( CanvasGroup.DOFade( 1f, 0.33f ) )
                .OnComplete( () =>
                {
                    IsInProcess = false;

                    _sequence = null;
				
                    callback?.Invoke();
                } ).ToUniTask();
        }

        public async UniTask Hide( Action callback = null )
        {
            IsInProcess = true;
            CanvasGroup.Enable( false, false );
            IsShowing = false;
			
            _sequence?.Kill( true );
            _sequence = DOTween.Sequence();
            await _sequence
                .Append( CanvasGroup.DOFade( 0f, 0.16f ) )
                .OnComplete( () =>
                {
                    IsInProcess = false;

                    _sequence = null;
				
                    callback?.Invoke();
                } ).ToUniTask();
        }
    }
}