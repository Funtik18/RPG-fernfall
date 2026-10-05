using Game.Core.Systems.SheetSystem;
using System;
using UnityEngine;
using Value;

namespace Game.Core.Gameplay.TBS
{
    [ System.Serializable ]
    public sealed class AllySupportEffectSettings : EffectSettings
    {
        public override Type GetEffectType() => typeof(AllySupportEffect);
    }
    
    public sealed class AllySupportEffect : Effect
    {
        private IStat _hit;
        private AttributeModifier _hitModifier;
        private IStat _avoid;
        private AttributeModifier _avoidModifier;

        private readonly AllySupportEffectSettings _settings;
        
        public AllySupportEffect( AllySupportEffectSettings settings ) : base( settings )
        {
            _settings = settings ?? throw new ArgumentNullException( nameof(settings) );
        }

        public override void Apply( Sheet sheet )
        {
            _hit = sheet.Stats.Hit;
            _hitModifier = new AddAttributeModifier( _settings.X );
            _hit.AddModifier( _hitModifier );

            _avoid = sheet.Stats.Avoid;
            _avoidModifier = new AddAttributeModifier( _settings.Y );
            _avoid.AddModifier( _avoidModifier );
        }

        public override void Remove( Sheet sheet )
        {
            _hit.RemoveModifier( _hitModifier );
            _hit = null;
            _hitModifier = null;

            _avoid.RemoveModifier( _avoidModifier );
            _avoid = null;
            _avoidModifier = null;
        }
    }
}