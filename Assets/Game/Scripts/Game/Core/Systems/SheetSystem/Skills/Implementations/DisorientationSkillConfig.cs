using System;
using System.Collections.Generic;
using Game.Core.Gameplay.TBS;
using Game.Managers.BattleManager;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(DisorientationSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(DisorientationSkillConfig) ) ]
    public sealed class DisorientationSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(DisorientationSkill);
    }
    
    public sealed class DisorientationSkill : Skill
    {
        private Battle _battle;
        private BattleRound _round;
        
        private readonly Dictionary< UnitController, TargetState > _targetStates = new();
        private readonly List< UnitController > _unitsToRemove = new();
        private readonly List< BattleTurn > _subscribedTurns = new();

        private readonly DisorientationSkillConfig _config;
        private readonly BattleManager _battleManager;
        
        public DisorientationSkill(
            DisorientationSkillConfig config,
            BattleManager battleManager
            ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _battleManager = battleManager ?? throw new ArgumentNullException( nameof(battleManager) );
        }

        public override void Apply( Sheet sheet )
        {
            SubscribeBattle();
        }

        public override void Refresh( Sheet sheet )
        {
            SubscribeBattle();
        }

        public override void OnAfterHit( Sheet attacker, Sheet defender, bool isHit )
        {
            if ( !isHit ) return;
            if ( defender.IsDead ) return;
            if ( _config.X <= 0 ) return;

            SubscribeBattle();

            if ( !TryGetUnit( defender, out var defenderUnit ) ) return;

            Schedule( defenderUnit );
        }

        public override void Dispose( Sheet sheet )
        {
            RemoveAllStates();
            UnsubscribeBattle();
        }

        private void SubscribeBattle()
        {
            var battle = _battleManager.BattleController?.Battle;
            if ( battle == null ) return;
            if ( _battle == battle )
            {
                SubscribeRound( battle.CurrentRound );
                return;
            }

            UnsubscribeBattle();

            _battle = battle;
            _battle.OnRoundStarted += RoundStartedHandler;
            _battle.OnFinished += BattleFinishedHandler;

            SubscribeRound( _battle.CurrentRound );
        }

        private void UnsubscribeBattle()
        {
            UnsubscribeRound();

            if ( _battle != null )
            {
                _battle.OnRoundStarted -= RoundStartedHandler;
                _battle.OnFinished -= BattleFinishedHandler;
            }

            _battle = null;
        }

        private void RoundStartedHandler( BattleRound round )
        {
            SubscribeRound( round );
        }

        private void BattleFinishedHandler()
        {
            RemoveAllStates();
            UnsubscribeBattle();
        }

        private void SubscribeRound( BattleRound round )
        {
            if ( _round == round ) return;

            UnsubscribeRound();

            _round = round;
            _round.OnTurnStarted += TurnStartedHandler;
            _round.OnTurnFinished += TurnFinishedHandler;

            foreach ( var turn in _round.Turns )
            {
                SubscribeTurn( turn );
            }
        }

        private void UnsubscribeRound()
        {
            foreach ( var turn in _subscribedTurns )
            {
                turn.OnUnitCompleted -= UnitCompletedHandler;
            }
            _subscribedTurns.Clear();

            if ( _round != null )
            {
                _round.OnTurnStarted -= TurnStartedHandler;
                _round.OnTurnFinished -= TurnFinishedHandler;
                _round = null;
            }
        }

        private void SubscribeTurn( BattleTurn turn )
        {
            if ( _subscribedTurns.Contains( turn ) ) return;

            turn.OnUnitCompleted += UnitCompletedHandler;
            _subscribedTurns.Add( turn );
        }

        private void TurnStartedHandler( BattleTurn turn )
        {
            foreach ( var pair in _targetStates )
            {
                if ( !turn.Team.Contains( pair.Key ) ) continue;

                ApplyState( pair.Value );
            }
        }

        private void UnitCompletedHandler( UnitController unit )
        {
            RemoveActiveState( unit );
        }

        private void TurnFinishedHandler( BattleTurn turn )
        {
            _unitsToRemove.Clear();

            foreach ( var pair in _targetStates )
            {
                var state = pair.Value;
                if ( !state.IsActive ) continue;
                if ( !turn.Team.Contains( pair.Key ) ) continue;

                _unitsToRemove.Add( pair.Key );
            }

            foreach ( var unit in _unitsToRemove )
            {
                RemoveActiveState( unit );
            }
        }

        private void Schedule( UnitController unit )
        {
            if ( unit == null ) return;
            if ( _targetStates.ContainsKey( unit ) ) return;

            _targetStates.Add( unit, new TargetState( unit, new AddAttributeModifier( -_config.X ) ) );
        }

        private void ApplyState( TargetState state )
        {
            if ( state == null ) return;
            if ( state.IsActive ) return;
            if ( state.Unit?.Sheet == null ) return;
            if ( state.Unit.Sheet.IsDead ) return;

            var movePoints = state.Unit.Sheet.Stats.MovePoints;
            if ( !movePoints.Contains( state.Modifier ) )
            {
                movePoints.AddModifier( state.Modifier );
            }

            state.Unit.ClampRemainingMovePoints();
            state.IsActive = true;
        }

        private void RemoveActiveState( UnitController unit )
        {
            if ( unit == null ) return;
            if ( !_targetStates.TryGetValue( unit, out var state ) ) return;
            if ( !state.IsActive ) return;

            RemoveModifier( state );
            _targetStates.Remove( unit );
        }

        private void RemoveAllStates()
        {
            foreach ( var pair in _targetStates )
            {
                RemoveModifier( pair.Value );
            }

            _targetStates.Clear();
            _unitsToRemove.Clear();
        }

        private void RemoveModifier( TargetState state )
        {
            if ( state == null ) return;
            if ( state.Unit?.Sheet == null ) return;

            var movePoints = state.Unit.Sheet.Stats.MovePoints;
            if ( movePoints.Contains( state.Modifier ) )
            {
                movePoints.RemoveModifier( state.Modifier );
            }

            state.IsActive = false;
        }

        private bool TryGetUnit( Sheet sheet, out UnitController unit )
        {
            unit = null;
            if ( _battle == null ) return false;

            unit = _battle.Units.Find( currentUnit => currentUnit?.Sheet == sheet );
            return unit != null;
        }

        private sealed class TargetState
        {
            public UnitController Unit { get; }
            public AddAttributeModifier Modifier { get; }
            public bool IsActive { get; set; }

            public TargetState( UnitController unit, AddAttributeModifier modifier )
            {
                Unit = unit ?? throw new ArgumentNullException( nameof(unit) );
                Modifier = modifier ?? throw new ArgumentNullException( nameof(modifier) );
            }
        }
    }
}
