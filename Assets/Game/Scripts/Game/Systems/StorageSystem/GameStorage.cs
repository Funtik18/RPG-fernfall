using SoosvetGames.Storage;
using SoosvetGames.Storage.Data;

namespace Game.Systems.StorageSystem
{
    public sealed class GameStorage : Storage
    {
        public StorageData< PreferencesData > PreferencesData { get; private set; }
        public StorageData< ProgressData > ProgressData { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            PreferencesData = new( Database, "preferences_data" );
            ProgressData = new( Database, "progress_data" );
        }

        public override void Purge()
        {
            
        }
    }
}