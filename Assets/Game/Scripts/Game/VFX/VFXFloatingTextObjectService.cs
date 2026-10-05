using DG.Tweening;
using Game.Systems.CameraSystem;
using System;
using UnityEngine;

namespace Game.VFX
{
    public sealed class VFXFloatingTextObjectService
    {
        private readonly VFXFloatingTextObject.Factory _vfxFloatingTextObjectFactory;
        private readonly CameraFacingService _cameraFacingService;
        
        public VFXFloatingTextObjectService(
            VFXFloatingTextObject.Factory vfxFloatingTextObjectFactory,
            CameraFacingService cameraFacingService
            )
        {
            _vfxFloatingTextObjectFactory = vfxFloatingTextObjectFactory ?? throw new ArgumentNullException( nameof(vfxFloatingTextObjectFactory) );
            _cameraFacingService = cameraFacingService ?? throw new ArgumentNullException( nameof(cameraFacingService) );
        }

        public void DoDamageText( string msg, Vector3 worldPosition )
        {
            var text = _vfxFloatingTextObjectFactory.Create();
            text.SetText( msg );
            text.EnableDamageText();

            Transform transform = text.transform;

            _cameraFacingService.Add( transform );

            transform.position = worldPosition;
            transform.localScale = Vector3.one;

            text.DamageText.alpha = 1f;

            const float duration = 0.8f;

            Sequence sequence = DOTween.Sequence()
                .Join( transform.DOMoveY( worldPosition.y + 0.75f, duration ).SetEase( Ease.OutQuad ) )
                .Insert( 0.2f, text.DamageText.DOFade( 0f, duration - 0.2f ).SetEase( Ease.InQuad ) );

            sequence.OnComplete( () =>
            {
                _cameraFacingService.Remove( transform );
                text.DespawnIt();
            } );
        }

        public void DoMissHitText( string msg, Vector3 worldPosition )
        {
            var text = _vfxFloatingTextObjectFactory.Create();
            text.SetText( msg );
            text.EnableMissText();

            Transform transform = text.transform;

            _cameraFacingService.Add( transform );

            transform.position = worldPosition;
            transform.localScale = Vector3.one * 0.7f;

            text.MissText.alpha = 1f;

            const float popDuration = 0.2f;
            const float holdDuration = 0.45f;
            const float floatDuration = 0.75f;

            DOTween.Sequence()
                .Append( transform.DOScale( 1.15f, popDuration ).SetEase( Ease.OutBack ) )
                .Append( transform.DOScale( 1f, 0.1f ).SetEase( Ease.OutQuad ) )
                .AppendInterval( holdDuration )
                .Append( transform.DOMoveY( worldPosition.y + 0.5f, floatDuration ).SetEase( Ease.OutQuad ) )
                .Join( text.MissText.DOFade( 0f, floatDuration ).SetEase( Ease.InQuad ) )
                .OnComplete( () =>
            {
                _cameraFacingService.Remove( transform );
                text.DespawnIt();
            } );
        }
        
        public void DoCritDamageText( string msg, Vector3 worldPosition )
        {
            var text = _vfxFloatingTextObjectFactory.Create();
            text.SetText( msg );
            text.EnableCritText();

            Transform transform = text.transform;

            _cameraFacingService.Add( transform );

            transform.position = worldPosition;
            transform.localScale = Vector3.one * 0.7f;

            text.CritText.alpha = 1f;

            const float popDuration = 0.2f;
            const float holdDuration = 0.45f;
            const float floatDuration = 0.75f;

            DOTween.Sequence()
                .Append( transform.DOScale( 1.15f, popDuration ).SetEase( Ease.OutBack ) )
                .Append( transform.DOScale( 1f, 0.1f ).SetEase( Ease.OutQuad ) )
                .AppendInterval( holdDuration )
                .Append( transform.DOMoveY( worldPosition.y + 0.5f, floatDuration ).SetEase( Ease.OutQuad ) )
                .Join( text.CritText.DOFade( 0f, floatDuration ).SetEase( Ease.InQuad ) )
                .OnComplete( () =>
                {
                    _cameraFacingService.Remove( transform );
                    text.DespawnIt();
                } );
        }
    }
}