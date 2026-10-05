using DG.Tweening;
using Game.UI.Services;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.UI
{
    public sealed class UITweenButtonComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [ SerializeField ] private DOTweenAnimation _onPointerDownAnimation;
        [ SerializeField ] private DOTweenAnimation _onPointerUpAnimation;
        [ SerializeField ] private DOTweenAnimation _onPointerEnterAnimation;
        [ SerializeField ] private DOTweenAnimation _onPointerExitAnimation;

        private UIFeedbackService _uiPlayerFeedbackService;
        
        [ Inject ]
        private void Construct( UIFeedbackService uiPlayerFeedbackService )
        {
            _uiPlayerFeedbackService = uiPlayerFeedbackService ?? throw new ArgumentNullException( nameof(uiPlayerFeedbackService) );
        }
        
        public void OnPointerDown( PointerEventData eventData )
        {
            _onPointerDownAnimation?.DORestart();
        }

        public void OnPointerUp( PointerEventData eventData )
        {
            _onPointerUpAnimation?.DORestart();
        }
        
        public void OnPointerEnter( PointerEventData eventData )
        {
            _onPointerEnterAnimation?.DORestart();
        }

        public void OnPointerExit( PointerEventData eventData )
        {
            _onPointerExitAnimation?.DORestart();
        }
        
        public void OnPointerClick( PointerEventData eventData )
        {
            _uiPlayerFeedbackService.PlayUIButton();
        }
    }
}