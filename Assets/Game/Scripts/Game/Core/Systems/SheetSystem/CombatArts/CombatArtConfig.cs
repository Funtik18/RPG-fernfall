using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    public abstract class CombatArtConfig : ScriptableObject
    {
        [ field: SerializeField ] public string UID { get; private set; }
        [ field: SerializeField ] public string Name { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public int Damage { get; private set; }
        [ field: SerializeField ] public int Hit { get; private set; }
        [ field: SerializeField ] public int Uses { get; private set; }
        [ field: SerializeField ] public bool FollowUp { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public int X { get; private set; }
        [ field: SerializeField ] public int Y { get; private set; }
        [ field: SerializeField ] public int Z { get; private set; }

        public abstract Type GetComboArtType();
    }
}