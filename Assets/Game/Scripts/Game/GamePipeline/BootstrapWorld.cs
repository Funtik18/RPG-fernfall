using Cysharp.Threading.Tasks;
using Game.Core.Gameplay;
using Game.Core.Gameplay.World;
using Game.Core.Gameplay.World.Player;
using Game.Managers.CampaignManager;
using Game.Managers.WorldManager;
using Game.UI;
using Game.UI.HUDScreen;
using System;
using Zenject;

namespace Game.GamePipeline
{
    public sealed class BootstrapWorld : IInitializable, IDisposable
    {
        private readonly DiContainer _diContainer;
        private readonly CampaignManager _campaignManager;
        private readonly WorldGameplayConfig _worldGameplayConfig;
        private readonly WorldManager _worldManager;
        private readonly UIManager _uiManager;
        
        public BootstrapWorld(
            DiContainer diContainer,
            CampaignManager campaignManager,
            WorldGameplayConfig worldGameplayConfig,
            WorldManager worldManager,
            UIManager uiManager
            )
        {
            _diContainer = diContainer ?? throw new ArgumentNullException( nameof(diContainer) );
            _campaignManager = campaignManager ?? throw new ArgumentNullException( nameof(campaignManager) );
            _worldGameplayConfig = worldGameplayConfig ?? throw new ArgumentNullException( nameof(worldGameplayConfig) );
            _worldManager = worldManager ?? throw new ArgumentNullException( nameof(worldManager) );
            _uiManager = uiManager ?? throw new ArgumentNullException( nameof(uiManager) );
        }
        
        public void Initialize()
        {
            Initialization().Forget();
        }

        public void Dispose()
        {
            _worldManager.Dispose();
        }

        private async UniTask Initialization()
        {
            _campaignManager.GetProgressData();

            //create world
            var world = _diContainer.InstantiatePrefabForComponent< WorldObject >( _worldGameplayConfig.WorldPrefab );
            world.Controller.Initialize();
            _worldManager.World = world.Controller;

            //create player
            var player = _diContainer.InstantiatePrefabForComponent< PlayerObject >( _worldGameplayConfig.Player.Prefab );
            player.Controller.Initialize();
            _worldManager.Player = player.Controller;
            
            //ui
            _uiManager.ScreenAggregator.ShowAndCreateIfNotExist< HUDScreenViewModel >();

            //spawn
            if ( _campaignManager.IsNewCampaign )
            {
                player.Controller.Teleport( world.PlayerSpawnPoint.position );
            }
            
            //save restore
            //camera

            _campaignManager.ConsumeNewCampaign();
        }
    }
}
