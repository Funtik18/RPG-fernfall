using Cysharp.Threading.Tasks;
using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleAIController
    {
        private Battle _battle;
        private bool _isExecutingTurn;

        private readonly AIBehaviourFactory _aiBehaviourFactory;
        private readonly BattleGridController _gridController;
        private readonly BattleFuryController _furyController;
        private readonly BattleFearController _fearController;
        private readonly BattleEnemyRetreatController _enemyRetreatController;

        public BattleAIController(
            AIBehaviourFactory aiBehaviourFactory,
            BattleGridController gridController,
            BattleFuryController furyController,
            BattleFearController fearController,
            BattleEnemyRetreatController enemyRetreatController
            )
        {
            _aiBehaviourFactory = aiBehaviourFactory ?? throw new ArgumentNullException( nameof(aiBehaviourFactory) );
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _furyController = furyController ?? throw new ArgumentNullException( nameof(furyController) );
            _fearController = fearController ?? throw new ArgumentNullException( nameof(fearController) );
            _enemyRetreatController = enemyRetreatController ?? throw new ArgumentNullException( nameof(enemyRetreatController) );
        }

        public void Initialize( Battle battle )
        {
            _battle = battle ?? throw new ArgumentNullException( nameof(battle) );
            _battle.OnChanged += BattleChangedHandler;

            TryExecuteAutomatedTurn().Forget();
        }

        public void Dispose()
        {
            if ( _battle == null ) return;

            _battle.OnChanged -= BattleChangedHandler;
            _battle = null;
        }

        private void BattleChangedHandler()
        {
            TryExecuteAutomatedTurn().Forget();
        }

        private async UniTask TryExecuteAutomatedTurn()
        {
            if ( _isExecutingTurn ) return;
            if ( !TryGetAutomatedTurn( out _ ) ) return;

            _isExecutingTurn = true;
            try
            {
                while ( TryGetAutomatedTurn( out var turn ) )
                {
                    await UniTask.Yield();

                    if ( !TryGetAutomatedTurn( out turn ) )
                    {
                        continue;
                    }

                    var unit = GetNextUnit( turn );
                    if ( unit == null )
                    {
                        if ( CanEndAutomatedTurn( turn ) )
                        {
                            _battle.EndCurrentTurn();
                            continue;
                        }

                        break;
                    }

                    if ( turn.ActiveUnit != null && turn.ActiveUnit != unit )
                    {
                        break;
                    }

                    if ( turn.ActiveUnit == null && !turn.TrySelectUnit( unit ) )
                    {
                        if ( CanEndAutomatedTurn( turn ) )
                        {
                            _battle.EndCurrentTurn();
                            continue;
                        }

                        break;
                    }

                    var behaviour = CreateBehaviour( unit );
                    if ( behaviour == null )
                    {
                        turn.CompleteActiveUnit();
                        continue;
                    }

                    var context = _aiBehaviourFactory.CreateContext( unit, _battle );
                    await behaviour.Execute( context );

                    if ( turn.IsActive && turn.ActiveUnit == unit )
                    {
                        turn.CompleteActiveUnit();
                    }
                }
            }
            finally
            {
                _isExecutingTurn = false;
            }
        }

        private UnitController GetNextUnit( BattleTurn turn )
        {
            if ( IsAutomatedUnit( turn, turn.ActiveUnit ) )
            {
                return turn.ActiveUnit;
            }

            foreach ( var unit in turn.Team.Units )
            {
                if ( turn.HasCompletedUnit( unit ) ) continue;
                if ( !unit.IsAlive() ) continue;
                if ( !_gridController.Registry.TryGetUnitCell( unit, out _ ) ) continue;
                if ( !IsAutomatedUnit( turn, unit ) ) continue;

                return unit;
            }

            return null;
        }

        private IAIBehaviour CreateBehaviour( UnitController unit )
        {
            if ( _fearController.IsEscaping( unit ) || _enemyRetreatController.IsRetreating( unit ) )
            {
                return _aiBehaviourFactory.Create< RetreatAIBehaviour >();
            }

            if ( _furyController.IsFurious( unit ) )
            {
                return _aiBehaviourFactory.Create< FuryAIBehaviour >();
            }

            if ( unit.Config is EnemyUnitConfig enemyUnitConfig )
            {
                return _aiBehaviourFactory.Create( enemyUnitConfig.AIBehaviour );
            }

            return null;
        }

        private bool IsAutomatedUnit( BattleTurn turn, UnitController unit )
        {
            if ( unit == null ) return false;

            if ( turn.Team.IsEnemyTeam() )
            {
                return true;
            }

            return IsPlayerAutomatedUnit( unit );
        }

        private bool CanEndAutomatedTurn( BattleTurn turn )
        {
            if ( turn.Team.IsEnemyTeam() ) return true;

            return turn?.AllUnitsCompleted ?? false;
        }

        private bool TryGetAutomatedTurn( out BattleTurn turn )
        {
            turn = null;

            if ( _battle == null ) return false;
            if ( _battle.IsFinished ) return false;

            turn = _battle.CurrentRound?.CurrentTurn;
            if ( turn == null || !turn.IsActive ) return false;

            if ( turn.Team.IsEnemyTeam() ) return true;
            if ( turn.ActiveUnit != null ) return IsPlayerAutomatedUnit( turn.ActiveUnit );

            return HasPendingAutomatedPlayerUnit( turn );
        }

        private bool IsPlayerAutomatedUnit( UnitController unit )
        {
            return _furyController.IsFurious( unit ) || _fearController.IsEscaping( unit );
        }

        private bool HasPendingAutomatedPlayerUnit( BattleTurn turn )
        {
            foreach ( var unit in turn.Team.Units )
            {
                if ( turn.HasCompletedUnit( unit ) ) continue;
                if ( !IsPlayerAutomatedUnit( unit ) ) continue;
                if ( !unit.IsAlive() ) continue;
                if ( !_gridController.Registry.TryGetUnitCell( unit, out _ ) ) continue;

                return true;
            }

            return false;
        }
    }
}
