using Zenject;

namespace Game.Core.Systems.StateSystem
{
    public sealed class StateSystemInstaller : Installer< StateSystemInstaller >
    {
        public override void InstallBindings()
        {
            Container.Bind< StateFactory >().AsSingle().Lazy();
        }
    }
}