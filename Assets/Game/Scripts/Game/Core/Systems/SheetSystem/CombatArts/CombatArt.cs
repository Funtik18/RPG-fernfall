using Cysharp.Threading.Tasks;
using Game.Core.Gameplay.TBS;
using System;
using Value;

namespace Game.Core.Systems.SheetSystem
{
    public abstract class CombatArt
    {
        public CombatArtConfig Config { get; }

        public virtual bool ReplacesDefaultDamageReaction => false;
        
        private AddAttributeModifier _damageModifier;
        private AddAttributeModifier _hitModifier;
        
        protected CombatArt( CombatArtConfig config )
        {
            Config = config ?? throw new ArgumentNullException( nameof(config) );
        }
        
        public virtual UniTask Apply( UnitController attacker, UnitController defender )
        {
            return UniTask.CompletedTask;
        }

        public virtual void OnBeforeAttack( UnitController attacker, UnitController defender )
        {
            _damageModifier = new AddAttributeModifier( Config.Damage );
            attacker.Sheet.Stats.ExtraDamage.AddModifier( _damageModifier );
            
            _hitModifier = new AddAttributeModifier( Config.Hit );
            attacker.Sheet.Stats.Hit.AddModifier( _damageModifier );
        }

        public virtual void OnAfterAttack( UnitController attacker, UnitController defender )
        {
            attacker.Sheet.Stats.ExtraDamage.RemoveModifier( _damageModifier );
            attacker.Sheet.Stats.Hit.RemoveModifier( _hitModifier );
        }
    }
}
