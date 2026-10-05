using Game.Core.Gameplay.TBS;
using Game.Core.Gameplay.World.Entity;
using Game.Managers.SceneManager;
using Game.Systems.StorageSystem.World;
using Game.UI;
using Game.UI.HUDBattleScreen;
using System;

namespace Game.Managers.BattleManager
{
    public sealed class BattleManager
    {
        public BattleObject CurrentBattlePrefab { get; private set; }
        public EntityData EntityData { get; private set; }
        
        public BattleController BattleController => _battle;
        private BattleController _battle;

        private readonly UIManager _uiManager;
        private readonly PartyManager.PartyManager _partyManager;
        private readonly SceneLoader _sceneLoader;
        
        public BattleManager(
            UIManager uiManager,
            PartyManager.PartyManager partyManager,
            SceneLoader sceneLoader
            )
        {
            _uiManager = uiManager ?? throw new ArgumentNullException( nameof(uiManager) );
            _partyManager = partyManager ?? throw new ArgumentNullException( nameof(partyManager) );
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException( nameof(sceneLoader) );
        }
        
        public void Dispose()
        {
            var hud = _uiManager.ScreenAggregator.GetAs< HUDBattleScreenViewModel >();
            hud.Dispose();
            
            CurrentBattlePrefab = null;
            _battle?.Dispose();
            _battle = null;
        }

        public void SetBattle( EntityObject entity, BattleObject battlePrefab )
        {
            CurrentBattlePrefab = battlePrefab;
            EntityData = entity.Controller.Data;
        }
        
        public void InitializeBattle( BattleController controller, Action callback = null )
        {
            _battle = controller;
            _battle.Initialize( callback );
            _battle.OnFinished += BattleFinishedHandler;
        }

        public void StartBattle()
        {
            _uiManager.ScreenAggregator.ShowAndCreateIfNotExist< HUDBattleScreenViewModel >();
            _battle.Start();
        }

        public void RunBattle()
        {
            _battle.OnFinished -= BattleFinishedHandler;
            Dispose();
            
            _sceneLoader.LoadGameplay();
        }

        private void BattleFinishedHandler( BattleResultType result )
        {
            _battle.OnFinished -= BattleFinishedHandler;
            Dispose();
            
            switch ( result )
            {
                case BattleResultType.Victory:
                {
                    if ( EntityData != null )
                    {
                        EntityData.IsEnabled = false;
                    }

                    var characters = _partyManager.Party.Characters;
                    foreach ( var character in characters )
                    {
                        character.Sheet.Stats.HealthPoints.Value = character.Sheet.Stats.HealthPoints.TotalValue;
                    }
                    
                    _sceneLoader.LoadGameplay();
                    break;
                }
                case BattleResultType.Defeat:
                {
                    _sceneLoader.LoadGameplay();
                    break;
                }
            }
        }
    }
}