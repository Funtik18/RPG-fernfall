using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(HeavyArmorSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(HeavyArmorSkillConfig) ) ]
    public sealed class HeavyArmorSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof( HeavyArmorSkill );
    }
    
    public sealed class HeavyArmorSkill : Skill
    {
        private AddAttributeModifier _defenseModifier;

        private readonly HeavyArmorSkillConfig _config;

        public HeavyArmorSkill( HeavyArmorSkillConfig config ) : base( config )
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
