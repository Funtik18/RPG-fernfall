using System;
using System.Collections.Generic;
using Game.Core.Gameplay.TBS;
using Game.Managers.BattleManager;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(HoldTheLineSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(HoldTheLineSkillConfig) ) ]
    public sealed class HoldTheLineSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(HoldTheLineSkill);
    }
    
    public sealed class HoldTheLineSkill : Skill
    {
        private readonly List< UnitController > _alliesInRange = new();
        private readonly List< UnitController > _unitsToRemove = new();
        private readonly Dictionary< UnitController, AddAttributeModifier > _strengthModifiers = new();

        private readonly HoldTheLineSkillConfig _config;
        private readonly BattleManager _battleManager;

        public HoldTheLineSkill(
            HoldTheLineSkillConfig config,
            BattleManager battleManager
            ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _battleManager = battleManager ?? throw new ArgumentNullException( nameof(battleManager) );
        }

        public override void Apply( Sheet sheet )
        {
            Refresh( sheet );
        }

        public override void Refresh( Sheet sheet )
        {
            var battleController = _battleManager.BattleController;
            var battle = battleController?.Battle;
            if ( battle == null )
            {
                RemoveAllModifiers();
                return;
            }

            var nearbyAlliesService = battleController.Services.GetAs< BattleNearbyAlliesService >();
            if ( !nearbyAlliesService.TryGetAlliesWithinTiles( battle, sheet, _config.X, _alliesInRange ) )
            {
                RemoveAllModifiers();
                return;
            }

            _unitsToRemove.Clear();
            foreach ( var pair in _strengthModifiers )
            {
                if ( !_alliesInRange.Contains( pair.Key ) )
                {
                    _unitsToRemove.Add( pair.Key );
                }
            }

            foreach ( var unit in _unitsToRemove )
            {
                RemoveModifier( unit );
            }

            foreach ( var ally in _alliesInRange )
            {
                ApplyModifier( ally );
            }
        }

        public override void Dispose( Sheet sheet )
        {
            RemoveAllModifiers();
            _alliesInRange.Clear();
            _unitsToRemove.Clear();
        }

        private void ApplyModifier( UnitController unit )
        {
            if ( unit?.Sheet == null ) return;

            if ( !_strengthModifiers.TryGetValue( unit, out var modifier ) )
            {
                modifier = new AddAttributeModifier( _config.Y );
                _strengthModifiers.Add( unit, modifier );
            }

            var strength = unit.Sheet.Stats.Strength;
            if ( !strength.Contains( modifier ) )
            {
                strength.AddModifier( modifier );
            }
        }

        private void RemoveModifier( UnitController unit )
        {
            if ( !_strengthModifiers.Remove( unit, out var modifier ) ) return;
            if ( unit?.Sheet == null ) return;

            var strength = unit.Sheet.Stats.Strength;
            if ( strength.Contains( modifier ) )
            {
                strength.RemoveModifier( modifier );
            }
        }

        private void RemoveAllModifiers()
        {
            foreach ( var pair in _strengthModifiers )
            {
                var unit = pair.Key;
                var modifier = pair.Value;
                if ( unit?.Sheet == null ) continue;

                var strength = unit.Sheet.Stats.Strength;
                if ( strength.Contains( modifier ) )
                {
                    strength.RemoveModifier( modifier );
                }
            }

            _strengthModifiers.Clear();
        }
    }
}
