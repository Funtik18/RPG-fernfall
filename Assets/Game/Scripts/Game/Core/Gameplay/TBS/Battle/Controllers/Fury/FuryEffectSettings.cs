using Game.Core.Systems.SheetSystem;
using System;
using UnityEngine;
using Value;

namespace Game.Core.Gameplay.TBS
{
    [ Serializable ]
    public sealed class FuryEffectSettings : EffectSettings
    {
        public override Type GetEffectType() => typeof(FuryEffect);
    }

    public sealed class FuryEffect : Effect
    {
        private IStat _statExtraDamage;
        private AttributeModifier _statExtraDamageModifier;
        private IStat _statDefence;
        private AttributeModifier _statDefenceModifier;
        private IStat _statResist;
        private AttributeModifier _statResistModifier;
        
        private readonly FuryEffectSettings _settings;

        public FuryEffect( FuryEffectSettings settings ) : base( settings )
        {
            _settings = settings ?? throw new ArgumentNullException( nameof(settings) );
        }

        public override void Apply( Sheet sheet )
        {
            _statExtraDamage = sheet.Stats.ExtraDamage;
            _statExtraDamageModifier = new AddAttributeModifier( _settings.X );
            _statExtraDamage.AddModifier( _statExtraDamageModifier );
            
            _statDefence = sheet.Stats.Defense;
            _statDefenceModifier = new AddAttributeModifier( _settings.Y );
            _statDefence.AddModifier( _statDefenceModifier );
            
            _statResist = sheet.Stats.Resist;
            _statResistModifier = new AddAttributeModifier( _settings.Z );
            _statResist.AddModifier( _statResistModifier );
        }

        public override void Remove( Sheet sheet )
        {
            _statExtraDamage.RemoveModifier( _statExtraDamageModifier );
            _statExtraDamage = null;
            _statExtraDamageModifier = null;
            
            _statDefence.RemoveModifier( _statDefenceModifier );
            _statDefence = null;
            _statDefenceModifier = null;
            
            _statResist.RemoveModifier( _statResistModifier );
            _statResist = null;
            _statResistModifier = null;
        }
    }
}
