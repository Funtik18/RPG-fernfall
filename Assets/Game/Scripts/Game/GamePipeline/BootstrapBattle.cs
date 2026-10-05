using Game.Core.Gameplay.TBS;
using Game.Managers.BattleManager;
using Game.UI;
using Game.UI.HUDBattleScreen;
using System;
using Zenject;

namespace Game.GamePipeline
{
    public sealed class BootstrapBattle : IInitializable, IDisposable
    {
        private readonly DiContainer _diContainer;
        private readonly BattleManager _battleManager;
        
        public BootstrapBattle(
            DiContainer diContainer,
            BattleManager battleManager
            )
        {
            _diContainer = diContainer ?? throw new ArgumentNullException( nameof(diContainer) );
            _battleManager = battleManager ?? throw new ArgumentNullException( nameof(battleManager) );
        }
        
        public void Initialize()
        {
            var view = _diContainer.InstantiatePrefabForComponent< BattleObject >( _battleManager.CurrentBattlePrefab );
            _battleManager.InitializeBattle( view.Controller, () =>
            {
                _battleManager.StartBattle();
            } );
        }

        public void Dispose()
        {
            
        }
    }
}