using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ System.Serializable ]
    public sealed class SkillsSettings
    {
        [ field: SerializeField ] public List< SkillRequiredLevel > Skills { get; private set; } = new();
    }

    [ System.Serializable ]
    public sealed class SkillRequiredLevel
    {
        [ field: SerializeField ] public int Level { get; private set; }
        [ field: SerializeField ] public SkillConfig Skill { get; private set; }
    }
}