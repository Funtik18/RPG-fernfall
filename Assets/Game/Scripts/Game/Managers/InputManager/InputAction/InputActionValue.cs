using System;
using UnityEngine.InputSystem;

namespace Game.Managers.InputManager
{
    public sealed class InputActionValue< T > : InputActionWrap
        where T : struct
    {
        private Action< T > _onPerformed;
        private Action< T > _onCanceled;
        
        public InputActionValue( InputAction input, Action< T > onPerformed = null, Action< T > onCanceled = null ) : base( input )
        {
            _onPerformed = onPerformed;
            _onCanceled = onCanceled;
        }

        public override void Dispose()
        {
            base.Dispose();

            _onPerformed = null;
            _onCanceled = null;
        }

        protected override void OnPerformed()
        {
            _onPerformed?.Invoke( Input.ReadValue< T >() );
        }

        protected override void OnCanceled()
        {
            _onCanceled?.Invoke( Input.ReadValue< T >() );
        }
    }
}