using System;

namespace Game.Core.Systems.SheetSystem
{
    public abstract class Skill
    {
        public SkillConfig Config { get; }
        
        protected Skill( SkillConfig config )
        {
            Config = config ?? throw new ArgumentNullException( nameof(config) );
        }
        
        public virtual void Apply( Sheet sheet ) {}

        public virtual void Refresh( Sheet sheet ) {}

        public virtual void OnBeforeAttack( Sheet attacker, Sheet defender ) {}

        public virtual void OnBeforeHit( Sheet attacker, Sheet defender, bool defenderCanCounterattack, bool attackedFromDistance ) {}

        public virtual void OnAfterHit( Sheet attacker, Sheet defender, bool isHit ) {}

        public virtual void OnAfterAttack( Sheet attacker, Sheet defender ) {}
        
        public virtual void Dispose( Sheet sheet ) {}
    }
}
