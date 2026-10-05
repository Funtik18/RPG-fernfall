using SoosvetGames.Storage;
using System;

namespace Game.Systems.StorageSystem
{
    public sealed class DataHolder
    {
        public GameFastData GameFastData { get; }
        
        public GameStorage GameStorageData => _storageSaveLoader.GetStorage();
		
        private readonly ISaveLoad< GameStorage > _storageSaveLoader;

        public DataHolder(
            GameFastData gameFastData,
            GameStorageInitializer gameStorageInitializer
        )
        {
            GameFastData = gameFastData ?? throw new ArgumentNullException( nameof(gameFastData) );
            _storageSaveLoader = gameStorageInitializer.GetStorage();
        }
        
        public void Save()
        {
            _storageSaveLoader.Save();
        }
    }
}