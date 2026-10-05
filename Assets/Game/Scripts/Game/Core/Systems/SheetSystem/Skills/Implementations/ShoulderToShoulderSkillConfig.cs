using System;
using Game.Core.Gameplay.TBS;
using Game.Managers.BattleManager;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(ShoulderToShoulderSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(ShoulderToShoulderSkillConfig) ) ]
    public sealed class ShoulderToShoulderSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(ShoulderToShoulderSkill);
    }
    
    public sealed class ShoulderToShoulderSkill : Skill
    {
        private AddAttributeModifier _defenseModifier;

        private readonly ShoulderToShoulderSkillConfig _config;
        private readonly BattleManager _battleManager;

        public ShoulderToShoulderSkill(
            ShoulderToShoulderSkillConfig config,
            BattleManager battleManager
            ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _battleManager = battleManager ?? throw new ArgumentNullException( nameof(battleManager) );
        }

        public override void Apply( Sheet sheet )
        {
            _defenseModifier ??= new AddAttributeModifier( _config.X );
            Refresh( sheet );
        }

        public override void Refresh( Sheet sheet )
        {
            _defenseModifier ??= new AddAttributeModifier( _config.X );

            var defense = sheet.Stats.Defense;

            var battle = _battleManager.BattleController.Battle;
            var battleNearbyAlliesService = _battleManager.BattleController.Services.GetAs< BattleNearbyAlliesService >();
            var shouldApply = battleNearbyAlliesService.TryHasNearbyAllies( battle, sheet, out var hasNearbyAllies ) && hasNearbyAllies;

            if ( shouldApply )
            {
                if ( !defense.Contains( _defenseModifier ) )
                {
                    defense.AddModifier( _defenseModifier );
                }

                return;
            }

            if ( defense.Contains( _defenseModifier ) )
            {
                defense.RemoveModifier( _defenseModifier );
            }
        }

        public override void Dispose( Sheet sheet )
        {
            if ( _defenseModifier == null ) return;

            var defense = sheet.Stats.Defense;
            if ( defense.Contains( _defenseModifier ) )
            {
                defense.RemoveModifier( _defenseModifier );
            }
        }
    }
}
