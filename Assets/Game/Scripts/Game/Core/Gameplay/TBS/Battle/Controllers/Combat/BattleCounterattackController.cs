using Cysharp.Threading.Tasks;
using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleCounterattackController
    {
        private readonly BattleCombatTargetingController _targetingController;
        private readonly BattleAttackExecutor _attackExecutor;

        public BattleCounterattackController(
            BattleCombatTargetingController targetingController,
            BattleAttackExecutor attackExecutor
            )
        {
            _targetingController = targetingController ?? throw new ArgumentNullException( nameof(targetingController) );
            _attackExecutor = attackExecutor ?? throw new ArgumentNullException( nameof(attackExecutor) );
        }

        public bool IsCanCounterattack( UnitController defender, UnitController attacker )
        {
            if ( defender == attacker ) return false;
            if ( !defender.Sheet.Class.IsCanCounterattack ) return false;
            if ( !defender.IsAlive() ) return false;
            if ( !attacker.IsAlive() ) return false;

            return _targetingController.IsCanAttackFromCurrentCell( defender, attacker );
        }

        public async UniTask< bool > TryCounterattack( UnitController defender, UnitController attacker )
        {
            if ( !IsCanCounterattack( defender, attacker ) ) return false;

            var attackerCanCounterattack = IsCanCounterattack( attacker, defender );
            await _attackExecutor.ExecuteAttack( defender, attacker, false, attackerCanCounterattack );
            return true;
        }
    }
}
