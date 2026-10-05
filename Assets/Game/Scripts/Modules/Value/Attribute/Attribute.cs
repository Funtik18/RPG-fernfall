using System;
using System.Collections.Generic;

namespace Value
{
    public abstract partial class Attribute : IAttribute
    {
        public event Action OnChanged;

        public virtual float Value
        {
            get => _value;
            set
            {
                _value = value;
                InvokeChanged();
            }
        }
        protected float _value;

        public Attribute( float value )
        {
            _value = value;
        }

        protected void InvokeChanged()
        {
            OnChanged?.Invoke();
        }
    }

    //IModifiable Implementation
    public abstract partial class Attribute
    {
        public event Action OnModifiersChanged;

        public virtual float TotalValue => ( Value + ModifyAddValue ) * ( 1f + ( ModifyPercentValue / 100f ) );

        public virtual float ModifyAddValue
        {
            get
            {
                float value = 0;

                _modifiers.ForEach( ( modifier ) =>
                {
                    if ( modifier is AddAttributeModifier )
                    {
                        value += modifier.Value;
                    }
                } );

                return value;
            }
        }

        public virtual float ModifyPercentValue
        {
            get
            {
                float value = 0;

                _modifiers.ForEach( ( modifier ) =>
                {
                    if ( modifier is PercentAttributeModifier )
                    {
                        value += modifier.Value;
                    }
                } );

                return value;
            }
        }

        public IReadOnlyList< AttributeModifier > Modifiers => _modifiers;
        private readonly List< AttributeModifier > _modifiers = new();

        public virtual bool AddModifier( AttributeModifier modifier )
        {
            if ( !Contains( modifier ) )
            {
                _modifiers.Add( modifier );

                InvokeModifiersChanged();
                InvokeChanged();

                return true;
            }

            return false;
        }

        public virtual bool RemoveModifier( AttributeModifier modifier )
        {
            if ( Contains( modifier ) )
            {
                _modifiers.Remove( modifier );

                InvokeModifiersChanged();
                InvokeChanged();

                return true;
            }

            return false;
        }

        public bool Contains( AttributeModifier modifier ) => _modifiers.Contains( modifier );

        protected void InvokeModifiersChanged()
        {
            OnModifiersChanged?.Invoke();
        }
    }
}