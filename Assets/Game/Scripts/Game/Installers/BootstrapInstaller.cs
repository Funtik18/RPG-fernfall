using Game.Core.Gameplay;
using Game.Core.Systems.SheetSystem;
using Game.Managers.BattleManager;
using Game.Managers.WorldManager;
using Game.Managers.CampaignManager;
using Game.Systems.StorageSystem;
using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.Pipeline;
using Game.Systems.CameraSystem;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public sealed class BootstrapInstaller : MonoInstaller< BootstrapInstaller >
    {
        [ SerializeField ] private PartyGameplayConfig _partyGameplayConfig;
        
        public override void InstallBindings()
        {
            Debug.Log( "[GamePipeline] BootstrapInstaller" );
            
            Container.BindInstance( _partyGameplayConfig );
            
            StorageSystemInstaller.Install( Container );
            CameraSystemInstaller.Install( Container );
            WorldManagerInstaller.Install( Container );
            CampaignManagerInstaller.Install( Container );
            PartyManagerInstaller.Install( Container );
            BattleManagerInstaller.Install( Container );
            
            Container.Bind< Bootstrap >().AsSingle().NonLazy();
        }
    }
}