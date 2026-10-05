using UnityEngine;
using Zenject;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitInstaller : MonoInstaller< UnitInstaller >
    {
        [ SerializeField ] private UnitObject _view;
        
        public override void InstallBindings()
        {
            Container.BindInstance( _view );

            Container.Bind< UnitMovementController >().AsSingle().Lazy();
            Container.Bind< UnitMotionController >().AsSingle().Lazy();
            Container.Bind< UnitHighlightController >().AsSingle().Lazy();
            Container.Bind< UnitGUIController >().AsSingle().Lazy();
            Container.Bind< UnitAnimatorController >().AsSingle().Lazy();
            Container.Bind< UnitEffectController >().AsSingle().Lazy();
            Container.Bind< UnitSkillController >().AsSingle().Lazy();

            AIInstaller.Install( Container );

            Container.Bind< UnitController >().AsSingle().NonLazy();
        }
    }
}
