using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(SturdyFellowSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(SturdyFellowSkillConfig) ) ]
    public sealed class SturdyFellowSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(SturdyFellowSkill);
    }
    
    public sealed class SturdyFellowSkill : Skill
    {
        private AddAttributeModifier _defenseModifier;

        private readonly SturdyFellowSkillConfig _config;

        public SturdyFellowSkill( SturdyFellowSkillConfig config ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }

        public override void Apply( Sheet sheet )
        {
            if ( sheet == null ) return;

            _defenseModifier ??= new AddAttributeModifier( _config.X );

            if ( !sheet.Stats.Defense.Contains( _defenseModifier ) )
            {
                sheet.Stats.Defense.AddModifier( _defenseModifier );
            }
        }

        public override void Dispose( Sheet sheet )
        {
            if ( sheet == null ) return;
            if ( _defenseModifier == null ) return;

            if ( sheet.Stats.Defense.Contains( _defenseModifier ) )
            {
                sheet.Stats.Defense.RemoveModifier( _defenseModifier );
            }
        }
    }
}
