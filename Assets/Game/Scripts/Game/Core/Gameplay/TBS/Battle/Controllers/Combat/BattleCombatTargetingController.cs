using Game.Core.Systems.SheetSystem;
using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleCombatTargetingController
    {
        private Battle _battle;

        private readonly BattleGridController _gridController;
        private readonly BattlePathfinder _pathfinder;

        public BattleCombatTargetingController(
            BattleGridController gridController,
            BattlePathfinder pathfinder
            )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _pathfinder = pathfinder ?? throw new ArgumentNullException( nameof(pathfinder) );
        }

        public void Initialize( Battle battle )
        {
            _battle = battle ?? throw new ArgumentNullException( nameof(battle) );
        }

        public void Dispose()
        {
            _battle = null;
        }

        public bool IsCanAttackFromCurrentCell( UnitController attacker, UnitController target )
        {
            if ( !IsCanUseWeaponAgainstTarget( attacker, target, attacker.Sheet.Equipment.Weapon ) ) return false;

            return IsCanReachTargetFromCurrentCell( attacker, target, attacker.Sheet.Equipment.Weapon );
        }

        public bool IsCanReachTargetFromCurrentCell( UnitController attacker, UnitController target, Weapon weapon )
        {
            if ( weapon == null ) return false;
            if ( !weapon.CanUse ) return false;
            if ( !_gridController.Registry.TryGetUnitCell( attacker, out var attackerCell ) ) return false;
            if ( !_gridController.Registry.TryGetUnitCell( target, out var targetCell ) ) return false;

            var attackRange = attacker.GetAttackRangeAfterMovement( weapon );
            if ( attackRange <= 0 ) return false;

            var attackCells = _pathfinder.GetAttackCells( attackerCell, attacker, attackRange );
            foreach ( var cell in attackCells )
            {
                if ( cell == targetCell ) return true;
            }

            return false;
        }

        public bool IsCanUseWeaponAgainstTarget( UnitController attacker, UnitController target, Weapon weapon )
        {
            if ( attacker == target ) return false;
            if ( !attacker.IsAlive() ) return false;
            if ( !target.IsAlive() ) return false;
            if ( !weapon.CanUse ) return false;

            var attackerTeam = _battle.GetTeam( attacker );
            var targetTeam = _battle.GetTeam( target );
            if ( attackerTeam == null || targetTeam == null ) return false;
            var isAlly = attackerTeam == targetTeam;
            switch ( weapon.Config.TargetType )
            {
                case WeaponTargetType.Ally:
                    return isAlly;
                case WeaponTargetType.Enemy:
                    return !isAlly;
                default:
                    return false;
            }
        }

        public bool IsAttackFromDistance( UnitController attacker, UnitController defender )
        {
            if ( !_gridController.Registry.TryGetUnitCell( attacker, out var attackerCell ) ) return false;
            if ( !_gridController.Registry.TryGetUnitCell( defender, out var defenderCell ) ) return false;

            var meleeAttackCells = _pathfinder.GetAttackCells( attackerCell, attacker, 1 );
            foreach ( var cell in meleeAttackCells )
            {
                if ( cell == defenderCell ) return false;
            }

            return true;
        }
    }
}
