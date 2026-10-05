using Zenject;

namespace Game.Core.Systems.FeatureSystem
{
    public sealed class FeatureSystemInstaller : Installer< FeatureSystemInstaller >
    {
        public override void InstallBindings()
        {
            Container.Bind< FeatureFactory >().AsSingle().Lazy();
        }
    }
}