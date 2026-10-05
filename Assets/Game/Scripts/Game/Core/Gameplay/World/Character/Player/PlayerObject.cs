using UnityEngine;

namespace Game.Core.Gameplay.World.Player
{
    public sealed class PlayerObject : MonoBehaviour
    {
        [ field: SerializeField ] public Rigidbody Rigidbody { get; private set; }
        [ field: SerializeField ] public PlayerAvatar Avatar { get; private set; }

        public Transform Root => transform;
        public PlayerController Controller { get; private set; }
        
        public void SetController( PlayerController controller )
        {
            Controller = controller;
        }
    }
}