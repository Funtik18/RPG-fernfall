using System;
using Game.Core.Gameplay.TBS;
using Game.Managers.BattleManager;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(DesperationSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(DesperationSkillConfig) ) ]
    public sealed class DesperationSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof( DesperationSkill );
    }
    
    public sealed class DesperationSkill : Skill
    {
        private AddAttributeModifier _avoidModifier;
        
        private readonly DesperationSkillConfig _config;
        private readonly BattleManager _battleManager;
        
        public DesperationSkill(
            DesperationSkillConfig config,
            BattleManager battleManager
            ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _battleManager = battleManager ?? throw new ArgumentNullException( nameof(battleManager) );
        }

        public override void Apply( Sheet sheet )
        {
            _avoidModifier ??= new AddAttributeModifier( _config.X );
            Refresh( sheet );
        }

        public override void Refresh( Sheet sheet )
        {
            if ( sheet == null ) return;

            _avoidModifier ??= new AddAttributeModifier( _config.X );

            var avoid = sheet.Stats.Avoid;

            var battle = _battleManager.BattleController.Battle;
            var battleNearbyAlliesService = _battleManager.BattleController.Services.GetAs< BattleNearbyAlliesService >();
            var shouldApply = battleNearbyAlliesService.TryHasNearbyAllies( battle, sheet, out var hasNearbyAllies ) && !hasNearbyAllies;

            if ( shouldApply )
            {
                if ( !avoid.Contains( _avoidModifier ) )
                {
                    avoid.AddModifier( _avoidModifier );
                }

                return;
            }

            if ( avoid.Contains( _avoidModifier ) )
            {
                avoid.RemoveModifier( _avoidModifier );
            }
        }

        public override void Dispose( Sheet sheet )
        {
            if ( sheet == null ) return;
            if ( _avoidModifier == null ) return;

            var avoid = sheet.Stats.Avoid;
            if ( avoid.Contains( _avoidModifier ) )
            {
                avoid.RemoveModifier( _avoidModifier );
            }
        }
    }
}
