using Cysharp.Threading.Tasks;
using Game.Core.Systems.SheetSystem;
using SoosvetGames.CommonTools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleController
    {
        public event Action< BattleResultType > OnFinished;
        
        public BattleObject View { get; }
        public Battle Battle { get; private set; }
        
        public event Action< UnitController > OnUnitActionMenuRequested
        {
            add => _actionMenuController.OnUnitActionMenuRequested += value;
            remove => _actionMenuController.OnUnitActionMenuRequested -= value;
        }

        public event Action< UnitController, UnitController > OnCombatActionMenuRequested
        {
            add => _actionController.OnCombatActionMenuRequested += value;
            remove => _actionController.OnCombatActionMenuRequested -= value;
        }

        public event Action< BattleCell > OnCellHovered
        {
            add => _inputController.OnCellHovered += value;
            remove => _inputController.OnCellHovered -= value;
        }

        public event Action< BattleCell > OnCellHoverExited
        {
            add => _inputController.OnCellHoverExited += value;
            remove => _inputController.OnCellHoverExited -= value;
        }
        
        public IBattleSelection BattleSelection => _selectionController;

        public ServiceLocator Services { get; } = new();
        
        private readonly BattleFactory _factory;
        private readonly UnitFactory _unitFactory;
        private readonly BattleInputController _inputController;
        private readonly BattleGridController _gridController;
        private readonly BattleEffectController _effectController;
        private readonly BattleSkillController _skillController;
        private readonly BattleFuryController _furyController;
        private readonly BattleFearController _fearController;
        private readonly BattleEnemyRetreatController _enemyRetreatController;
        private readonly BattleAllySupportController _allySupportController;
        private readonly BattleActionMenuController _actionMenuController;
        private readonly BattleActionController _actionController;
        private readonly BattleCombatController _combatController;
        private readonly BattleCombatArtsController _combatArtsController;
        private readonly BattleAIController _aiController;
        private readonly BattleSelectionController _selectionController;
        private readonly BattlePathPreviewController _pathPreviewController;
        private readonly BattleCellsPreviewController _cellsPreviewController;
        private readonly BattleUnitPreviewController _unitPreviewController;
        
        public BattleController(
            BattleObject view,
            BattleFactory factory,
            UnitFactory unitFactory,
            BattleInputController inputController,
            BattleGridController gridController,
            BattleEffectController effectController,
            BattleSkillController skillController,
            BattleFuryController furyController,
            BattleFearController fearController,
            BattleEnemyRetreatController enemyRetreatController,
            BattleAllySupportController allySupportController,
            BattleActionMenuController actionMenuController,
            BattleActionController actionController,
            BattleCombatController combatController,
            BattleCombatArtsController combatArtsController,
            BattleAIController aiController,
            BattleSelectionController selectionController,
            BattlePathPreviewController pathPreviewController,
            BattleCellsPreviewController cellsPreviewController,
            BattleUnitPreviewController unitPreviewController,
            
            BattleNearbyAlliesService nearbyAlliesService
            )
        {
            View = view ?? throw new ArgumentNullException( nameof(view) );

            _factory = factory ?? throw new ArgumentNullException( nameof(factory) );
            _unitFactory = unitFactory ?? throw new ArgumentNullException( nameof(unitFactory) );
            _inputController = inputController ?? throw new ArgumentNullException( nameof(inputController) );
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _effectController = effectController ?? throw new ArgumentNullException( nameof(effectController) );
            _skillController = skillController ?? throw new ArgumentNullException( nameof(skillController) );
            _furyController = furyController ?? throw new ArgumentNullException( nameof(furyController) );
            _fearController = fearController ?? throw new ArgumentNullException( nameof(fearController) );
            _enemyRetreatController = enemyRetreatController ?? throw new ArgumentNullException( nameof(enemyRetreatController) );
            _allySupportController = allySupportController ?? throw new ArgumentNullException( nameof(allySupportController) );
            _actionMenuController = actionMenuController ?? throw new ArgumentNullException( nameof(actionMenuController) );
            _actionController = actionController ?? throw new ArgumentNullException( nameof(actionController) );
            _combatController = combatController ?? throw new ArgumentNullException( nameof(combatController) );
            _combatArtsController = combatArtsController ?? throw new ArgumentNullException( nameof(combatArtsController) );
            _aiController = aiController ?? throw new ArgumentNullException( nameof(aiController) );
            _selectionController = selectionController ?? throw new ArgumentNullException( nameof(selectionController) );
            _pathPreviewController = pathPreviewController ?? throw new ArgumentNullException( nameof(pathPreviewController) );
            _cellsPreviewController = cellsPreviewController ?? throw new ArgumentNullException( nameof(cellsPreviewController) );
            _unitPreviewController = unitPreviewController ?? throw new ArgumentNullException( nameof(unitPreviewController) );
            
            Services.Register( nearbyAlliesService );
            
            View.SetController( this );
        }
        
        public void Initialize( Action callback = null )
        {
            Create( callback ).Forget();
        }
        
        public void Dispose()
        {
            Services.Clear();
            
            _inputController.Dispose();
            _skillController.Dispose();
            _effectController.Dispose();
            _furyController.Dispose();
            _fearController.Dispose();
            _enemyRetreatController.Dispose();
            _allySupportController.Dispose();
            _combatArtsController.Dispose();
            _combatController.Dispose();
            _gridController.Dispose();
            _actionMenuController.Dispose();
            _actionController.Dispose();
            _aiController.Dispose();
            
            _pathPreviewController.Dispose();
            _cellsPreviewController.Dispose();
            _unitPreviewController.Dispose();

            if ( Battle == null ) return;
            Battle.OnFinished -= BattleFinishedHandler;
            foreach ( var unit in Battle.Units )
            {
                unit.Dispose();
            }
        }

        private async UniTask Create( Action callback = null )
        {
            await _gridController.Create();
            
            List< UnitController > playerUnits = new();
            List< UnitController > enemyUnits = new();

            foreach ( var grid in View.Grids )
            {
                var definitions = grid.Definitions;
                foreach ( var definition in definitions.CellDefinitions )
                {
                    if ( definition.Unit == null ) continue;
                
                    var controller = _unitFactory.Create( definition.Unit );
                    controller.Teleport( definition.Cell.transform.position, definition.UnitTransform.Forward );

                    if ( definition.Unit.SpawnGroup is GridCellPlayerSpawnGroup )
                    {
                        playerUnits.Add( controller );
                    }
                    else if ( definition.Unit.SpawnGroup is GridCellEnemySpawnGroup )
                    {
                        enemyUnits.Add( controller );
                    }
                
                    var cell = _gridController.GetCell( definition.Cell );
                    _gridController.Registry.Place( controller, cell );
                }
            }

            Battle = _factory.Create( playerUnits, enemyUnits );
            Battle.OnFinished += BattleFinishedHandler;
            
            _combatArtsController.Initialize();
            _combatController.Initialize( Battle );
            _effectController.Initialize( Battle );
            _skillController.Initialize( Battle );
            _furyController.Initialize( Battle );
            _fearController.Initialize( Battle );
            _enemyRetreatController.Initialize( Battle );
            _allySupportController.Initialize( Battle );
            _inputController.Initialize();
            _actionMenuController.Initialize( Battle );
            _actionController.Initialize( Battle );
            _aiController.Initialize( Battle );
            
            _pathPreviewController.Initialize( Battle );
            _cellsPreviewController.Initialize();
            _unitPreviewController.Initialize();
            
            callback?.Invoke();
        }

        public void Start()
        {
            Battle.Start();
        }

        public GridCellTerrainConfig GetUnitCellTerrainConfig( UnitController unit )
        {
            var cell = _gridController.Registry.GetUnitCell( unit );
            if ( cell == null ) return null;
            return GetCellTerrainConfig( cell );
        }
        
        public GridCellTerrainConfig GetCellTerrainConfig( BattleCell cell )
        {
            return cell.Terrain ?? cell.Grid.DefaultTerrainConfig;
        }

        public bool SelectAttackAction()
        {
            return !_actionController.IsExecuting && _actionMenuController.SelectAttackAction();
        }

        public bool TrySelectUnit( UnitController unit )
        {
            return _actionController.TrySelectUnit( unit );
        }

        public bool CanSelectUnit( UnitController unit )
        {
            return _actionController.CanSelectUnit( unit );
        }

        public bool WaitSelectedUnit()
        {
            return !_actionController.IsExecuting && _actionMenuController.WaitSelectedUnit();
        }

        public bool ConfirmCombatAction( CombatArtConfig combatArt = null )
        {
            return !_actionController.IsExecuting && _actionController.ConfirmPendingCombatAction( combatArt );
        }

        public bool CanAttackWithWeaponFromCurrentCell( UnitController attacker, UnitController defender, Weapon weapon )
        {
            return _combatController.IsCanAttackWithWeaponFromCurrentCell( attacker, defender, weapon );
        }

        public bool CanUseCombatArt( UnitController attacker, Weapon weapon, CombatArtConfig combatArt )
        {
            return _combatArtsController.CanUseCombatArt( attacker, weapon, combatArt );
        }

        public void RefreshSelectedUnitCellsPreview()
        {
            _cellsPreviewController.RefreshSelectedUnitPreview();
        }

        public int GetCombatArtUsesRemaining( UnitController attacker, CombatArtConfig combatArt )
        {
            return _combatArtsController.GetCombatArtUsesRemaining( attacker, combatArt );
        }

        public bool CancelCombatAction()
        {
            return _actionController.CancelPendingCombatAction();
        }
        
        private void BattleFinishedHandler()
        {
            OnFinished?.Invoke( Battle.Result.Type );
        }
    }
}
