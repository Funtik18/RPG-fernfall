using Cysharp.Threading.Tasks;
using Game.Core.Systems.SheetSystem;
using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleCombatController
    {
        public event Action< UnitController, UnitController, float, float > OnDamageApplied
        {
            add => _attackExecutor.OnDamageApplied += value;
            remove => _attackExecutor.OnDamageApplied -= value;
        }

        public event Action< UnitController, UnitController > OnUnitKilled
        {
            add => _attackExecutor.OnUnitKilled += value;
            remove => _attackExecutor.OnUnitKilled -= value;
        }

        private readonly BattleCombatTargetingController _targetingController;
        private readonly BattleCombatArtsController _combatArtsController;
        private readonly BattleAttackExecutor _attackExecutor;
        private readonly BattleCounterattackController _counterattackController;
        private readonly BattleFollowUpAttackController _followUpAttackController;
        
        public BattleCombatController(
            BattleCombatTargetingController targetingController,
            BattleCombatArtsController combatArtsController,
            BattleAttackExecutor attackExecutor,
            BattleCounterattackController counterattackController,
            BattleFollowUpAttackController followUpAttackController
            )
        {
            _targetingController = targetingController ?? throw new ArgumentNullException( nameof(targetingController) );
            _combatArtsController = combatArtsController ?? throw new ArgumentNullException( nameof(combatArtsController) );
            _attackExecutor = attackExecutor ?? throw new ArgumentNullException( nameof(attackExecutor) );
            _counterattackController = counterattackController ?? throw new ArgumentNullException( nameof(counterattackController) );
            _followUpAttackController = followUpAttackController ?? throw new ArgumentNullException( nameof(followUpAttackController) );
        }

        public void Initialize( Battle battle )
        {
            _targetingController.Initialize( battle );
        }

        public void Dispose()
        {
            _targetingController.Dispose();
        }

        public bool IsCanAttack( UnitController attacker, UnitController defender )
        {
            return IsCanAttack( attacker, defender, attacker?.Sheet?.Equipment?.Weapon );
        }

        public bool IsCanAttack( UnitController attacker, UnitController defender, Weapon weapon )
        {
            if ( !attacker.CanAttack ) return false;

            return _targetingController.IsCanUseWeaponAgainstTarget( attacker, defender, weapon );
        }

        public bool IsCanAttackWithWeaponFromCurrentCell( UnitController attacker, UnitController target, Weapon weapon )
        {
            if ( !IsCanAttack( attacker, target, weapon ) ) return false;

            return _targetingController.IsCanReachTargetFromCurrentCell( attacker, target, weapon );
        }

        public bool IsCanAttackWithAnyWeaponFromCurrentCell( UnitController attacker, UnitController target )
        {
            foreach ( var weapon in attacker.Sheet.Equipment.GetWeapons() )
            {
                if ( IsCanAttackWithWeaponFromCurrentCell( attacker, target, weapon ) ) return true;
            }

            return false;
        }

        public bool CanUseCombatArt( UnitController attacker, CombatArtConfig config )
        {
            return CanUseCombatArt( attacker, attacker?.Sheet?.Equipment?.Weapon, config );
        }

        public bool CanUseCombatArt( UnitController attacker, Weapon weapon, CombatArtConfig config )
        {
            return _combatArtsController.CanUseCombatArt( attacker, weapon, config );
        }

        public int GetCombatArtUsesRemaining( UnitController attacker, CombatArtConfig config )
        {
            return _combatArtsController.GetCombatArtUsesRemaining( attacker, config );
        }

        public bool IsCanTargetWithAnyWeapon( UnitController attacker, UnitController target )
        {
            if ( attacker?.Sheet?.Equipment == null ) return false;

            foreach ( var weapon in attacker.Sheet.Equipment.GetWeapons() )
            {
                if ( _targetingController.IsCanUseWeaponAgainstTarget( attacker, target, weapon ) ) return true;
            }

            return false;
        }

        public async UniTask Attack( UnitController attacker, UnitController defender, CombatArtConfig combatArtConfig = null )
        {
            if ( !IsCanAttack( attacker, defender ) ) return;

            if ( attacker.Sheet.Equipment.Weapon.Config.TargetType == WeaponTargetType.Ally )
            {
                await _attackExecutor.ExecuteSupport( attacker, defender );
            }
            else
            {
                if ( combatArtConfig != null && !CanUseCombatArt( attacker, combatArtConfig ) ) return;

                var combatArt = _combatArtsController.Create( attacker, combatArtConfig );
                var attackerFollowUpOverride = combatArt?.Config.FollowUp;
                var followUpUnit = _followUpAttackController.GetFollowUpAttackUnit( attacker, defender, attackerFollowUpOverride );
                var defenderCanCounterattack = _counterattackController.IsCanCounterattack( defender, attacker );
                var isCounterattacked = false;

                if ( defenderCanCounterattack && defender.Skills.Contains< VantageSkill >() )
                {
                    isCounterattacked = await _counterattackController.TryCounterattack( defender, attacker );
                    if ( !attacker.IsAlive() ) return;

                    defenderCanCounterattack = false; // && _counterattackController.IsCanCounterattack( defender, attacker );
                }

                if ( combatArtConfig != null && !_combatArtsController.TrySpendUse( attacker, combatArtConfig ) ) return;

                attacker.Skills.OnBeforeAttack( defender );
                await _attackExecutor.ExecuteAttack( attacker, defender, true, defenderCanCounterattack, combatArt );
                if ( !isCounterattacked )
                {
                    isCounterattacked = await _counterattackController.TryCounterattack( defender, attacker );
                }
                await _followUpAttackController.TryFollowUpAttack( followUpUnit, attacker, defender, isCounterattacked, attackerFollowUpOverride );
                attacker.Skills.OnAfterAttack( defender );
            }
        }

        public UniTask Kill( UnitController unit )
        {
            return _attackExecutor.Kill( unit );
        }
    }
}
