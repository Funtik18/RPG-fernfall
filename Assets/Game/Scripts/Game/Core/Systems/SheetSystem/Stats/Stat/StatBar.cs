using Value;

namespace Game.Core.Systems.SheetSystem
{
    public class StatBar : AttributeBar, IStatBar
    {
        // public override string LocalizationKey => $"{base.LocalizationKey}stats.";

        public StatBar( float value, float min, float max ) : base( value, min, max ) {}

        public void Restore()
        {
            Value = TotalValue;
        }
    }
}