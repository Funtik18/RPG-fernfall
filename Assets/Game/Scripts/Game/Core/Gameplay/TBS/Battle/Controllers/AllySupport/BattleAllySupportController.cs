using Game.Core.Systems.SheetSystem;
using System;
using System.Collections.Generic;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleAllySupportController
    {
        private readonly Dictionary< UnitController, Effect > _effects = new();

        private readonly BattleAllySupportConfig _config;
        private readonly BattleGridController _gridController;
        private readonly BattleNearbyAlliesService _nearbyAlliesService;

        private Battle _battle;

        public BattleAllySupportController(
            TBSGameplayConfig config,
            BattleGridController gridController,
            BattleNearbyAlliesService nearbyAlliesService
            )
        {
            _config = config.AllySupport;
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _nearbyAlliesService = nearbyAlliesService ?? throw new ArgumentNullException( nameof(nearbyAlliesService) );
        }

        public void Initialize( Battle battle )
        {
            Dispose();

            _battle = battle ?? throw new ArgumentNullException( nameof(battle) );

            _battle.OnStarted += BattleStartedHandler;
            _gridController.Registry.OnUnitPlaced += UnitPlacedHandler;
            _gridController.Registry.OnUnitMoved += UnitMovedHandler;
            _gridController.Registry.OnUnitRemoved += UnitRemovedHandler;

            RefreshAll();
        }

        public void Dispose()
        {
            _gridController.Registry.OnUnitPlaced -= UnitPlacedHandler;
            _gridController.Registry.OnUnitMoved -= UnitMovedHandler;
            _gridController.Registry.OnUnitRemoved -= UnitRemovedHandler;

            if ( _battle != null )
            {
                _battle.OnStarted -= BattleStartedHandler;
                _battle = null;
            }

            foreach ( var unit in new List< UnitController >( _effects.Keys ) )
            {
                Remove( unit );
            }
        }
        
        private void Refresh( UnitController unit )
        {
            if ( unit == null )
            {
                return;
            }

            var shouldApply = _nearbyAlliesService.TryHasNearbyAllies( _battle, unit, out var hasNearbyAllies ) && hasNearbyAllies;
            if ( shouldApply )
            {
                Apply( unit );
                return;
            }

            Remove( unit );
        }

        private void Apply( UnitController unit )
        {
            if ( _effects.ContainsKey( unit ) ) return;

            _effects.Add( unit, unit.Effects.Apply( _config.EffectSettings ) );
        }

        private void Remove( UnitController unit )
        {
            if ( !_effects.Remove( unit, out var effect ) ) return;

            unit.Effects.Remove( effect );
        }
        
        private void RefreshAll()
        {
            foreach ( var unit in _battle.Units )
            {
                Refresh( unit );
            }
        }

        private void BattleStartedHandler()
        {
            RefreshAll();
        }

        private void UnitPlacedHandler( UnitController unit, BattleCell cell )
        {
            RefreshAll();
        }

        private void UnitMovedHandler( UnitController unit, BattleCell from, BattleCell to )
        {
            RefreshAll();
        }

        private void UnitRemovedHandler( UnitController unit, BattleCell cell )
        {
            RefreshAll();
        }
    }
}
