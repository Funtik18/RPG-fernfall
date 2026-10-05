using System;
using UnityEngine.InputSystem;

namespace Game.Managers.InputManager
{
    public sealed class InputAction1DVoid : InputAction1DWrap
    {
        private Action _onPositivePerformed;
        private Action _onNegativePerformed;
        private Action _onCanceled;
        
        public InputAction1DVoid(
            InputAction input,
            Action onNegativePerformed = null,
            Action onPositivePerformed = null,
            Action onCanceled = null
            ) : base( input )
        {
            _onNegativePerformed = onNegativePerformed;
            _onPositivePerformed = onPositivePerformed;
            _onCanceled = onCanceled;
        }
        
        public override void Dispose()
        {
            base.Dispose();

            _onNegativePerformed = null;
            _onPositivePerformed = null;
            _onCanceled = null;
        }

        protected override void OnPositivePerformed()
        {
            _onPositivePerformed?.Invoke();
        }

        protected override void OnNegativePerformed()
        {
            _onNegativePerformed?.Invoke();
        }

        protected override void OnCanceled()
        {
            _onCanceled?.Invoke();
        }
    }
}