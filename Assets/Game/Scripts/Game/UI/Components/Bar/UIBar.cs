using DG.Tweening;
using System;
using UnityEngine;

namespace Game.UI
{
    public class UIBar : MonoBehaviour
    {
        [ SerializeField ] protected RectTransform _root;
        [ SerializeField ] protected RectTransform _bar;
        
        protected float BarSize => _root.sizeDelta.x;
        
        public virtual void ResetBar()
        {
            var size = _bar.sizeDelta;
            size.x = 0;
            _bar.sizeDelta = size;
        }
        
        public virtual void SetBar( float value )
        {
            var size = _bar.sizeDelta;
            size.x = value * BarSize;
            _bar.sizeDelta = size;
        }
        
        public virtual void DoBar( float value, float duration = 0.33f, Action callback = null )
        {
            var size = _bar.sizeDelta;
            size.x = value * BarSize;
            _bar.DOSizeDelta( size, duration ).OnComplete( () => callback?.Invoke() );
        }
    }
}