using Cysharp.Threading.Tasks;
using System;

namespace Game.Core.Gameplay.TBS.Actions
{
    public sealed class RangeAttackAction : BattleAction
    {
        private readonly BattleGridController _gridController;
        private readonly BattlePathfinder _pathfinder;
        private readonly BattleCombatController _combatController;
        private readonly BattleCombatArtsController _combatArtsController;

        public override bool CompletesUnitTurn => true;

        public RangeAttackAction(
            UnitController unit,
            BattleGridController gridController,
            BattlePathfinder pathfinder,
            BattleCombatController combatController,
            BattleCombatArtsController combatArtsController
            ) : base( unit )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _pathfinder = pathfinder ?? throw new ArgumentNullException( nameof(pathfinder) );
            _combatController = combatController ?? throw new ArgumentNullException( nameof(combatController) );
            _combatArtsController = combatArtsController ?? throw new ArgumentNullException( nameof(combatArtsController) );
        }

        public override bool CanExecute( BattleActionTarget target )
        {
            if ( target.Cell == null ) return false;
            if ( target.CombatArt != null && !_combatArtsController.CanUseCombatArt( Owner, target.CombatArt ) ) return false;

            var targetUnit = target.Unit;
            if ( !_combatController.IsCanAttack( Owner, targetUnit ) ) return false;
            if ( !CanAttackCell( target.Cell ) ) return false;

            return true;
        }

        public override async UniTask Execute( BattleActionTarget target )
        {
            if ( !CanExecute( target ) ) return;

            await _combatController.Attack( Owner, target.Unit, target.CombatArt );
        }

        private bool CanAttackCell( BattleCell targetCell )
        {
            var startCell = _gridController.Registry.GetUnitCell( Owner );
            var attackCells = _pathfinder.GetAttackCells( startCell, Owner, Owner.GetAttackRangeAfterMovement() );

            foreach ( var cell in attackCells )
            {
                if ( cell == targetCell )
                {
                    return true;
                }
            }

            return false;
        }
    }
}
