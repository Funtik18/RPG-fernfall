using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ System.Serializable ]
    public sealed class StatsSettings
    {
        [ field: SerializeField ] public int Level { get; private set; } = 1;
        [ field: Space ]
        [ field: SerializeField ] public int HealthPoints { get; private set; } = 1;
        [ field: Space ]
        [ field: SerializeField ] public int Strength { get; private set; } = 1;
        [ field: SerializeField ] public int Dexterity { get; private set; } = 1;
        [ field: SerializeField ] public int Speed { get; private set; } = 1;
        [ field: SerializeField ] public int Luck { get; private set; } = 1;
        [ field: Space ]
        [ field: SerializeField ] public int Defense { get; private set; } = 0;
        [ field: SerializeField ] public int DefenseIgnore { get; private set; } = 0;
        [ field: SerializeField ] public int Resist { get; private set; } = 0;
        [ field: SerializeField ] public int ResistIgnore { get; private set; } = 0;
        [ field: SerializeField ] public int Craft { get; private set; } = 0;
        [ field: SerializeField ] public int Avoid { get; private set; } = 0;
        [ field: SerializeField ] public int Hit { get; private set; } = 0;
        [ field: SerializeField ] public int Critical { get; private set; } = 0;
        [ field: Space ]
        [ field: SerializeField ] public int MoveRange { get; private set; } = 1;
    }
}
