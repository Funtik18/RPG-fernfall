using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ System.Serializable ]
    public sealed class UnitTransform
    {
        [ SerializeField ] private Vector3 _forward = Vector3.forward;
        public Vector3 Forward => _forward;

        public void SetForward( Vector3 forward )
        {
            _forward = forward.normalized;
        }
    }
}