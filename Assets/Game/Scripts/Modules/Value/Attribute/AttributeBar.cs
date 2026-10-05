using UnityEngine;

namespace Value
{
    public abstract partial class AttributeBar : Attribute, IBar
    {
        public float PercentValue => Value / TotalValue;
        
        public override float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp( value, MinValue, TotalValue );
                InvokeChanged();
            }
        }

        public virtual float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                _value = Mathf.Clamp( _value, MinValue, TotalValue );
                InvokeChanged();
            }
        }
        protected float _maxValue;

        public virtual float MinValue { get; }

        protected AttributeBar( float value, float min, float max ) : base( value )
        {
            MinValue = min;
            _maxValue = max;
        }
    }
    
    //IModifiable Implementation
    public abstract partial class AttributeBar
    {
        public override float TotalValue => ( MaxValue + ModifyAddValue ) * ( 1f + ( ModifyPercentValue / 100f ) );

        public override bool AddModifier( AttributeModifier modifier )
        {
            if ( base.AddModifier( modifier ) )
            {
                Value = _value;
                return true;
            }

            return false;
        }

        public override bool RemoveModifier( AttributeModifier modifier )
        {
            if ( base.RemoveModifier( modifier ) )
            {
                Value = _value;
                return true;
            }

            return false;
        }
    }
}