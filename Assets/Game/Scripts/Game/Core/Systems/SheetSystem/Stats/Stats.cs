namespace Game.Core.Systems.SheetSystem
{
    public sealed class Stats
    {
        public IStat Level { get; }
        public IStatBar Experience { get; }
        
        public IStatBar HealthPoints { get; }
        public IStatBar RagePoints { get; }
        public IStatBar FearPoints { get; }
        
        public IStat Strength { get; }
        public IStat Dexterity { get; }
        public IStat Speed { get; }
        public IStat Luck{ get; }

        public IStat Hit { get; }
        public IStat Craft{ get; }
        public IStat Critical { get; }
        public IStat Avoid { get; }
        
        public IStat Defense { get; }
        public IStat DefenseIgnore { get; }
        public IStat Resist { get; }
        public IStat ResistIgnore { get; }
        
        public IStat MovePoints { get; }
        //левел
        //экспиренс
        
        public IStat ExtraDamage { get; }
        
        public Stats( StatsSettings settigns )
        {
            Level = new Stat( settigns.Level );
            Experience = new StatBar( 0, 0, 100 );
            
            HealthPoints = new StatBar( settigns.HealthPoints, 0, settigns.HealthPoints );
            RagePoints = new StatBar( 0, 0, 100 );
            FearPoints = new StatBar( 0, 0, 100 );
            
            Strength = new Stat( settigns.Strength );
            Dexterity = new Stat( settigns.Dexterity );
            Speed = new Stat( settigns.Speed );
            Luck = new Stat( settigns.Luck );
            
            Hit = new Stat( settigns.Hit );
            Craft = new Stat( settigns.Craft );
            Critical = new Stat( settigns.Critical );
            Avoid = new Stat( settigns.Avoid );
            
            Defense = new Stat( settigns.Defense );
            DefenseIgnore = new Stat( settigns.DefenseIgnore );
            Resist = new Stat( settigns.Resist );
            ResistIgnore = new Stat( settigns.ResistIgnore );
            
            MovePoints = new Stat( settigns.MoveRange );
            ExtraDamage = new Stat( 0 );
        }
    }
}
