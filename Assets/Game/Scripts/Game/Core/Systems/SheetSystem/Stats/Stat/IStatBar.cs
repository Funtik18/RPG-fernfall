using Value;

namespace Game.Core.Systems.SheetSystem
{
    public interface IStatBar : IStat, IBar
    {
        void Restore();
    }
}