using UnityEngine;

namespace Game.Systems.StorageSystem.World
{
    public sealed class EntityData
    {
        public string UID;
        
        public Vector2 Position;
        public Quaternion Rotation;

        public bool IsEnabled;
    }
}