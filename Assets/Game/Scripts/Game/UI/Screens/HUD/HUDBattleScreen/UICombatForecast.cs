using Game.Core.Gameplay.TBS;
using Game.Core.Systems.SheetSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI.HUDBattleScreen
{
    public sealed class UICombatForecast : MonoBehaviour
    {
        private const int NoneArtIndex = -1;

        public event Action OnAttackButtonClicked;
        public event Action OnBackButtonClicked;
        public event Action OnAttackerWeaponChanged;

        [ SerializeField ] private TMPro.TextMeshProUGUI _attackerName;
        [ SerializeField ] private TMPro.TextMeshProUGUI _attackerClass;
        [ SerializeField ] private UIUnitBadgeInfo _attackerBadge;
        [ SerializeField ] private TMPro.TextMeshProUGUI _attackerArt;
        [ SerializeField ] private TMPro.TextMeshProUGUI _attackerWeapon;
        [ SerializeField ] private TMPro.TextMeshProUGUI _attackerCell;
        [ Space ]
        [ SerializeField ] private TMPro.TextMeshProUGUI _defenderName;
        [ SerializeField ] private TMPro.TextMeshProUGUI _defenderClass;
        [ SerializeField ] private UIUnitBadgeInfo _defenderBadge;
        [ SerializeField ] private TMPro.TextMeshProUGUI _defenderWeapon;
        [ SerializeField ] private TMPro.TextMeshProUGUI _defenderCell;
        [ Space ]
        [ Space ]
        [ SerializeField ] private TMPro.TextMeshProUGUI _attackerHP;
        [ SerializeField ] private TMPro.TextMeshProUGUI _defenderHP;
        [ Space ]
        [ SerializeField ] private UICombatStrikeForecast _attack;
        [ SerializeField ] private UICombatStrikeForecast _counterAttack;
        [ SerializeField ] private UICombatStrikeForecast _followUpAttack;
        [ Space ]
        [ SerializeField ] private UICombatBarForecast _attackHPBar;
        [ SerializeField ] private UICombatBarForecast _defenderHPBar;
        [ Space ]
        [ SerializeField ] private GameObject _prevArtButton;
        [ SerializeField ] private GameObject _nextArtButton;
        [ SerializeField ] private GameObject _prevWeaponButton;
        [ SerializeField ] private GameObject _nextWeaponButton;

        public bool IsShowing { get; private set; }
        
        private int _attackerWeaponIndex;
        private List< Weapon > _attackerWeapons;

        public CombatArtConfig SelectedAttackerCombatArt
        {
            get
            {
                if ( _attackerArts == null || _attackerArts.Count == 0 ) return null;
                if ( _attackerArtIndex == NoneArtIndex ) return null;

                return _attackerArts[ _attackerArtIndex ];
            }
        }
        private int _attackerArtIndex;
        private List< CombatArtConfig > _attackerArts;
        
        private UnitController _attacker;
        private UnitController _defender;
        private BattleController _battleController;
        private WeaponRules _weaponRules;

        public void Initialize(
            BattleController battleController,
            WeaponRules weaponRules
            )
        {
            _battleController = battleController;
            _weaponRules = weaponRules;
        }
        
        public void Enable( bool trigger )
        {
            IsShowing = trigger;
            gameObject.SetActive( trigger );
        }
        
        public void EnableArtSelectorButtons( bool trigger )
        {
            _prevArtButton.SetActive( trigger );
            _nextArtButton.SetActive( trigger );
        }
        
        public void EnableWeaponSelectorButtons( bool trigger )
        {
            _prevWeaponButton.SetActive( trigger );
            _nextWeaponButton.SetActive( trigger );
        }
        
        public void SetUnits( UnitController attacker, UnitController defender )
        {
            _attacker = attacker;
            _defender = defender;
            
            _attackerWeapons = _attacker.Sheet.Equipment.GetWeapons()
                .Where( x => _battleController.CanAttackWithWeaponFromCurrentCell( _attacker, _defender, x ) )
                .ToList();
            EnableWeaponSelectorButtons( _attackerWeapons.Count > 1 );
            _attackerWeaponIndex = GetSelectedWeaponIndex();
            EquipAttackerWeapon( _attackerWeapons[ _attackerWeaponIndex ] );
            SetDefenderWeapon( _defender.Sheet.Equipment.Weapon );

            _attackerName.text = _attacker.Sheet.Information.Name;
            _attackerClass.text = _attacker.Sheet.Class.Name;
            _attackerCell.text = _battleController.GetUnitCellTerrainConfig( _attacker ).Name;
            _defenderName.text = _defender.Sheet.Information.Name;
            _defenderClass.text = _defender.Sheet.Class.Name;
            _defenderCell.text = _battleController.GetUnitCellTerrainConfig( _defender ).Name;
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            var attackerHP = _attacker.Sheet.Stats.HealthPoints.TotalValue;
            var defenderHP = _defender.Sheet.Stats.HealthPoints.TotalValue;
            
            _attackerHP.text = ( (int)attackerHP ).ToString();
            _defenderHP.text = ( (int)defenderHP ).ToString();
            
            var attackerWeapon = _attacker.Sheet.Equipment.Weapon;
            _attackerBadge.Set( attackerWeapon.Config.TriangleType );
            _attackerBadge.SetWeak( _weaponRules.IsWeak( attackerWeapon, _defender.Sheet.Equipment.Weapon ) );
            _defenderBadge.Set( _defender.Sheet.Equipment.Weapon.Config.TriangleType );
            _defenderBadge.SetWeak( _weaponRules.IsWeak( _defender.Sheet.Equipment.Weapon, attackerWeapon ) );

            var attackerAttackDamage = BattleCombatFormulas.GetDamage( _attacker, _defender, false, _weaponRules );
            _attack.Set( _attacker.GetHitRate(), _attacker.GetCriticalRate(), -attackerAttackDamage, defenderHP - attackerAttackDamage < 0 );
            var defenderCounterAttackDamage = BattleCombatFormulas.GetDamage( _defender, _attacker, false, _weaponRules );
            _counterAttack.Set( _defender.GetHitRate(), _defender.GetCriticalRate(), defenderCounterAttackDamage, attackerHP - defenderCounterAttackDamage < 0 );
            var attackerFollowUpAttackDamage = BattleCombatFormulas.GetDamage( _attacker, _defender, false, _weaponRules );
            _attack.Set( _attacker.GetHitRate(), _attacker.GetCriticalRate(), -attackerFollowUpAttackDamage, defenderHP - attackerFollowUpAttackDamage < 0 );
        }

        private int GetSelectedWeaponIndex()
        {
            var equippedWeapon = _attacker.Sheet.Equipment.Weapon;
            var index = _attackerWeapons.FindIndex( x => x.Config == equippedWeapon.Config );

            return index >= 0 ? index : 0;
        }

        private void EquipAttackerWeapon( Weapon weapon )
        {
            _attacker.Sheet.Equipment.EquipWeapon( weapon );
            _attackerWeapon.text = GetWeaponName();

            SetAttackerArts( weapon );
            
            string GetWeaponName()
            {
                var config = weapon.Config;
                if ( weapon.IsBaseWeapon ) return config.Name;

                return config.IsInfinity
                    ? $"{config.Name} ({weapon.SpecialUses})"
                    : $"{config.Name} ({weapon.Uses})";
            }
        }

        private void SetAttackerArts( Weapon weapon )
        {
            _attackerArts = weapon.Config.Family.CombatArts
                .Where( x => _battleController.CanUseCombatArt( _attacker, weapon, x ) )
                .ToList();
            EnableArtSelectorButtons( _attackerArts.Count > 0 );

            _attackerArtIndex = NoneArtIndex;
            RefreshAttackerArt();
        }

        private void RefreshAttackerArt()
        {
            var combatArt = SelectedAttackerCombatArt;
            _attackerArt.text = combatArt == null ? "None" : $"{combatArt.Name} ({_battleController.GetCombatArtUsesRemaining( _attacker, combatArt )})";
        }
        
        private void SetDefenderWeapon( Weapon weapon )
        {
            var config = weapon.Config;

            _defenderWeapon.text = config.Name;
            // _defenderWeaponDamage.text = config.Might.ToString();
        }

        public void OnLeftArtButtonClick()
        {
            if ( _attackerArts == null || _attackerArts.Count == 0 ) return;

            _attackerArtIndex++;
            if ( _attackerArtIndex >= _attackerArts.Count )
            {
                _attackerArtIndex = NoneArtIndex;
            }

            RefreshAttackerArt();
            RefreshInfo();
        }

        public void OnRightArtButtonClick()
        {
            if ( _attackerArts == null || _attackerArts.Count == 0 ) return;

            if ( _attackerArtIndex == NoneArtIndex )
            {
                _attackerArtIndex = _attackerArts.Count - 1;
            }
            else
            {
                _attackerArtIndex--;
            }

            RefreshAttackerArt();
            RefreshInfo();
        }
        
        public void OnLeftWeaponButtonClick()
        {
            if ( _attackerWeapons == null || _attackerWeapons.Count <= 1 ) return;

            _attackerWeaponIndex = ( _attackerWeaponIndex + 1 ) % _attackerWeapons.Count;
            EquipAttackerWeapon( _attackerWeapons[ _attackerWeaponIndex ] );
            RefreshInfo();
            OnAttackerWeaponChanged?.Invoke();
        }

        public void OnRightWeaponButtonClick()
        {
            if ( _attackerWeapons == null || _attackerWeapons.Count <= 1 ) return;

            _attackerWeaponIndex = ( _attackerWeaponIndex - 1 + _attackerWeapons.Count ) % _attackerWeapons.Count;
            EquipAttackerWeapon( _attackerWeapons[ _attackerWeaponIndex ] );
            RefreshInfo();
            OnAttackerWeaponChanged?.Invoke();
        }

        public void OnAttackButtonClick() => OnAttackButtonClicked?.Invoke();
        public void OnBackButtonClick() => OnBackButtonClicked?.Invoke();
    }
}
