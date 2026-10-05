using System;
using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = "RoundHealthDamageEffect", menuName = "Game/Sheet/Effects/Round Health Damage" ) ]
    public sealed class RoundHealthDamageEffectConfig : CellEffectConfig
    {
        [ field: Header( "TODO RM" ) ]
        [ field: SerializeField, Min( 1 ) ] public int DurationRounds { get; private set; } = 3;
        [ field: SerializeField, Min( 1f ) ] public float DamagePerRound { get; private set; } = 1f;
        [ field: SerializeField ] public bool ApplyImmediately { get; private set; }

        public override Type GetEffectType() => typeof(RoundHealthDamageEffect);
    }
    
    public sealed class RoundHealthDamageEffect : Effect
    {
        public override bool IsExpired => _remainingRounds <= 0;
        
        private int _remainingRounds;

        private readonly RoundHealthDamageEffectConfig _config;
        
        public RoundHealthDamageEffect( IEffectSettings settings ) : base( settings )
        {
            _config = settings as RoundHealthDamageEffectConfig ?? throw new ArgumentException( $"Expected {nameof(RoundHealthDamageEffectConfig)}.", nameof(settings) );
        }

        public override void Apply( Sheet sheet )
        {
            Refresh( sheet );

            if ( _config.ApplyImmediately )
            {
                ApplyDamage( sheet );
            }
        }

        public override void Refresh( Sheet sheet )
        {
            _remainingRounds = Mathf.Max( 0, _config.DurationRounds );
        }

        public override void TickRoundStarted( Sheet sheet )
        {
            if ( _remainingRounds <= 0 ) return;

            ApplyDamage( sheet );
            _remainingRounds--;
        }

        private void ApplyDamage( Sheet sheet )
        {
            sheet.Stats.HealthPoints.Value -= _config.DamagePerRound;
        }
    }
}
