using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(ImpenetrableSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(ImpenetrableSkillConfig) ) ]
    public sealed class ImpenetrableSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(ImpenetrableSkill);
    }
    
    public sealed class ImpenetrableSkill : Skill
    {
        private AddAttributeModifier _defenseModifier;

        private readonly ImpenetrableSkillConfig _config;

        public ImpenetrableSkill( ImpenetrableSkillConfig config ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }

        public override void OnBeforeAttack( Sheet attacker, Sheet defender )
        {
            _defenseModifier ??= new AddAttributeModifier( _config.X );

            if ( !attacker.Stats.Defense.Contains( _defenseModifier ) )
            {
                attacker.Stats.Defense.AddModifier( _defenseModifier );
            }
        }

        public override void OnAfterAttack( Sheet attacker, Sheet defender )
        {
            RemoveModifier( attacker );
        }

        public override void Dispose( Sheet sheet )
        {
            RemoveModifier( sheet );
        }

        private void RemoveModifier( Sheet sheet )
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
