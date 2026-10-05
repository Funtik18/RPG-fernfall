using Game.Managers.SceneManager;
using Game.Systems.StorageSystem;
using System;

namespace Game.Managers.CampaignManager
{
    public sealed class CampaignManager
    {
        public bool IsHasCampaign => _dataHolder.GameStorageData.ProgressData.Value != null;
        public bool IsNewCampaign { get; private set; }
        
        private ProgressData _data;

        private readonly DataHolder _dataHolder;
        private readonly SceneLoader _sceneLoader;
        
        public CampaignManager(
            DataHolder dataHolder,
            SceneLoader sceneLoader
            )
        {
            _dataHolder = dataHolder ?? throw new ArgumentNullException( nameof(dataHolder) );
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException( nameof(sceneLoader) );
        }
        
        public void StartNewCampaign()
        {
            CreateNewProgressData();

            _sceneLoader.LoadGameplay();
        }

        public void ContinueCampaign()
        {
            IsNewCampaign = false;
            _data = _dataHolder.GameStorageData.ProgressData.Value;
            
            _sceneLoader.LoadGameplay();
        }

        public void ConsumeNewCampaign()
        {
            IsNewCampaign = false;
        }

        public ProgressData GetProgressData()
        {
#if UNITY_EDITOR
            //Editor checks, start from gameplay
            if ( _dataHolder.GameStorageData.ProgressData.Value == null )
            {
                CreateNewProgressData();
            }
#endif
            if ( _data == null )
            {
                _data = _dataHolder.GameStorageData.ProgressData.Value;
            }
            
            return _data;
        }

        private void CreateNewProgressData()
        {
            IsNewCampaign = true;
            _data = new ProgressData()
            {
                World = new(),
                Party = new(),
                Player = new(),
                Inventory = new(),
            };
            _dataHolder.GameStorageData.ProgressData.SetData( _data );
        }
    }
}
