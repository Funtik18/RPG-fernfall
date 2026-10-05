using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(DrillProficiencySkillConfig), menuName = "Game/Sheet/Skills/" + nameof(DrillProficiencySkillConfig) ) ]
    public sealed class DrillProficiencySkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(DrillProficiencySkill);
    }
    
    public sealed class DrillProficiencySkill : Skill
    {
        private AddAttributeModifier _hitModifier;

        private readonly DrillProficiencySkillConfig _config;

        public DrillProficiencySkill( DrillProficiencySkillConfig config ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }

        public override void Apply( Sheet sheet )
        {
            _hitModifier ??= new AddAttributeModifier( _config.X );

            if ( !sheet.Stats.Hit.Contains( _hitModifier ) )
            {
                sheet.Stats.Hit.AddModifier( _hitModifier );
            }
        }

        public override void Dispose( Sheet sheet )
        {
            if ( _hitModifier == null ) return;

            if ( sheet.Stats.Hit.Contains( _hitModifier ) )
            {
                sheet.Stats.Hit.RemoveModifier( _hitModifier );
            }
        }
    }
}
