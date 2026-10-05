using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(VeteranSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(VeteranSkillConfig) ) ]
    public sealed class VeteranSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(VeteranSkill);
    }
    
    public sealed class VeteranSkill : Skill
    {
        private AddAttributeModifier _hitModifier;
        private AddAttributeModifier _criticalModifier;

        private readonly VeteranSkillConfig _config;

        public VeteranSkill( VeteranSkillConfig config ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }

        public override void Apply( Sheet sheet )
        {
            _hitModifier ??= new AddAttributeModifier( _config.X );
            _criticalModifier ??= new AddAttributeModifier( _config.Y );

            if ( !sheet.Stats.Hit.Contains( _hitModifier ) )
            {
                sheet.Stats.Hit.AddModifier( _hitModifier );
            }

            if ( !sheet.Stats.Critical.Contains( _criticalModifier ) )
            {
                sheet.Stats.Critical.AddModifier( _criticalModifier );
            }
        }

        public override void Dispose( Sheet sheet )
        {
            if ( _hitModifier != null && sheet.Stats.Hit.Contains( _hitModifier ) )
            {
                sheet.Stats.Hit.RemoveModifier( _hitModifier );
            }

            if ( _criticalModifier != null && sheet.Stats.Critical.Contains( _criticalModifier ) )
            {
                sheet.Stats.Critical.RemoveModifier( _criticalModifier );
            }
        }
    }
}
