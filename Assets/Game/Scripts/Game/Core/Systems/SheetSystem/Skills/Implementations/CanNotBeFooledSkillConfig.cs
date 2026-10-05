using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(CanNotBeFooledSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(CanNotBeFooledSkillConfig) ) ]
    public sealed class CanNotBeFooledSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(CanNotBeFooledSkill);
    }
    
    public sealed class CanNotBeFooledSkill : Skill
    {
        public CanNotBeFooledSkill( CanNotBeFooledSkillConfig config ) : base( config )
        {
        }
    }
}