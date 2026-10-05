using Game.Core.Systems.SheetSystem;
using UnityEngine;

namespace Game.UI.HUDBattleScreen
{
    public sealed class UIUnitBadgeInfo : MonoBehaviour
    {
        [ SerializeField ] private GameObject _field;
        [ SerializeField ] private GameObject _forge;
        [ SerializeField ] private GameObject _forest;
        [ SerializeField ] private GameObject _craft;
        [ SerializeField ] private GameObject _weak;
        
        public void Set( WeaponType type )
        {
            _field.SetActive( type == WeaponType.Field );
            _forge.SetActive( type == WeaponType.Forge );
            _forest.SetActive( type == WeaponType.Forest );
            _craft.SetActive( type == WeaponType.Craft );
        }
        
        public void SetWeak( bool isWeak )
        {
            if ( _weak != null )
            {
                _weak.SetActive( isWeak );
            }
        }
    }
}