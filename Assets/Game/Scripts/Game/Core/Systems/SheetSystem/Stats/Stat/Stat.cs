using Value;

namespace Game.Core.Systems.SheetSystem
{
    public class Stat : Attribute, IStat
    {
        // public override string LocalizationKey => $"{base.LocalizationKey}stats.";

        public Stat( float value ) : base( value ) {}
    }
}