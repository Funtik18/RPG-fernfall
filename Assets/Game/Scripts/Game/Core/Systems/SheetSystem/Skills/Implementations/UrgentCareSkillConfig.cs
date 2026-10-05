using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(UrgentCareSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(UrgentCareSkillConfig) ) ]
    public sealed class UrgentCareSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(UrgentCareSkill);
    }
    
    public sealed class UrgentCareSkill : Skill
    {
        private readonly UrgentCareSkillConfig _config;

        public UrgentCareSkill( UrgentCareSkillConfig config ) : base( config )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
        }

        public override void OnAfterHit( Sheet attacker, Sheet defender, bool isHit )
        {
            if ( !isHit ) return;
            if ( defender.IsDead ) return;

            var healthPoints = defender.Stats.HealthPoints;
            if ( healthPoints.Value >= healthPoints.TotalValue ) return;
            if ( healthPoints.PercentValue > Mathf.Clamp01( _config.Y / 100f ) ) return;
            if ( _config.X <= 0 ) return;

            healthPoints.Value += _config.X;
        }
    }
}
