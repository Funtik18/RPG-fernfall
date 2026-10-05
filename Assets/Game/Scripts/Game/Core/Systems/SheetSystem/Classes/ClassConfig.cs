using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = "ClassConfig", menuName = "Game/Sheet/ClassConfig" ) ]
    public sealed class ClassConfig : ScriptableObject
    {
        [ field: SerializeField ] public string UID { get; private set; }
        [ field: SerializeField ] public string Name { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public bool IsCanCounterattack { get; private set; } = true;
        [ field: SerializeField ] public bool IsCanFollowUpAttack { get; private set; } = true;
        [ field: SerializeField ] public StatsSettings Stats { get; private set; }
        [ field: SerializeField ] public SkillsSettings Skills { get; private set; }
        [ field: SerializeField ] public WeaponFamilyConfig WeaponsFamily { get; private set; }
    }
}
