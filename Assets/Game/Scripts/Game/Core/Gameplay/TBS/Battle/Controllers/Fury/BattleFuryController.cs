using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleFuryController
    {
        public event Action< UnitController > OnFuryApplied;
        public event Action< UnitController > OnFuryRemoved;

        private Battle _battle;
        private BattleRound _observedRound;
        
        private FuryRageEffectSettings RageEffect => _config.Effects.Get< FuryRageEffectSettings >();
        private FuryEffectSettings FuryEffect => _config.Effects.Get< FuryEffectSettings >();
        private FuryExhaustedEffectSettings ExhaustedEffect => _config.Effects.Get< FuryExhaustedEffectSettings >();
        
        private bool HasRageEffect( UnitController unit ) => unit.Effects.GetEffect( RageEffect ) != null;
        private bool HasFuryEffect( UnitController unit ) => unit.Effects.GetEffect( FuryEffect ) != null;
        private bool HasExhaustedEffect( UnitController unit ) => unit.Effects.GetEffect( ExhaustedEffect ) != null;

        private readonly BattleFuryConfig _config;
        private readonly Dictionary< UnitController, Action > _furyChangedHandlers = new();
        private readonly HashSet< UnitController > _lowHealthTriggeredUnits = new();
        private readonly List< UnitController > _nearAllies = new();
        private readonly List< UnitController > _farAllies = new();

        private readonly BattleGridController _gridController;
        private readonly BattleCombatController _combatController;
        private readonly BattleNearbyAlliesService _nearbyAlliesService;

        public BattleFuryController(
            TBSGameplayConfig gameplayConfig,
            BattleGridController gridController,
            BattleCombatController combatController,
            BattleNearbyAlliesService nearbyAlliesService
            )
        {
            _config = gameplayConfig.Fury;
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _combatController = combatController ?? throw new ArgumentNullException( nameof(combatController) );
            _nearbyAlliesService = nearbyAlliesService ?? throw new ArgumentNullException( nameof(nearbyAlliesService) );
        }

        public void Initialize( Battle battle )
        {
            Dispose();

            _battle = battle ?? throw new ArgumentNullException( nameof(battle) );

            foreach ( var unit in _battle.Units )
            {
                ResetFuryValue( unit );
                ObserveFury( unit );
            }

            _combatController.OnDamageApplied += DamageAppliedHandler;
            _combatController.OnUnitKilled += UnitKilledHandler;
            _battle.OnRoundStarted += RoundStartedHandler;
            _battle.OnRoundFinished += RoundFinishedHandler;
        }

        public void Dispose()
        {
            if ( _observedRound != null )
            {
                _observedRound.OnUnitCompleted -= UnitCompletedHandler;
                _observedRound = null;
            }

            if ( _battle != null )
            {
                _battle.OnRoundStarted -= RoundStartedHandler;
                _battle.OnRoundFinished -= RoundFinishedHandler;
                _battle = null;
            }

            _combatController.OnDamageApplied -= DamageAppliedHandler;
            _combatController.OnUnitKilled -= UnitKilledHandler;

            foreach ( var pair in new Dictionary< UnitController, Action >( _furyChangedHandlers ) )
            {
                RemoveFuryState( pair.Key );
                RemoveRageState( pair.Key );
                RemoveExhaustedEffect( pair.Key );

                pair.Key.Sheet.Stats.RagePoints.OnChanged -= pair.Value;
            }

            _furyChangedHandlers.Clear();
            _lowHealthTriggeredUnits.Clear();
            _nearAllies.Clear();
            _farAllies.Clear();
        }

        public bool IsFurious( UnitController unit )
        {
            return unit.Sheet.Fraction.IsFeelFury && unit.IsAlive() && unit.Sheet.Stats.RagePoints.Value >= _config.FuryThreshold;
        }

        public bool IsRaging( UnitController unit )
        {
            if ( !unit.Sheet.Fraction.IsFeelFury ) return false;
            if ( !unit.IsAlive() ) return false;

            var fury = unit.Sheet.Stats.RagePoints.Value;
            return fury >= _config.RageThreshold && fury < _config.FuryThreshold;
        }

        private void ObserveFury( UnitController unit )
        {
            if ( _furyChangedHandlers.ContainsKey( unit ) ) return;

            void RagePointsChangedHandler()
            {
                UpdateFuryState( unit );
            }

            _furyChangedHandlers.Add( unit, RagePointsChangedHandler );
            unit.Sheet.Stats.RagePoints.OnChanged += RagePointsChangedHandler;

            UpdateFuryState( unit );
        }

        public void AddFury( UnitController unit, int amount )
        {
            if ( amount <= 0 ) return;
            if ( !unit.Sheet.Fraction.IsFeelFury ) return;
            if ( IsFurious( unit ) ) return;
            if ( !unit.IsAlive() ) return;
            if ( !_gridController.Registry.TryGetUnitCell( unit, out _ ) ) return;

            var fury = unit.Sheet.Stats.RagePoints;
            fury.Value = Mathf.Min( fury.Value + amount, _config.FuryThreshold );
        }

        public void ResetFury( UnitController unit, bool applyExhaustedEffect = true )
        {
            var wasFurious = IsFurious( unit ) || HasFuryEffect( unit );
            ResetFuryValue( unit );

            if ( applyExhaustedEffect && wasFurious && unit.IsAlive() )
            {
                ApplyExhaustedEffect( unit );
            }
        }

        private void ResetFuryValue( UnitController unit )
        {
            unit.Sheet.Stats.RagePoints.Value = unit.Sheet.Stats.RagePoints.MinValue;
        }

        private void UpdateFuryState( UnitController unit )
        {
            if ( !unit.Sheet.Fraction.IsFeelFury )
            {
                RemoveFuryState( unit );
                RemoveRageState( unit );
                return;
            }

            if ( IsFurious( unit ) )
            {
                RemoveRageState( unit );
                ApplyFuryState( unit );
                return;
            }

            RemoveFuryState( unit );

            if ( IsRaging( unit ) )
            {
                ApplyRageState( unit );
                return;
            }

            RemoveRageState( unit );
        }

        private void ApplyRageState( UnitController unit )
        {
            if ( HasRageEffect( unit ) ) return;
            if ( !unit.IsAlive() ) return;
            if ( !_gridController.Registry.TryGetUnitCell( unit, out _ ) ) return;

            unit.Effects.Apply( RageEffect );
        }

        private void RemoveRageState( UnitController unit )
        {
            if ( !HasRageEffect( unit ) ) return;

            unit.Effects.Remove( RageEffect );
        }

        private void ApplyFuryState( UnitController unit )
        {
            if ( HasFuryEffect( unit ) ) return;
            if ( !unit.IsAlive() ) return;
            if ( !_gridController.Registry.TryGetUnitCell( unit, out _ ) ) return;

            RemoveExhaustedEffect( unit );
            unit.Effects.Apply( FuryEffect );

            OnFuryApplied?.Invoke( unit );
        }

        private void RemoveFuryState( UnitController unit )
        {
            if ( !HasFuryEffect( unit ) ) return;

            unit.Effects.Remove( FuryEffect );

            OnFuryRemoved?.Invoke( unit );
        }

        private void AddKillEnemyFury( UnitController attacker, UnitController killedUnit )
        {
            if ( attacker == killedUnit ) return;
            if ( _battle.GetTeam( attacker ) == _battle.GetTeam( killedUnit ) ) return;
            if ( !attacker.IsAlive() ) return;

            AddFury( attacker, _config.KillEnemy );
        }

        private void AddAllyDeathFury( UnitController killedUnit )
        {
            if ( _nearbyAlliesService.TryGetAlliesWithinTiles( _battle, killedUnit, _config.AllyDyingNearRange, _nearAllies, true ) )
            {
                foreach ( var ally in _nearAllies )
                {
                    AddFury( ally, _config.AllyDyingNear );
                }
            }

            if ( !_nearbyAlliesService.TryGetAlliesFromTiles( _battle, killedUnit, _config.AllyDyingFarRange, _farAllies, true ) )
            {
                return;
            }

            foreach ( var ally in _farAllies )
            {
                if ( _nearAllies.Contains( ally ) ) continue;

                AddFury( ally, _config.AllyDyingFar );
            }
        }

        private void ApplyExhaustedEffect( UnitController unit )
        {
            unit.Effects.Apply( ExhaustedEffect );
        }

        private void RemoveExhaustedEffect( UnitController unit )
        {
            if ( !HasExhaustedEffect( unit ) ) return;

            unit.Effects.Remove( ExhaustedEffect );
        }

        private void DamageAppliedHandler( UnitController attacker, UnitController defender, float previousHealthPercent, float currentHealthPercent )
        {
            if ( _lowHealthTriggeredUnits.Contains( defender ) ) return;

            var lowHealthPercent = PercentToNormalized( _config.LowHealthPercent );
            if ( previousHealthPercent < lowHealthPercent ) return;
            if ( currentHealthPercent >= lowHealthPercent ) return;

            _lowHealthTriggeredUnits.Add( defender );
            AddFury( defender, _config.LowHealth );
        }

        private void UnitKilledHandler( UnitController attacker, UnitController killedUnit )
        {
            AddKillEnemyFury( attacker, killedUnit );
            AddAllyDeathFury( killedUnit );
            ResetFury( killedUnit, false );
            RemoveExhaustedEffect( killedUnit );
        }

        private void RoundStartedHandler( BattleRound round )
        {
            if ( _observedRound != null )
            {
                _observedRound.OnUnitCompleted -= UnitCompletedHandler;
            }

            _observedRound = round;

            if ( _observedRound != null )
            {
                _observedRound.OnUnitCompleted += UnitCompletedHandler;
            }
        }

        private void RoundFinishedHandler( BattleRound round )
        {
            if ( _observedRound != round ) return;

            _observedRound.OnUnitCompleted -= UnitCompletedHandler;
            _observedRound = null;
        }

        private void UnitCompletedHandler( UnitController unit )
        {
            if ( !unit.IsAlive() )
            {
                ResetFury( unit, false );
                return;
            }

            if ( !IsFurious( unit ) ) return;

            if ( UnityEngine.Random.value >= PercentToNormalized( _config.FuryEndChance ) ) return;

            ResetFury( unit );
        }

        private static float PercentToNormalized( float percent ) => Mathf.Clamp01( percent / 100f );
    }
}
