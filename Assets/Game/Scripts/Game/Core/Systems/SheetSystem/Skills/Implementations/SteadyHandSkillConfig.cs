using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(SteadyHandSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(SteadyHandSkillConfig) ) ]
    public sealed class SteadyHandSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(SteadyHandSkill);
    }
    
    public sealed class SteadyHandSkill : Skill
    {
        public SteadyHandSkill( SteadyHandSkillConfig config ) : base( config )
        {
        }
    }
}