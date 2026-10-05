using SoosvetGames.Storage;

namespace Game.Systems.StorageSystem
{
    public sealed class GameFastData
    {
        private const string IS_FIRST_TIME = "is_first_time";
        
        public bool IsFirstTime
        {
            get => InputOutput.DeserializePlayerPrefs( IS_FIRST_TIME, true );
            set => InputOutput.SerializePlayerPrefs( IS_FIRST_TIME, value );
        }
    }
}