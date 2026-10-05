using UnityEngine;

namespace Game.Utils
{
    public static class SpriteRendererUtils
    {
        /// <summary>
        /// Изменяет scale SpriteRenderer так, чтобы его размер совпал
        /// с размером RectTransform на экране.
        /// </summary>
        public static void FitToRectTransform(
            SpriteRenderer spriteRenderer,
            RectTransform rectTransform,
            Camera worldCamera,
            bool preserveAspect = false
            )
        {
            if ( spriteRenderer == null || spriteRenderer.sprite == null || rectTransform == null || worldCamera == null )
            {
                Debug.LogError( "Не передан SpriteRenderer, Sprite, RectTransform или Camera." );
                return;
            }

            Canvas canvas = rectTransform.GetComponentInParent< Canvas >();
            Camera uiCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

            Vector3[] rectCorners = new Vector3[4];
            rectTransform.GetWorldCorners( rectCorners );

            // Плоскость, на которой находится спрайт.
            Plane spritePlane = new Plane( spriteRenderer.transform.forward, spriteRenderer.transform.position );
            Vector3[] projectedCorners = new Vector3[4];

            for ( int i = 0; i < rectCorners.Length; i++ )
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint( uiCamera, rectCorners[ i ] );

                Ray ray = worldCamera.ScreenPointToRay( screenPoint );

                if ( !spritePlane.Raycast( ray, out float distance ) )
                {
                    Debug.LogError( "Не удалось спроецировать RectTransform на плоскость спрайта." );
                    return;
                }

                projectedCorners[ i ] = ray.GetPoint( distance );
            }

            // GetWorldCorners:
            // 0 — левый нижний
            // 1 — левый верхний
            // 2 — правый верхний
            // 3 — правый нижний
            float targetWidth = Vector3.Distance( projectedCorners[ 0 ], projectedCorners[ 3 ] );
            float targetHeight = Vector3.Distance( projectedCorners[ 0 ], projectedCorners[ 1 ] );

            Vector2 originalSpriteSize = spriteRenderer.sprite.bounds.size;
            Vector3 parentScale = spriteRenderer.transform.parent != null ? spriteRenderer.transform.parent.lossyScale : Vector3.one;
            float scaleX = targetWidth / ( originalSpriteSize.x * Mathf.Abs( parentScale.x ) );
            float scaleY = targetHeight / ( originalSpriteSize.y * Mathf.Abs( parentScale.y ) );
            Vector3 localScale = spriteRenderer.transform.localScale;

            if ( preserveAspect )
            {
                float scale = Mathf.Min( scaleX, scaleY );

                localScale.x = Mathf.Sign( localScale.x ) * scale;
                localScale.y = Mathf.Sign( localScale.y ) * scale;
            }
            else
            {
                localScale.x = Mathf.Sign( localScale.x ) * scaleX;
                localScale.y = Mathf.Sign( localScale.y ) * scaleY;
            }

            spriteRenderer.transform.localScale = localScale;
        }
    }
}