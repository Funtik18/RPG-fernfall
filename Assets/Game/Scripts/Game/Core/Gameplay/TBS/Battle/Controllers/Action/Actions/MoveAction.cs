using Cysharp.Threading.Tasks;
using System;

namespace Game.Core.Gameplay.TBS.Actions
{
    public sealed class MoveAction : BattleAction
    {
        private readonly BattleGridController _gridController;
        private readonly BattleMovementController _movementController;

        public MoveAction(
            UnitController unit,
            BattleGridController gridController,
            BattleMovementController movementController
            ) : base( unit )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );

            _movementController = movementController ?? throw new ArgumentNullException( nameof(movementController) );
        }

        public override bool CanExecute( BattleActionTarget target )
        {
            var cell = target.Cell;

            if ( cell == null ) return false;
            if ( _gridController.Registry.IsOccupied( cell ) ) return false;

            return _movementController.CanMove( Owner, cell );
        }

        public override async UniTask Execute( BattleActionTarget target )
        {
            await _movementController.Move( Owner, target.Cell );
            Owner.CompleteMove();
        }
    }
}