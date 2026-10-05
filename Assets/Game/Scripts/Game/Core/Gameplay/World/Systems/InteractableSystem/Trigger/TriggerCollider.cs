using R3;
using System;
using UnityEngine;

namespace Game.Core.Gameplay.World
{
    public sealed class TriggerCollider : MonoBehaviour
    {
        public Observable< Collider > Enter => _enter;
        public Observable< Collider > Exit => _exit;
        
        private readonly Subject< Collider > _enter = new();
        private readonly Subject< Collider > _exit = new();

        private void OnTriggerEnter( Collider other )
        {
            _enter.OnNext( other );
        }

        private void OnTriggerExit( Collider other )
        {
            _exit.OnNext( other );
        }

        private void OnDestroy()
        {
            _enter.Dispose();
            _exit.Dispose();
        }
    }
}