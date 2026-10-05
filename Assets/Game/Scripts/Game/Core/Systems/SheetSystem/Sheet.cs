using System;

namespace Game.Core.Systems.SheetSystem
{
    public sealed class Sheet
    {
        public Information Information { get; }
        public FractionConfig Fraction { get; }
        public ClassConfig Class { get; }
        public Stats Stats { get; }
        public Inventory Inventory { get; }
        public Equipment Equipment { get; }

        public bool IsDead => Stats.HealthPoints.Value <= Stats.HealthPoints.MinValue;

        public Sheet(
            Information information,
            FractionConfig fraction,
            ClassConfig classConfig,
            Stats stats,
            Inventory inventory,
            Equipment equipment
            )
        {
            Information = information ?? throw new ArgumentNullException( nameof(information) );
            Fraction = fraction ?? throw new ArgumentNullException( nameof(fraction) );
            Class = classConfig ?? throw new ArgumentNullException( nameof(classConfig) );
            Stats = stats ?? throw new ArgumentNullException( nameof(stats) );
            Inventory = inventory ?? throw new ArgumentNullException( nameof(inventory) );
            Equipment = equipment ?? throw new ArgumentNullException( nameof(equipment) );
        }

        public int GetWeight()
        {
            int weight = 0;
            foreach ( var item in Inventory.Items )
            {
                weight += item.Config.Weight;
            }
            
            return weight;
        }
    }
}
