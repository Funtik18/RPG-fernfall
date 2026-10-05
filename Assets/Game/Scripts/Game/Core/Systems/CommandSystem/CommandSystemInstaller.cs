using Zenject;

namespace Game.Core.Systems.CommandSystem
{
    public sealed class CommandSystemInstaller : Installer< CommandSystemInstaller >
    {
        public override void InstallBindings()
        {
            Container.Bind< CommandFactory >().AsSingle().Lazy();
        }
    }
}