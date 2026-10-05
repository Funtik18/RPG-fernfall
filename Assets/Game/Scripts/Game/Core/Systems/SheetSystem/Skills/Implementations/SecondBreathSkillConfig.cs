using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(SecondBreathSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(SecondBreathSkillConfig) ) ]
    public sealed class SecondBreathSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(SecondBreathSkill);
    }
    
    public sealed class SecondBreathSkill : Skill
    {
        private AddAttributeModifier _craftModifier;

        private readonly SecondBreathSkillConfig _config;

        public SecondBreathSkill( SecondBreathSkillConfig config ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }

        public override void OnBeforeHit( Sheet attacker, Sheet defender, bool defenderCanCounterattack, bool attackedFromDistance )
        {
            if ( defender.Stats.Defense.TotalValue < _config.Y ) return;

            _craftModifier ??= new AddAttributeModifier( _config.X );

            if ( !attacker.Stats.Craft.Contains( _craftModifier ) )
            {
                attacker.Stats.Craft.AddModifier( _craftModifier );
            }
        }

        public override void OnAfterHit( Sheet attacker, Sheet defender, bool isHit )
        {
            RemoveModifier( attacker );
        }

        public override void Dispose( Sheet sheet )
        {
            RemoveModifier( sheet );
        }

        private void RemoveModifier( Sheet sheet )
        {
            if ( _craftModifier == null ) return;

            if ( sheet.Stats.Craft.Contains( _craftModifier ) )
            {
                sheet.Stats.Craft.RemoveModifier( _craftModifier );
            }
        }
    }
}
