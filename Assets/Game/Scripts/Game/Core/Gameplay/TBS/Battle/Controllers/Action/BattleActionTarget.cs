using Game.Core.Systems.SheetSystem;

namespace Game.Core.Gameplay.TBS
{
    public readonly struct BattleActionTarget
    {
        public BattleCell Cell { get; }
        public UnitController Unit { get; }
        public CombatArtConfig CombatArt { get; }

        public BattleActionTarget(
            BattleCell cell,
            UnitController unit = null,
            CombatArtConfig combatArt = null
            )
        {
            Cell = cell;
            Unit = unit;
            CombatArt = combatArt;
        }
    }
}
