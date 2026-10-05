using Game.Core.Systems.SheetSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleFearController
    {
        private Battle _battle;

        private FearEffectSettings FearEffect => _config.Effects.Get< FearEffectSettings >();
        private bool HasFearEffect( UnitController unit ) => unit.Effects.GetEffect( FearEffect ) != null;

        private readonly Dictionary< UnitController, Action > _fearChangedHandlers = new();
        private readonly HashSet< UnitController > _lowHealthTriggeredUnits = new();
        private readonly List< UnitController > _nearAllies = new();
        private readonly List< UnitController > _farAllies = new();
        
        private readonly BattleFearConfig _config;
        private readonly BattleCombatController _combatController;
        private readonly BattleNearbyAlliesService _nearbyAlliesService;
        private readonly BattleFuryController _furyController;

        public BattleFearController(
            TBSGameplayConfig gameplayConfig,
            BattleCombatController combatController,
            BattleNearbyAlliesService nearbyAlliesService,
            BattleFuryController furyController
            )
        {
            _config = gameplayConfig.Fear;
            _combatController = combatController ?? throw new ArgumentNullException( nameof(combatController) );
            _nearbyAlliesService = nearbyAlliesService ?? throw new ArgumentNullException( nameof(nearbyAlliesService) );
            _furyController = furyController ?? throw new ArgumentNullException( nameof(furyController) );
        }

        public void Initialize( Battle battle )
        {
            Dispose();

            _battle = battle;

            foreach ( var unit in _battle.Units )
            {
                ResetFearValue( unit );
                ObserveFear( unit );
            }

            _combatController.OnDamageApplied += DamageAppliedHandler;
            _combatController.OnUnitKilled += UnitKilledHandler;
            _furyController.OnFuryApplied += FuryAppliedHandler;
        }

        public void Dispose()
        {
            _combatController.OnDamageApplied -= DamageAppliedHandler;
            _combatController.OnUnitKilled -= UnitKilledHandler;
            _furyController.OnFuryApplied -= FuryAppliedHandler;
            _battle = null;

            foreach ( var pair in new Dictionary< UnitController, Action >( _fearChangedHandlers ) )
            {
                RemoveFearState( pair.Key );

                pair.Key.Sheet.Stats.FearPoints.OnChanged -= pair.Value;
            }

            _fearChangedHandlers.Clear();
            _lowHealthTriggeredUnits.Clear();
            _nearAllies.Clear();
            _farAllies.Clear();
        }

        public bool IsEscaping( UnitController unit )
        {
            return unit.Sheet.Fraction.IsFeelFear && unit.IsAlive() && unit.Sheet.Stats.FearPoints.Value >= _config.EscapeThreshold;
        }

        public bool IsFearing( UnitController unit )
        {
            if ( !unit.Sheet.Fraction.IsFeelFear ) return false;
            if ( !unit.IsAlive() ) return false;

            var fear = unit.Sheet.Stats.FearPoints.Value;
            return fear >= _config.FearThreshold && fear < _config.EscapeThreshold;
        }

        private bool HasFearThreshold( UnitController unit )
        {
            return unit.Sheet.Fraction.IsFeelFear && unit.IsAlive() && unit.Sheet.Stats.FearPoints.Value >= _config.FearThreshold;
        }

        private void ObserveFear( UnitController unit )
        {
            if ( _fearChangedHandlers.ContainsKey( unit ) ) return;

            void FearPointsChangedHandler()
            {
                UpdateFearState( unit );
            }

            _fearChangedHandlers.Add( unit, FearPointsChangedHandler );
            unit.Sheet.Stats.FearPoints.OnChanged += FearPointsChangedHandler;

            UpdateFearState( unit );
        }

        public void AddFear( UnitController unit, int amount )
        {
            if ( amount <= 0 ) return;
            if ( !unit.Sheet.Fraction.IsFeelFear ) return;
            if ( IsEscaping( unit ) ) return;
            if ( !unit.IsInBattle() ) return;

            var fear = unit.Sheet.Stats.FearPoints;
            fear.Value = Mathf.Min( fear.Value + amount, _config.EscapeThreshold );
        }

        private void ResetFearValue( UnitController unit )
        {
            unit.Sheet.Stats.FearPoints.Value = unit.Sheet.Stats.FearPoints.MinValue;
        }

        private void UpdateFearState( UnitController unit )
        {
            if ( HasFearThreshold( unit ) )
            {
                ApplyFearState( unit );
                return;
            }

            RemoveFearState( unit );
        }

        private void ApplyFearState( UnitController unit )
        {
            if ( HasFearEffect( unit ) ) return;

            unit.Effects.Apply( FearEffect );
        }

        private void RemoveFearState( UnitController unit )
        {
            if ( !HasFearEffect( unit ) ) return;

            unit.Effects.Remove( FearEffect );
        }

        private void UnitKilledHandler( UnitController attacker, UnitController killedUnit )
        {
            AddAllyDeathFear( killedUnit );
            ResetFearValue( killedUnit );
        }

        private void AddAllyDeathFear( UnitController killedUnit )
        {
            if ( _nearbyAlliesService.TryGetAlliesWithinTiles( _battle, killedUnit, _config.AllyDyingNearRange, _nearAllies, true ) )
            {
                foreach ( var ally in _nearAllies )
                {
                    AddFear( ally, _config.AllyDyingNear );
                }
            }

            if ( !_nearbyAlliesService.TryGetAlliesFromTiles( _battle, killedUnit, _config.AllyDyingFarRange, _farAllies, true ) )
            {
                return;
            }

            foreach ( var ally in _farAllies )
            {
                if ( _nearAllies.Contains( ally ) ) continue;

                AddFear( ally, _config.AllyDyingFar );
            }
        }

        private void DamageAppliedHandler( UnitController attacker, UnitController defender, float previousHealthPercent, float currentHealthPercent )
        {
            AddBeesDamageFear( attacker, defender );

            if ( _lowHealthTriggeredUnits.Contains( defender ) ) return;

            var lowHealthPercent = _config.LowHealthPercent / 100f;
            if ( previousHealthPercent < lowHealthPercent ) return;
            if ( currentHealthPercent >= lowHealthPercent ) return;

            _lowHealthTriggeredUnits.Add( defender );
            AddFear( defender, _config.LowHealth );
        }

        private void FuryAppliedHandler( UnitController furiousUnit )
        {
            if ( furiousUnit == null ) return;
            if ( _battle == null ) return;

            var furiousTeam = _battle.GetTeam( furiousUnit );
            if ( furiousTeam == null ) return;

            foreach ( var unit in _battle.Units )
            {
                if ( furiousTeam.Contains( unit ) ) continue;

                AddFear( unit, _config.EnemyFurious );
            }
        }

        private void AddBeesDamageFear( UnitController attacker, UnitController defender )
        {
            if ( !attacker.IsBeesWeaponEquipped() ) return;

            AddFear( defender, _config.BeesDamage );
        }
    }
}
