using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(DistancingSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(DistancingSkillConfig) ) ]
    public sealed class DistancingSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(DistancingSkill);
    }
    
    public sealed class DistancingSkill : Skill
    {
        private AddAttributeModifier _avoidModifier;

        private readonly DistancingSkillConfig _config;

        public DistancingSkill( DistancingSkillConfig config ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }

        public override void OnBeforeHit( Sheet attacker, Sheet defender, bool defenderCanCounterattack, bool attackedFromDistance )
        {
            if ( !attackedFromDistance ) return;
            if ( defender == null ) return;

            _avoidModifier ??= new AddAttributeModifier( _config.X );

            if ( !defender.Stats.Avoid.Contains( _avoidModifier ) )
            {
                defender.Stats.Avoid.AddModifier( _avoidModifier );
            }
        }

        public override void OnAfterHit( Sheet attacker, Sheet defender, bool isHit )
        {
            RemoveModifier( defender );
        }

        public override void Dispose( Sheet sheet )
        {
            RemoveModifier( sheet );
        }

        private void RemoveModifier( Sheet sheet )
        {
            if ( _avoidModifier == null ) return;

            if ( sheet.Stats.Avoid.Contains( _avoidModifier ) )
            {
                sheet.Stats.Avoid.RemoveModifier( _avoidModifier );
            }
        }
    }
}
