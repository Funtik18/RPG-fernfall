using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = nameof(ImmovableSkillConfig), menuName = "Game/Sheet/Skills/" + nameof(ImmovableSkillConfig) ) ]
    public sealed class ImmovableSkillConfig : SkillConfig
    {
        public override Type GetSkillType() => typeof(ImmovableSkill);
    }
    
    public sealed class ImmovableSkill : Skill
    {
        public ImmovableSkill( ImmovableSkillConfig config ) : base( config )
        {
        }
    }
}