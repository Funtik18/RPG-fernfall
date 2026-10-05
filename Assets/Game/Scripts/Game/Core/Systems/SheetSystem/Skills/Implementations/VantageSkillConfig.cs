using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(VantageSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(VantageSkillConfig) ) ]
    public sealed class VantageSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(VantageSkill);
    }
    
    public sealed class VantageSkill : Skill
    {
        public VantageSkill( VantageSkillConfig config ) : base( config )
        {
        }
    }
}
