using Game.Core.Systems.SheetSystem;
using System;
using Value;

namespace Game.Core.Gameplay.TBS
{
    [ Serializable ]
    public sealed class FearEffectSettings : EffectSettings
    {
        public override Type GetEffectType() => typeof(FearEffect);
    }

    public sealed class FearEffect : Effect
    {
        private IStat _statHit;
        private AttributeModifier _statHitModifier;
        
        private readonly FearEffectSettings _settings;

        public FearEffect( FearEffectSettings settings ) : base( settings )
        {
            _settings = settings ?? throw new ArgumentNullException( nameof(settings) );
        }

        public override void Apply( Sheet sheet )
        {
            _statHit = sheet.Stats.Hit;
            _statHitModifier = new AddAttributeModifier( -_settings.X );
            _statHit.AddModifier( _statHitModifier );
        }

        public override void Remove( Sheet sheet )
        {
            _statHit.RemoveModifier( _statHitModifier );
            _statHit = null;
            _statHitModifier = null;
        }
    }
}