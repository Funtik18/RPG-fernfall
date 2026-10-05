using System;
using UnityEngine;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(ToTheEndSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(ToTheEndSkillConfig) ) ]
    public sealed class ToTheEndSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(ToTheEndSkill);
    }

    public sealed class ToTheEndSkill : Skill
    {
        private Sheet _sheet;
        private AddAttributeModifier _defenseModifier;

        private readonly ToTheEndSkillConfig _config;

        public ToTheEndSkill( ToTheEndSkillConfig config ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }

        public override void Apply( Sheet sheet )
        {
            _sheet = sheet;
            _defenseModifier = new AddAttributeModifier( _config.X );

            sheet.Stats.HealthPoints.OnChanged += HealthPointsChangedHandler;

            Refresh( sheet );
        }

        public override void Refresh( Sheet sheet )
        {
            _defenseModifier ??= new AddAttributeModifier( _config.X );

            if ( sheet.Stats.HealthPoints.PercentValue <= _config.Y / 100f )
            {
                if ( !sheet.Stats.Defense.Contains( _defenseModifier ) )
                {
                    sheet.Stats.Defense.AddModifier( _defenseModifier );
                }

                return;
            }

            if ( sheet.Stats.Defense.Contains( _defenseModifier ) )
            {
                sheet.Stats.Defense.RemoveModifier( _defenseModifier );
            }
        }

        public override void Dispose( Sheet sheet )
        {
            sheet.Stats.HealthPoints.OnChanged -= HealthPointsChangedHandler;
            if ( sheet.Stats.Defense.Contains( _defenseModifier ) )
            {
                sheet.Stats.Defense.RemoveModifier( _defenseModifier );
            }
        }

        private void HealthPointsChangedHandler()
        {
            Refresh( _sheet );
        }
    }
}
