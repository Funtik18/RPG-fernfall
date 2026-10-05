using SoosvetGames.Storage;
using System;

namespace Game.Systems.StorageSystem
{
    public sealed class GameStorageInitializer
    {
        private readonly GameFastData _gameFastData;
        
        public GameStorageInitializer(
            GameFastData gameFastData
            )
        {
            _gameFastData = gameFastData ?? throw new ArgumentNullException( nameof(gameFastData) );
        }
        
        public ISaveLoad< GameStorage > GetStorage()
        {
            string dataKey = "game_storage";
            
            ISaveLoad< GameStorage > storageSaveLoader = new PlayerPrefsSaveLoad< GameStorage >( dataKey );
            GameStorage storage = storageSaveLoader.GetStorage();

            if ( _gameFastData.IsFirstTime )
            {
                AddFirstTimeData( storage );
            }

            var isSupported = AddDataForOldPlayers( storage );
            var isPurged = PurgeOldKeys( storage );

            if ( _gameFastData.IsFirstTime || isSupported || isPurged )
            {
                storageSaveLoader.Save();
            }

            return storageSaveLoader;
        }
        
        private void AddFirstTimeData( GameStorage storage )
        {

        }

        private bool AddDataForOldPlayers( GameStorage storage )
        {
            var isSupported = false;

            return isSupported;
        }
        
        private bool PurgeOldKeys( GameStorage storage )
        {
            var isPurged = false;

            return isPurged;
        }
    }
}