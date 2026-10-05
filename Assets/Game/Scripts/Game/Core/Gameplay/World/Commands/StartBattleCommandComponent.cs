using Game.Core.Gameplay.TBS;
using Game.Core.Gameplay.World.Entity;
using Game.Core.Systems.CommandSystem;
using Game.Managers.BattleManager;
using Game.Managers.SceneManager;
using Game.Managers.WorldManager;
using System;
using UnityEngine;

namespace Game.Core.Gameplay.World
{
    public sealed class StartBattleCommandComponent : CommandComponent
    {
        [ field: SerializeField ] public BattleObject Prefab { get; private set; }
        
        public override Type GetPresenterType() => typeof( StartBattleCommandPresenter );
    }
    
    public sealed class StartBattleCommandPresenter : EntityCommandPresenter< StartBattleCommandComponent >
    {
        private readonly WorldManager _worldManager;
        private readonly BattleManager _battleManager;
        private readonly SceneLoader _sceneLoader;
        
        public StartBattleCommandPresenter(
            EntityObject entity,
            
            WorldManager worldManager,
            BattleManager battleManager,
            SceneLoader sceneLoader,
            StartBattleCommandComponent component
        ) : base( entity, component )
        {
            _worldManager = worldManager ?? throw new ArgumentNullException( nameof(worldManager) );
            _battleManager = battleManager ?? throw new ArgumentNullException( nameof(battleManager) );
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException( nameof(sceneLoader) );
        }

        public override void Execute()
        {
            _worldManager.Commit();
            _battleManager.SetBattle( _entity, Component.Prefab );
            _sceneLoader.LoadBattle();
        }
    }
}