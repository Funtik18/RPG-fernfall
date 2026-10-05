using Zenject;

namespace Game.Managers.BattleManager
{
    public sealed class BattleManagerInstaller : Installer< BattleManagerInstaller >
    {
        public override void InstallBindings()
        {
            Container.Bind< BattleManager >().AsSingle().Lazy();
        }
    }
}