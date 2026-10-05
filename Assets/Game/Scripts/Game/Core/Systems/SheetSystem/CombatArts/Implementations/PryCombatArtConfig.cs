using Cysharp.Threading.Tasks;
using Game.Core.Gameplay.TBS;
using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(PryCombatArtConfig), menuName = "Game/Sheet/CombatArts/" + nameof(PryCombatArtConfig) ) ]
    public sealed class PryCombatArtConfig : CombatArtConfig
    {
        public override Type GetComboArtType() => typeof(PryCombatArt);
    }

    public sealed class PryCombatArt : CombatArt
    {
        private readonly PryCombatArtConfig _config;
        private readonly BattleGridController _gridController;

        public PryCombatArt( PryCombatArtConfig config, BattleGridController gridController ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
        }

        public override bool ReplacesDefaultDamageReaction => true;

        public override async UniTask Apply( UnitController attacker, UnitController defender )
        {
            if ( _config.X <= 0 ) return;
            if ( !TryGetDestination( attacker, defender, out var currentCell, out var destination ) ) return;

            await defender.Motion.FlyBack( currentCell, destination, _config.X );

            if ( !_gridController.Registry.TryGetUnitCell( defender, out var registeredCell ) ) return;
            if ( registeredCell != currentCell ) return;
            if ( _gridController.Registry.IsOccupied( destination ) ) return;

            _gridController.Registry.Move( defender, destination );
        }

        private bool TryGetDestination( UnitController attacker, UnitController defender, out BattleCell currentCell, out BattleCell destination )
        {
            currentCell = null;
            destination = null;

            if ( !_gridController.Registry.TryGetUnitCell( attacker, out var attackerCell ) ) return false;
            if ( !_gridController.Registry.TryGetUnitCell( defender, out var defenderCell ) ) return false;

            var direction = GetPushDirection( attackerCell, defenderCell );
            if ( direction.x == 0 && direction.z == 0 ) return false;

            var defenderPosition = defenderCell.View.Position;
            var targetPosition = new GridPosition( defenderPosition.X + direction.x * _config.X, defenderPosition.Y, defenderPosition.Z + direction.z * _config.X );

            if ( !TryGetDestinationView( defenderCell.Grid, targetPosition, defenderPosition.Y, out var destinationView ) ) return false;

            destination = _gridController.GetCell( destinationView );
            if ( _gridController.Registry.IsOccupied( destination ) )
            {
                destination = null;
                return false;
            }

            currentCell = defenderCell;
            return true;
        }

        private static bool TryGetDestinationView( GridObject grid, GridPosition targetPosition, int maxHeight, out GridCellObject destinationView )
        {
            destinationView = null;

            if ( grid == null ) return false;
            if ( grid.TryGetCell( targetPosition, out var sameHeightCell ) )
            {
                destinationView = sameHeightCell;
                return true;
            }

            foreach ( var cell in grid.Cells )
            {
                if ( cell == null ) continue;
                if ( cell.Position.X != targetPosition.X ) continue;
                if ( cell.Position.Z != targetPosition.Z ) continue;
                if ( cell.Position.Y > maxHeight ) continue;
                if ( destinationView != null && cell.Position.Y <= destinationView.Position.Y ) continue;

                destinationView = cell;
            }

            return destinationView != null;
        }

        private static Vector3Int GetPushDirection( BattleCell attackerCell, BattleCell defenderCell )
        {
            var attackerPosition = attackerCell.View.Position;
            var defenderPosition = defenderCell.View.Position;

            return new Vector3Int( Math.Sign( defenderPosition.X - attackerPosition.X ), 0, Math.Sign( defenderPosition.Z - attackerPosition.Z ) );
        }
    }
}
