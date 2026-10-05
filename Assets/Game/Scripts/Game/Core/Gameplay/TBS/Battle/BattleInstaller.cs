using Game.Core.Systems.SheetSystem;
using UnityEngine;
using Zenject;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleInstaller : MonoInstaller< BattleInstaller >
    {
        [ SerializeField ] private BattleObject _view;
        
        public override void InstallBindings()
        {
            Container.BindInstance( _view );

            Container.Bind< BattleOutcomeEvaluator >().AsSingle().Lazy();
            Container.Bind< BattleRoundFactory >().AsSingle().Lazy();
            Container.Bind< BattleFactory >().AsSingle().Lazy();
            
            Container.Bind< BattleInputController >().AsSingle().Lazy();
            Container.Bind< BattleSelectionController >().AsSingle().Lazy();
            Container.Bind< BattleHoveringController >().AsSingle().Lazy();
            Container.Bind< BattleGridController >().AsSingle().Lazy();
            Container.Bind< BattleEffectController >().AsSingle().Lazy();
            Container.Bind< BattleSkillController >().AsSingle().Lazy();
            Container.Bind< BattleFuryController >().AsSingle().Lazy();
            Container.Bind< BattleFearController >().AsSingle().Lazy();
            Container.Bind< BattleEnemyRetreatController >().AsSingle().Lazy();
            Container.Bind< BattleAllySupportController >().AsSingle().Lazy();
            AIInstaller.Install( Container );
            Container.Bind< BattleAIController >().AsSingle().Lazy();
 
            InstallActions();
            InstallCombat();
            InstallPreview();
            
            Container.Bind< BattleNearbyAlliesService >().AsSingle().Lazy();
            
            Container.Bind< BattleController >().AsSingle().NonLazy();
        }

        private void InstallCombat()
        {
            Container.Bind< BattleCombatTargetingController >().AsSingle().Lazy();
            Container.Bind< BattleAttackExecutor >().AsSingle().Lazy();
            Container.Bind< BattleCounterattackController >().AsSingle().Lazy();
            Container.Bind< BattleFollowUpAttackController >().AsSingle().Lazy();
            Container.Bind< CombatArtFactory >().AsSingle().Lazy();
            Container.Bind< BattleCombatArtsController >().AsSingle().Lazy();
            Container.Bind< BattleCombatController >().AsSingle().Lazy();
        }

        private void InstallActions()
        {
            Container.Bind< BattlePathfinder >().AsSingle().Lazy();
            Container.Bind< BattleMovementController >().AsSingle().Lazy();

            Container.Bind< BattleActionMenuController >().AsSingle().Lazy();
            Container.Bind< BattleActionController >().AsSingle().Lazy();
            Container.Bind< BattleActionFactory >().AsSingle().Lazy();
        }

        private void InstallPreview()
        {
            Container.Bind< BattlePathPreviewController >().AsSingle().Lazy();
            Container.Bind< BattleCellsPreviewController >().AsSingle().Lazy();
            Container.Bind< BattleUnitPreviewController >().AsSingle().Lazy();
        }
    }
}
