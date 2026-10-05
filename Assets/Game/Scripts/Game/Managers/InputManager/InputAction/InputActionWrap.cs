using System;
using UnityEngine.InputSystem;

namespace Game.Managers.InputManager
{
    public abstract class InputActionWrap
    {
        public InputAction Input { get; }
        
        public bool IsEnable { get; private set; }

        public InputActionWrap( InputAction input )
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

        protected abstract void OnPerformed();
        protected abstract void OnCanceled();
        
        private void InputPerformedHandler( InputAction.CallbackContext context )
        {
            OnPerformed();
        }

        private void InputCanceledHandler( InputAction.CallbackContext context )
        {
            OnCanceled();
        }
    }
}