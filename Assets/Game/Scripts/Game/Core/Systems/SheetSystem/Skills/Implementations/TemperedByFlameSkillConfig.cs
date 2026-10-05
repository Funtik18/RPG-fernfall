using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(TemperedByFlameSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(TemperedByFlameSkillConfig) ) ]
    public sealed class TemperedByFlameSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(TemperedByFlameSkill);
    }
    
    public sealed class TemperedByFlameSkill : Skill
    {
        private AddAttributeModifier _craftModifier;

        private readonly TemperedByFlameSkillConfig _config;

        public TemperedByFlameSkill( TemperedByFlameSkillConfig config ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }

        public override void Apply( Sheet sheet )
        {
            if ( sheet == null ) return;

            _craftModifier ??= new AddAttributeModifier( _config.X );

            if ( !sheet.Stats.Craft.Contains( _craftModifier ) )
            {
                sheet.Stats.Craft.AddModifier( _craftModifier );
            }
        }

        public override void Dispose( Sheet sheet )
        {
            if ( sheet == null ) return;
            if ( _craftModifier == null ) return;

            if ( sheet.Stats.Craft.Contains( _craftModifier ) )
            {
                sheet.Stats.Craft.RemoveModifier( _craftModifier );
            }
        }
    }
}
