using System;
using UnityEngine.InputSystem;

namespace Game.Managers.InputManager
{
    public abstract class InputAction1DWrap
    {
        public InputAction Input { get; }
        
        public bool IsEnable { get; private set; }

        public InputAction1DWrap( InputAction input )
        {
            Input = input ?? throw new ArgumentNullException( nameof(input) );
        }

        public virtual void Dispose()
        {
            Disable();
        }
        
        public void Enable()
        {
            if ( IsEnable ) return;
            IsEnable = true;
            
            Input.performed += InputPerformedHandler;
            Input.canceled += InputCanceledHandler;
        }

        public void Disable()
        {
            if ( !IsEnable ) return;
            IsEnable = false;
            
            Input.performed -= InputPerformedHandler;
            Input.canceled -= InputCanceledHandler;
        }

        protected abstract void OnPositivePerformed();
        protected abstract void OnNegativePerformed();
        protected abstract void OnCanceled();
        
        private void InputPerformedHandler( InputAction.CallbackContext context )
        {
            var value = context.ReadValue< float >();
            if ( value < 0 )
            {
                OnNegativePerformed();
            }
            else if ( value > 0 )
            {
                OnPositivePerformed();
            }
        }

        private void InputCanceledHandler( InputAction.CallbackContext context )
        {
            OnCanceled();
        }
    }
}