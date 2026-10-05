using TMPro;
using UnityEngine;

namespace Game.UI.HUDBattleScreen
{
    public sealed class UIUnitStatInfo : MonoBehaviour
    {
        [ SerializeField ] private TextMeshProUGUI _value;
        [ SerializeField ] private GameObject _buffArrow;
        [ SerializeField ] private GameObject _debuffArrow;
        [ SerializeField ] private Color _color;
        [ SerializeField ] private Color _buffColor;
        [ SerializeField ] private Color _debuffColor;

        public void Set( string value, int state = 0 )
        {
            _value.text = value;
            if ( state == 0 )
            {
                _value.color = _color;
                _buffArrow.SetActive( false );
                _debuffArrow.SetActive( false );
            }
            else if ( state == 1 )
            {
                _value.color = _buffColor;
                _buffArrow.SetActive( true );
                _debuffArrow.SetActive( false );
            }
            else if ( state == 2 )
            {
                _value.color = _debuffColor;
                _buffArrow.SetActive( false );
                _debuffArrow.SetActive( true );
            }
        }
    }
}