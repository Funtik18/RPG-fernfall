using Game.Core.Gameplay.TBS;
using Game.Core.Systems.SheetSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.HUDBattleScreen
{
    public sealed class UIUnitInfo : MonoBehaviour
    {
        public event Action OnNextUnitButtonClicked;
        public event Action OnPrevUnitButtonClicked;
        public event Action OnNextWeaponButtonClicked;
        public event Action OnPrevWeaponButtonClicked;
        
        [ SerializeField ] private TMPro.TextMeshProUGUI _unitName;
        [ SerializeField ] private TMPro.TextMeshProUGUI _unitClass;
        [ SerializeField ] private TMPro.TextMeshProUGUI _statLevel;
        [ SerializeField ] private Image _statExp;
        [ SerializeField ] private TMPro.TextMeshProUGUI _statHP;
        [ SerializeField ] private UIUnitStatInfo _statAtk;
        [ SerializeField ] private UIUnitStatInfo _statHit;
        [ SerializeField ] private UIUnitStatInfo _statDef;
        [ SerializeField ] private UIUnitStatInfo _statRes;
        [ SerializeField ] private UIUnitBadgeInfo _badgeInfo;
        [ SerializeField ] private TMPro.TextMeshProUGUI _weaponName;
        [ Space ]
        [ SerializeField ] private GameObject _prevUnitButton;
        [ SerializeField ] private GameObject _nextUnitButton;
        
        public bool IsShowing { get; private set; }
        
        private UnitController _currentUnit;
        private List< Weapon > _weapons;
        private int _weaponIndex;

        public void Enable( bool trigger )
        {
            IsShowing = trigger;
            gameObject.SetActive( trigger );
        }
        
        public void EnableUnitSelectorButtons( bool trigger )
        {
            _prevUnitButton.SetActive( trigger );
            _nextUnitButton.SetActive( trigger );
        }
        
        public void SetUnit( UnitController unit )
        {
            if ( unit == null )
            {
                _currentUnit = null;
                _weapons = null;
                return;
            }

            _currentUnit = unit;
            _weapons = unit.Sheet.Equipment.GetWeapons().ToList();
            _weaponIndex = GetSelectedWeaponIndex();

            RefreshInfo();
        }

        private void RefreshInfo()
        {
            if ( _currentUnit == null ) return;

            _unitName.text = _currentUnit.Sheet.Information.Name;
            _unitClass.text = _currentUnit.Sheet.Class.Name;
            _statLevel.text = _currentUnit.Sheet.Stats.Level.Value.ToString();
            _statExp.fillAmount = _currentUnit.Sheet.Stats.Experience.PercentValue;
            _statHP.text = $"{_currentUnit.Sheet.Stats.HealthPoints.TotalValue}<size=60%>/{_currentUnit.Sheet.Stats.HealthPoints.MaxValue}</size>";
            _statAtk.Set( _currentUnit.GetAttack().ToString() );
            _statHit.Set( _currentUnit.GetHitRate().ToString() );
            _statDef.Set( ( (int)_currentUnit.Sheet.Stats.Defense.TotalValue ).ToString() );
            _statRes.Set( ( (int)_currentUnit.Sheet.Stats.Resist.TotalValue ).ToString() );

            var weapon = _currentUnit.Sheet.Equipment.Weapon;
            _badgeInfo.Set( weapon.Config.TriangleType );
            _badgeInfo.SetWeak( false );
            _weaponName.text = GetWeaponName( weapon );
            
            string GetWeaponName( Weapon weapon )
            {
                var config = weapon.Config;
                if ( weapon.IsBaseWeapon ) return config.Name;

                return config.IsInfinity
                    ? $"{config.Name} ({weapon.SpecialUses})"
                    : $"{config.Name} ({weapon.Uses})";
            }
        }

        private int GetSelectedWeaponIndex()
        {
            if ( _weapons == null || _weapons.Count == 0 ) return 0;

            var equippedWeapon = _currentUnit.Sheet.Equipment.Weapon;
            var index = _weapons.FindIndex( x => x.Config == equippedWeapon.Config );

            return index >= 0 ? index : 0;
        }

        private void SelectWeapon( int direction )
        {
            if ( !isActiveAndEnabled ) return;
            if ( _currentUnit == null ) return;

            _weaponIndex = ( _weaponIndex + direction + _weapons.Count ) % _weapons.Count;
            _currentUnit.Sheet.Equipment.EquipWeapon( _weapons[ _weaponIndex ] );
            RefreshInfo();
        }
        
        public void Clear()
        {
            _currentUnit = null;
            _weapons = null;
        }
        
        public void OnNextUnitButtonClick()
        {
            OnNextUnitButtonClicked?.Invoke();
        }
        
        public void OnPrevUnitButtonClick()
        {
            OnPrevUnitButtonClicked?.Invoke();
        }
        
        public void OnNextWeaponButtonClick()
        {
            SelectWeapon( 1 );
            OnNextWeaponButtonClicked?.Invoke();
        }
        
        public void OnPrevWeaponButtonClick()
        {
            SelectWeapon( -1 );
            OnPrevWeaponButtonClicked?.Invoke();
        }
    }
}
