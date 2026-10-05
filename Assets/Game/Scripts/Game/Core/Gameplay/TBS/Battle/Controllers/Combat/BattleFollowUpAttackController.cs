using Cysharp.Threading.Tasks;
using Game.Core.Systems.SheetSystem;
using System;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleFollowUpAttackController
    {
        private readonly BattleCombatTargetingController _targetingController;
        private readonly BattleCounterattackController _counterattackController;
        private readonly BattleAttackExecutor _attackExecutor;
        private readonly TBSGameplayConfig _config;

        public BattleFollowUpAttackController(
            BattleCombatTargetingController targetingController,
            BattleCounterattackController counterattackController,
            BattleAttackExecutor attackExecutor,
            TBSGameplayConfig config
            )
        {
            _targetingController = targetingController ?? throw new ArgumentNullException( nameof(targetingController) );
            _counterattackController = counterattackController ?? throw new ArgumentNullException( nameof(counterattackController) );
            _attackExecutor = attackExecutor ?? throw new ArgumentNullException( nameof(attackExecutor) );
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }
        
        public UnitController GetFollowUpAttackUnit( UnitController attacker, UnitController defender, bool? attackerFollowUpOverride = null )
        {
            var unit = GetUnit();
            if ( unit == null ) return null;
            if ( !unit.Sheet.Class.IsCanFollowUpAttack ) return null;
            if ( unit == attacker && attackerFollowUpOverride == false ) return null;

            return unit;
            
            UnitController GetUnit()
            {
                var attackerSpeed = attacker.Sheet.Stats.Speed.TotalValue - Mathf.Max( 0, attacker.Sheet.GetWeight() - attacker.Sheet.Stats.Strength.TotalValue );
                var defenderSpeed = defender.Sheet.Stats.Speed.TotalValue - Mathf.Max( 0, defender.Sheet.GetWeight() - defender.Sheet.Stats.Strength.TotalValue );

                if ( attackerSpeed - defender.Sheet.Stats.Speed.TotalValue >= _config.FollowUpSpeedDifference )
                {
                    return attacker;
                }

                if ( defenderSpeed - attacker.Sheet.Stats.Speed.TotalValue >= _config.FollowUpSpeedDifference )
                {
                    return defender;
                }

                return null;
            }
        }

        public async UniTask TryFollowUpAttack( UnitController followUpUnit, UnitController attacker, UnitController defender, bool isCounterattacked, bool? attackerFollowUpOverride = null )
        {
            if ( followUpUnit == null ) return;
            if ( followUpUnit == attacker )
            {
                if ( !CanFollowUpAttack( attacker, defender, attackerFollowUpOverride ) ) return;

                var defenderCanCounterattack = _counterattackController.IsCanCounterattack( defender, attacker );
                await _attackExecutor.ExecuteAttack( attacker, defender, false, defenderCanCounterattack );
                return;
            }

            if ( followUpUnit != defender ) return;
            if ( !isCounterattacked ) return;
            if ( !CanFollowUpCounterattack( defender, attacker ) ) return;

            var attackerCanCounterattack = _counterattackController.IsCanCounterattack( attacker, defender );
            await _attackExecutor.ExecuteAttack( defender, attacker, false, attackerCanCounterattack );
        }

        private bool CanFollowUpAttack( UnitController attacker, UnitController defender, bool? attackerFollowUpOverride )
        {
            if ( !IsCanFollowUp( attacker, defender, attackerFollowUpOverride ) ) return false;

            return _targetingController.IsCanAttackFromCurrentCell( attacker, defender );
        }

        private bool CanFollowUpCounterattack( UnitController defender, UnitController attacker )
        {
            if ( !IsCanFollowUp( defender, attacker ) ) return false;

            return _counterattackController.IsCanCounterattack( defender, attacker );
        }

        private bool IsCanFollowUp( UnitController attacker, UnitController defender, bool? attackerFollowUpOverride = null )
        {
            if ( attacker == defender ) return false;
            if ( !attacker.Sheet.Class.IsCanFollowUpAttack ) return false;
            if ( attackerFollowUpOverride == false ) return false;
            if ( !attacker.IsAlive() ) return false;
            if ( !defender.IsAlive() ) return false;
            if ( attacker.Skills.Contains< ImmovableSkill >() ) return false;
            if ( defender.Skills.Contains< ImmovableSkill >() ) return false;

            return true;
        }
    }
}
