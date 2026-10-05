using Game.Core.Gameplay.World;
using Game.Core.Gameplay.World.Player;
using Game.Systems.StorageSystem;

namespace Game.Managers.WorldManager
{
    public sealed class WorldManager : IMemento
    {
        public WorldController World { get; set; }
        public PlayerController Player { get; set; }

        public void Dispose()
        {
            Player?.Dispose();
            World?.Dispose();
        }

        public void Commit()
        {
            World?.Commit();
            Player?.Commit();
        }

        public void RestoreCommit()
        {

        }
    }
}