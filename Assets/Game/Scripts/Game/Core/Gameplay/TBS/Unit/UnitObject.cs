using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitObject : MonoBehaviour
    {
        [ field: SerializeField ] public Transform MeshRoot { get; private set; }
        [ field: SerializeField ] public UnitGUI GUI { get; private set; }
        [ field: SerializeField ] public Animator Animator { get; private set; }
        [ field: SerializeField ] public Rigidbody Rigidbody { get; private set; }
        [ field: SerializeField ] public Collider Collider { get; private set; }
        [ field: SerializeField ] public MeshRenderer Highlighter { get; private set; }
        
        public UnitController Controller { get; private set; }

        public void SetController( UnitController controller )
        {
            Controller = controller;
        }

        public void EnableCollider( bool trigger )
        {
            Collider.enabled = trigger;
        }
        
        public void LockRigidbodyToGrid()
        {
            if ( Rigidbody == null )
            {
                return;
            }

            if ( !Rigidbody.isKinematic )
            {
                Rigidbody.linearVelocity = Vector3.zero;
                Rigidbody.angularVelocity = Vector3.zero;
            }

            Rigidbody.useGravity = false;
            Rigidbody.isKinematic = true;
        }
    }
}
