using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(TrueAimSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(TrueAimSkillConfig) ) ]
    public sealed class TrueAimSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(TrueAimSkill);
    }
    
    public sealed class TrueAimSkill : Skill
    {
        private AddAttributeModifier _criticalModifier;

        private readonly TrueAimSkillConfig _config;

        public TrueAimSkill( TrueAimSkillConfig config ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }

        public override void OnBeforeHit( Sheet attacker, Sheet defender, bool defenderCanCounterattack, bool attackedFromDistance )
        {
            if ( defenderCanCounterattack ) return;

            _criticalModifier ??= new AddAttributeModifier( _config.X );

            if ( !attacker.Stats.Critical.Contains( _criticalModifier ) )
            {
                attacker.Stats.Critical.AddModifier( _criticalModifier );
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
            if ( _criticalModifier == null ) return;

            if ( sheet.Stats.Critical.Contains( _criticalModifier ) )
            {
                sheet.Stats.Critical.RemoveModifier( _criticalModifier );
            }
        }
    }
}
