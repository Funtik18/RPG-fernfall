using System;
using Game.Core.Gameplay.TBS;
using Game.Managers.BattleManager;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(GrandmaRecipeSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(GrandmaRecipeSkillConfig) ) ]
    public sealed class GrandmaRecipeSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(GrandmaRecipeSkill);
    }
    
    public sealed class GrandmaRecipeSkill : Skill
    {
        private Sheet _sheet;
        private Battle _battle;
        private UnitController _unit;
        private BattleTurn _handledTurn;

        private readonly GrandmaRecipeSkillConfig _config;
        private readonly BattleManager _battleManager;

        public GrandmaRecipeSkill(
            GrandmaRecipeSkillConfig config,
            BattleManager battleManager
            ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _battleManager = battleManager ?? throw new ArgumentNullException( nameof(battleManager) );
        }

        public override void Apply( Sheet sheet )
        {
            _sheet = sheet;
            SubscribeBattle();
        }

        public override void Refresh( Sheet sheet )
        {
            _sheet ??= sheet;
            SubscribeBattle();
        }

        public override void Dispose( Sheet sheet )
        {
            UnsubscribeBattle();
            _sheet = null;
        }

        private void SubscribeBattle()
        {
            var battle = _battleManager.BattleController?.Battle;
            if ( battle == null ) return;
            if ( _battle == battle ) return;

            UnsubscribeBattle();

            _battle = battle;
            _unit = _battle.Units.Find( unit => unit?.Sheet == _sheet );
            _battle.OnRoundChanged += RoundChangedHandler;
            RoundChangedHandler( _battle.CurrentRound );
        }

        private void UnsubscribeBattle()
        {
            if ( _battle != null )
            {
                _battle.OnRoundChanged -= RoundChangedHandler;
            }
            
            _battle = null;
            _unit = null;
            _handledTurn = null;
        }

        private void RoundChangedHandler( BattleRound round )
        {
            var turn = round?.CurrentTurn;
            if ( _sheet == null ) return;
            if ( turn == _handledTurn ) return;
            if ( turn?.Team == null ) return;

            _handledTurn = turn;

            if ( _unit == null ) return;
            if ( !turn.Team.Contains( _unit ) ) return;
            if ( _unit.Sheet.IsDead ) return;

            Heal( _unit.Sheet );
        }

        private void Heal( Sheet sheet )
        {
            var healthPoints = sheet.Stats.HealthPoints;
            if ( healthPoints.Value >= healthPoints.TotalValue ) return;

            var amount = healthPoints.TotalValue * Mathf.Clamp01( _config.X / 100f );
            if ( amount <= 0f ) return;

            healthPoints.Value += amount;
        }
    }
}
