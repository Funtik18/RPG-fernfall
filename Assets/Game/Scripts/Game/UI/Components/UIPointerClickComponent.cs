using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class UIPointerClickComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [ SerializeField ] private Transform _target;
        [ SerializeField ] private float _scaleMultiply = 0.95f;

        private Vector3 startScale;
        private Vector3 endScale;

        protected virtual void Awake()
        {
            startScale = Vector3.one;
            endScale = startScale * _scaleMultiply;
        }

        public virtual void OnPointerDown( PointerEventData eventData )
        {
            _target.DOKill( true );
            _target.DOScale( endScale, 0.1f ).SetEase( Ease.Linear );
        }

        public virtual void OnPointerUp( PointerEventData eventData )
        {
            _target.DOKill( true );
            _target.DOScale( startScale, 0.1f ).SetEase( Ease.Linear );
        }

        public virtual void OnPointerClick( PointerEventData eventData )
        {
            
        }
    }
}