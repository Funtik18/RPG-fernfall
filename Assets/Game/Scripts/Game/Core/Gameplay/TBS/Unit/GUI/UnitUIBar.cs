using UnityEngine;
using UnityEngine.UI;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitUIBar : MonoBehaviour
    {
        [ SerializeField ] private Image _bar;
        [ SerializeField ] private Image _track;
        
        public void ResetBar()
        {
            SetBar( 0 );
        }
        
        public void SetBar( float value )
        {
            _bar.fillAmount = Mathf.Lerp( 0, _track.fillAmount,value );
        }
    }
}