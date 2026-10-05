using Game.Core.Systems.SheetSystem;
using System;
using UnityEngine;
using Value;

namespace Game.Core.Gameplay.TBS
{
    [ Serializable ]
    public sealed class FuryRageEffectSettings : EffectSettings
    {
        public override Type GetEffectType() => typeof(FuryRageEffect);
    }

    public sealed class FuryRageEffect : Effect
    {
        private IStat _statExtraDamage;
        private AttributeModifier _statExtraDamageModifier;
        private IStat _statHit;
        private AttributeModifier _statHitModifier;

        private readonly FuryRageEffectSettings _settings;

        public FuryRageEffect( FuryRageEffectSettings settings ) : base( settings )
        {
            _settings = settings ?? throw new ArgumentNullException( nameof(settings) );
        }

        public override void Apply( Sheet sheet )
        {
            _statExtraDamage = sheet.Stats.ExtraDamage;
            _statExtraDamageModifier = new AddAttributeModifier( _settings.X );
            _statExtraDamage.AddModifier( _statExtraDamageModifier );

            _statHit = sheet.Stats.Hit;
            _statHitModifier = new AddAttributeModifier( _settings.Y );
            _statHit.AddModifier( _statHitModifier );
        }

        public override void Remove( Sheet sheet )
        {
            _statExtraDamage.RemoveModifier( _statExtraDamageModifier );
            _statExtraDamage = null;
            _statExtraDamageModifier = null;

            _statHit.RemoveModifier( _statHitModifier );
            _statHit = null;
            _statHitModifier = null;
        }
    }
}
