using Game.UI.Services;
using System;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.UI
{
    public sealed class UIButtonComponent : UIPointerClickComponent
    {
        private UIFeedbackService _uiPlayerFeedbackService;
        
        [ Inject ]
        private void Construct( UIFeedbackService uiPlayerFeedbackService )
        {
            _uiPlayerFeedbackService = uiPlayerFeedbackService ?? throw new ArgumentNullException( nameof(uiPlayerFeedbackService) );
        }
        
        public override void OnPointerClick( PointerEventData eventData )
        {
            base.OnPointerClick( eventData );
            
            _uiPlayerFeedbackService.PlayUIButton();
        }
    }
}