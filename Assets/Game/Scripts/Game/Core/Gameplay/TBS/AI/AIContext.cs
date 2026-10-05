using Cysharp.Threading.Tasks;
using Game.Core.Gameplay.TBS.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core.Gameplay.TBS
{
    public sealed class AIContext
    {
        public Battle Battle { get; }
        public UnitController Unit { get; }
        public BattleGridController GridController { get; }
        public BattlePathfinder Pathfinder { get; }

        private readonly BattleActionController _actionController;
        private readonly BattleActionFactory _actionFactory;

        public AIContext(
            Battle battle,
            UnitController unit,
            BattleGridController gridController,
            BattlePathfinder pathfinder,
            BattleActionController actionController,
            BattleActionFactory actionFactory
            )
        {
            Battle = battle ?? throw new ArgumentNullException( nameof(battle) );
            Unit = unit ?? throw new ArgumentNullException( nameof(unit) );
            GridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            Pathfinder = pathfinder ?? throw new ArgumentNullException( nameof(pathfinder) );
            _actionController = actionController ?? throw new ArgumentNullException( nameof(actionController) );
            _actionFactory = actionFactory ?? throw new ArgumentNullException( nameof(actionFactory) );
        }

        public bool TryGetUnitCell( UnitController unit, out BattleCell cell )
        {
            return GridController.Registry.TryGetUnitCell( unit, out cell );
        }

        public IReadOnlyList< UnitController > GetAlivePlayerUnits()
        {
            var playerTeam = Battle.Teams.FirstOrDefault( x => string.Equals( x.Id, BattleParams.PLAYER_TEAM, StringComparison.InvariantCultureIgnoreCase ) );
            if ( playerTeam == null ) return Array.Empty< UnitController >();

            return playerTeam.Units
                .Where( x => x.IsAlive() )
                .Where( x => TryGetUnitCell( x, out _ ) )
                .ToArray();
        }

        public IReadOnlyList< UnitController > GetAliveEnemyUnits()
        {
            var unitTeam = Battle.GetTeam( Unit );
            if ( unitTeam == null ) return Array.Empty< UnitController >();

            return Battle.Teams
                .Where( x => x != unitTeam )
                .SelectMany( x => x.Units )
                .Where( x => x.IsAlive() )
                .Where( x => TryGetUnitCell( x, out _ ) )
                .ToArray();
        }

        public IReadOnlyList< BattleCell > GetReachableMoveCells()
        {
            if ( !TryGetUnitCell( Unit, out var startCell ) ) return Array.Empty< BattleCell >();
            if ( !Unit.CanMove ) return Array.Empty< BattleCell >();

            return Pathfinder.GetReachableCells( startCell, Unit, Unit.RemainingMovePoints )
                .Where( x => !GridController.Registry.IsOccupied( x ) )
                .ToArray();
        }

        public async UniTask< bool > TryMove( BattleCell cell )
        {
            var action = _actionFactory.Create< MoveAction >( Unit );
            var target = new BattleActionTarget( cell );

            return await _actionController.TryExecuteAction( action, target );
        }

        public async UniTask< bool > TryAttack( UnitController targetUnit )
        {
            if ( !TryGetUnitCell( targetUnit, out var targetCell ) ) return false;

            var action = _actionFactory.Create< AttackAction >( Unit );
            var target = new BattleActionTarget( targetCell, targetUnit );

            return await _actionController.TryExecuteAction( action, target );
        }

        public void CompleteUnitTurn()
        {
            var turn = Battle.CurrentRound?.CurrentTurn;
            if ( turn?.ActiveUnit != Unit ) return;

            turn.CompleteActiveUnit();
        }
    }
}
