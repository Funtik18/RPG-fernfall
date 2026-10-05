using Game.GamePipeline;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public sealed class CampInstaller : MonoInstaller< CampInstaller >
    {
        public override void InstallBindings()
        {
            Debug.Log( "[GamePipeline] CampInstaller" );

            Container.BindInterfacesAndSelfTo< BootstrapCamp >().AsSingle().NonLazy();
        }
    }
}