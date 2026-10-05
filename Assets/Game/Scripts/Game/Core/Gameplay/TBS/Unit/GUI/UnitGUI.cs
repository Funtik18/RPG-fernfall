using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitGUI : MonoBehaviour
    {
        [ field: SerializeField ] public UnitUIBar HealthBar { get; private set; }
        [ field: SerializeField ] public UnitUIBar FuryBar { get; private set; }
        [ field: SerializeField ] public UnitUIBar FearBar { get; private set; }
    }
}
