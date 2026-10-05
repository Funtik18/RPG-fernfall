using Zenject;

namespace Game.Core.Gameplay.TBS
{
    public sealed class AIInstaller : Installer< AIInstaller >
    {
        public override void InstallBindings()
        {
            Container.Bind< AIBehaviourFactory >().AsSingle().Lazy();
        }
    }
}