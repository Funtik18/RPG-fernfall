using Zenject;

namespace Game.Systems.StorageSystem
{
    public sealed class StorageSystemInstaller : Installer< StorageSystemInstaller >
    {
        public override void InstallBindings()
        {
            Container.Bind< GameFastData >().AsSingle();

            Container.Bind< GameStorageInitializer >().AsSingle().WhenInjectedInto< DataHolder >();
            Container.Bind< DataHolder >().AsSingle().NonLazy();
        }
    }
}