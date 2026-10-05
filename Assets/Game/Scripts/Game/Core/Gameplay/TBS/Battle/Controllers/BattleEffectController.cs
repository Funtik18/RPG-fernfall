using Cysharp.Threading.Tasks;
using System;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleEffectController
    {
        private Battle _battle;
        private BattleRound _round;

        private readonly BattleGridController _gridController;
        private readonly BattleCombatController _combatController;

        public BattleEffectController(
            BattleGridController gridController,
            BattleCombatController combatController
            )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _combatController = combatController ?? throw new ArgumentNullException( nameof(combatController) );
        }

        public void Initialize( Battle battle )
        {
            _battle = battle ?? throw new ArgumentNullException( nameof(battle) );

            _gridController.Registry.OnUnitPlaced += UnitPlacedHandler;
            _gridController.Registry.OnUnitMoved += UnitMovedHandler;
            _gridController.Registry.OnUnitRemoved += UnitRemovedHandler;
            _battle.OnRoundStarted += RoundStartedHandler;
            _battle.OnRoundFinished += RoundFinishedHandler;

            ApplyInitialCellEffects();
        }

        public void Dispose()
        {
            _gridController.Registry.OnUnitPlaced -= UnitPlacedHandler;
            _gridController.Registry.OnUnitMoved -= UnitMovedHandler;
            _gridController.Registry.OnUnitRemoved -= UnitRemovedHandler;

            if ( _battle != null )
            {
                _battle.OnRoundStarted -= RoundStartedHandler;
                _battle.OnRoundFinished -= RoundFinishedHandler;
                _battle = null;
            }
            
            if ( _round != null )
            {
                _round.OnUnitCompleted -= UnitCompletedHandler;
                _round = null;
            }
        }

        private void ApplyInitialCellEffects()
        {
            if ( _battle == null ) return;

            foreach ( var unit in _battle.Units )
            {
                if ( unit == null ) continue;
                if ( !_gridController.Registry.TryGetUnitCell( unit, out var cell ) ) continue;

                ApplyCellEffect( unit, cell );
                HandleDeadUnit( unit );
            }
        }

        private void UnitPlacedHandler( UnitController unit, BattleCell cell )
        {
            ApplyCellEffect( unit, cell );
            HandleDeadUnit( unit );
        }

        private void UnitMovedHandler( UnitController unit, BattleCell from, BattleCell to )
        {
            unit.Effects.RemoveCellEffects();
            ApplyCellEffect( unit, to );
            HandleDeadUnit( unit );
        }

        private void UnitRemovedHandler( UnitController unit, BattleCell cell )
        {
            unit.Effects.RemoveCellEffects();
        }

        private void RoundStartedHandler( BattleRound round )
        {
            if ( _round != null )
            {
                _round.OnUnitCompleted -= UnitCompletedHandler;
            }

            _round = round;
            if ( _round != null )
            {
                _round.OnUnitCompleted += UnitCompletedHandler;
            }

            if ( _battle == null ) return;

            foreach ( var unit in _battle.Units )
            {
                if ( unit == null ) continue;

                unit.Effects.TickRoundStarted();
                HandleDeadUnit( unit );
            }
        }

        private void RoundFinishedHandler( BattleRound round )
        {
            if ( _round != round ) return;

            _round.OnUnitCompleted -= UnitCompletedHandler;
            _round = null;
        }

        private void UnitCompletedHandler( UnitController unit )
        {
            if ( unit == null ) return;

            unit.Effects.TickUnitCompleted();
            HandleDeadUnit( unit );
        }

        private void ApplyCellEffect( UnitController unit, BattleCell cell )
        {
            var definition = cell.Grid.Effects?.GetDefinition( cell.View );
            if ( definition?.Effect == null ) return;

            unit.Effects.Apply( definition.Effect );
        }

        private void HandleDeadUnit( UnitController unit )
        {
            if ( !unit.Sheet.IsDead ) return;
            if ( !unit.View.gameObject.activeInHierarchy ) return;

            _combatController.Kill( unit ).Forget();
            _battle?.TryFinishByOutcome();
        }
    }
}
