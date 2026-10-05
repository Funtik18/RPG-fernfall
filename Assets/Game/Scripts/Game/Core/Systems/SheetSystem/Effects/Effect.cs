namespace Game.Core.Systems.SheetSystem
{
    public abstract class Effect
    {
        public IEffectSettings Settings { get; }

        public virtual bool IsExpired => false;

        public virtual bool RemoveOnCellExit => false;

        protected Effect( IEffectSettings settings )
        {
            Settings = settings ?? throw new System.ArgumentNullException( nameof(settings) );
        }

        public virtual void Apply( Sheet sheet ) {}

        public virtual void Refresh( Sheet sheet ) {}

        public virtual void TickRoundStarted( Sheet sheet ) {}

        public virtual void TickUnitCompleted( Sheet sheet ) {}

        public virtual void Remove( Sheet sheet ) {}
    }
}
