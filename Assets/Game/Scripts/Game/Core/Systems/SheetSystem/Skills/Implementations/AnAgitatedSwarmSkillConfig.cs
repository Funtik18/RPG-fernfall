using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(AnAgitatedSwarmSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(AnAgitatedSwarmSkillConfig) ) ]
    public sealed class AnAgitatedSwarmSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(AnAgitatedSwarmSkill);
    }
    
    public sealed class AnAgitatedSwarmSkill : Skill
    {
        public AnAgitatedSwarmSkill( AnAgitatedSwarmSkillConfig config ) : base( config )
        {
        }
    }
}