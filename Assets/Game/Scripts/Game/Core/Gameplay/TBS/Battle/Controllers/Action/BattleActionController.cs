using Cysharp.Threading.Tasks;
using Game.Core.Gameplay.TBS.Actions;
using Game.Core.Systems.SheetSystem;
using System;
using System.Linq;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleActionController
    {
        public event Action OnActionExecute;
        public event Action OnActionExecuted;
        public event Action< UnitController, UnitController > OnCombatActionMenuRequested;
        public bool IsExecuting => _isExecuting;

        private Battle _battle;
        private bool _isExecuting;
        private BattleActionTarget? _pendingCombatActionTarget;
        
        private readonly BattleActionFactory _actionFactory;
        private readonly BattleActionMenuController _actionMenuController;
        private readonly BattleInputController _inputController;
        private readonly BattleSelectionController _selectionController;
        private readonly BattleHoveringController _hoveringController;
        private readonly BattleGridController _gridController;
        private readonly BattleCombatController _combatController;
        private readonly BattleCombatArtsController _combatArtsController;
        private readonly BattleFuryController _furyController;
        private readonly BattleFearController _fearController;
        
        public BattleActionController(
            BattleActionFactory actionFactory,
            BattleActionMenuController actionMenuController,
            BattleInputController inputController,
            BattleSelectionController selectionController,
            BattleHoveringController hoveringController,
            BattleGridController gridController,
            BattleCombatController combatController,
            BattleCombatArtsController combatArtsController,
            BattleFuryController furyController,
            BattleFearController fearController
            )
        {
            _actionFactory = actionFactory ?? throw new ArgumentNullException( nameof(actionFactory) );
            _actionMenuController = actionMenuController ?? throw new ArgumentNullException( nameof(actionMenuController) );
            _inputController = inputController ?? throw new ArgumentNullException( nameof(inputController) );
            _selectionController = selectionController ?? throw new ArgumentNullException( nameof(selectionController) );
            _hoveringController = hoveringController ?? throw new ArgumentNullException( nameof(hoveringController) );
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
            _combatController = combatController ?? throw new ArgumentNullException( nameof(combatController) );
            _combatArtsController = combatArtsController ?? throw new ArgumentNullException( nameof(combatArtsController) );
            _furyController = furyController ?? throw new ArgumentNullException( nameof(furyController) );
            _fearController = fearController ?? throw new ArgumentNullException( nameof(fearController) );
        }

        public void Initialize( Battle battle )
        {
            _battle = battle;

            _battle.OnTurned += BattleTurnedHandler;
            
            _inputController.OnUnitClicked += UnitClickedHandler;
            _inputController.OnCellClicked += CellClickedHandler;
            _inputController.OnCellHovered += CellHoveredHandler;
            _inputController.OnCellHoverExited += CellHoverExitHandler;
        }

        public void Dispose()
        {
            _pendingCombatActionTarget = null;

            _battle.OnTurned -= BattleTurnedHandler;
            
            _inputController.OnUnitClicked -= UnitClickedHandler;
            _inputController.OnCellClicked -= CellClickedHandler;
            _inputController.OnCellHovered -= CellHoveredHandler;
            _inputController.OnCellHoverExited -= CellHoverExitHandler;
        }

        public bool TrySelectUnit( UnitController unit )
        {
            if ( !CanSelectUnit( unit ) ) return false;

            var turn = _battle.CurrentRound?.CurrentTurn;
            if ( turn == null || !turn.TrySelectUnit( unit ) ) return false;

            SelectUnit( unit );
            return true;
        }

        public bool CanSelectUnit( UnitController unit )
        {
            if ( _isExecuting ) return false;
            if ( _pendingCombatActionTarget.HasValue ) return false;
            if ( _actionMenuController.IsOpened ) return false;
            if ( IsSelectingActionTarget() ) return false;

            return CanSelectUnitInternal( unit );
        }

        private void UnitClickedHandler( UnitController unit )
        {
            if ( _isExecuting ) return;
            if ( _pendingCombatActionTarget.HasValue ) return;
            
            if ( _actionMenuController.IsOpened )
            {
                return;
            }
            
            if ( IsSelectingActionTarget() )
            {
                TryRequestCombatActionMenu( unit );
                return;
            }
            
            if ( CanSelectUnitInternal( unit ) )
            {
                if ( _battle.CurrentRound.CurrentTurn.TrySelectUnit( unit ) )
                {
                    SelectUnit( unit );
                }

                return;
            }

            var selectedUnit = _selectionController.SelectedUnit;
            if ( selectedUnit == null ) return;
            
            TryMoveToAttackPosition( selectedUnit, unit ).Forget();
        }

        private void CellClickedHandler( BattleCell cell )
        {
            if ( _isExecuting ) return;
            if ( _pendingCombatActionTarget.HasValue ) return;

            var selectedUnit = _selectionController.SelectedUnit;
            if ( selectedUnit == null ) return;

            if ( _actionMenuController.IsOpened )
            {
                return;
            }
            
            if ( IsSelectingActionTarget() )
            {
                TryRequestCombatActionMenu( cell );
                return;
            }
            
            var cellUnit = _gridController.Registry.GetUnit( cell );
            if ( cellUnit != null )
            {
                TryMoveToAttackPosition( selectedUnit, cellUnit ).Forget();
                return;
            }

            var action = _actionFactory.Create< MoveAction >( selectedUnit );
            var target = new BattleActionTarget( cell );

            TryMoveAndOpenActionMenu( action, target ).Forget();
        }

        private void CellHoveredHandler( BattleCell cell )
        {
            if ( _isExecuting ) return;

            var unit = _gridController.Registry.GetUnit( cell );
            if ( unit == null ) return;
            _hoveringController.HoverUnit( unit );
        }
        
        private void CellHoverExitHandler( BattleCell cell )
        {
            if ( _isExecuting ) return;
            
            var unit = _gridController.Registry.GetUnit( cell );
            if ( unit == null ) return;
            _hoveringController.HoverExitUnit( unit );
        }
        
        private void SelectUnit( UnitController unit )
        {
            _actionMenuController.Close();
            _selectionController.SelectUnit( unit );

            //default action.
            var moveAction = _actionFactory.Create< MoveAction >( unit );
            _selectionController.SelectAction( moveAction );
        }
        
        private bool CanSelectUnitInternal( UnitController unit )
        {
            if ( unit == null ) return false;
            if ( _battle == null ) return false;

            var playerTeam = _battle.Teams.FirstOrDefault( ( x ) => string.Equals( x.Id, BattleParams.PLAYER_TEAM, StringComparison.InvariantCultureIgnoreCase ) );

            if ( playerTeam == null ) return false;
            if ( _battle.IsFinished ) return false;

            var round = _battle.CurrentRound;
            if ( round == null || round.IsFinished ) return false;

            var turn = round.CurrentTurn;
            if ( turn == null || !turn.IsActive ) return false;
            if ( turn.Team != playerTeam ) return false;// Сейчас вообще не ход игрока.
            if ( !playerTeam.Contains( unit ) ) return false;// Нельзя управлять чужими юнитами.
            if ( _furyController.IsFurious( unit ) ) return false;// Яростным юнитом управляет AI.
            if ( _fearController.IsEscaping( unit ) ) return false;// Испуганный юнит в побеге управляется AI.
            if ( turn.HasCompletedUnit( unit ) ) return false;// Уже ходил в этом раунде.
            if ( !unit.IsInBattle() ) return false;

            return true;
        }
        
        public async UniTask< bool > TryExecuteAction( BattleAction action, BattleActionTarget target )
        {
            if ( _isExecuting ) return false;
            if ( action == null ) return false;
            if ( !action.CanExecute( target ) ) return false;

            _isExecuting = true;
            OnActionExecute?.Invoke();
            try
            {
                await action.Execute( target );

                var turn = _battle.CurrentRound?.CurrentTurn;
                if ( action.CompletesUnitTurn && turn?.ActiveUnit == action.Owner )
                {
                    turn.CompleteActiveUnit();
                    _actionMenuController.Close();
                    _selectionController.ClearAction();
                    _selectionController.ClearUnit();
                }

                return true;
            }
            finally
            {
                OnActionExecuted?.Invoke();
                _isExecuting = false;
            }
        }

        private async UniTask TryExecute( BattleAction action, BattleActionTarget target )
        {
            await TryExecuteAction( action, target );
        }

        public bool ConfirmPendingCombatAction( CombatArtConfig combatArt = null )
        {
            if ( _isExecuting ) return false;
            if ( !_pendingCombatActionTarget.HasValue ) return false;

            var selectedUnit = _selectionController.SelectedUnit;
            if ( selectedUnit == null ) return false;
            if ( combatArt != null && !_combatArtsController.CanUseCombatArt( selectedUnit, combatArt ) ) return false;

            var pendingTarget = _pendingCombatActionTarget.Value;
            var target = new BattleActionTarget( pendingTarget.Cell, pendingTarget.Unit, combatArt );
            _pendingCombatActionTarget = null;

            TryExecuteSelectedAction( target ).Forget();

            return true;
        }

        public bool CancelPendingCombatAction()
        {
            if ( !_pendingCombatActionTarget.HasValue ) return false;

            _pendingCombatActionTarget = null;

            var selectedUnit = _selectionController.SelectedUnit;
            _selectionController.ClearAction();

            if ( selectedUnit != null )
            {
                _actionMenuController.Open( selectedUnit );
            }

            return true;
        }

        private void BattleTurnedHandler()
        {
            _pendingCombatActionTarget = null;
            _actionMenuController.Close();
            _selectionController.ClearAction();
            _selectionController.ClearUnit();
        }
        
        private async UniTask TryMoveToAttackPosition( UnitController selectedUnit, UnitController targetUnit )
        {
            if ( selectedUnit == null ) return;
            if ( targetUnit == null ) return;
            if ( _battle.GetTeam( selectedUnit ) == _battle.GetTeam( targetUnit ) ) return;
            if ( !_gridController.Registry.TryGetUnitCell( targetUnit, out var targetCell ) ) return;

            var action = _actionFactory.Create< AttackAction >( selectedUnit );
            var target = new BattleActionTarget( targetCell, targetUnit );

            if ( !action.TryGetAttackDestination( target, out var destination ) ) return;

            var currentCell = _gridController.Registry.GetUnitCell( selectedUnit );
            if ( destination == currentCell )
            {
                _actionMenuController.Open( selectedUnit );
                return;
            }

            var moveAction = _actionFactory.Create< MoveAction >( selectedUnit );
            await TryMoveAndOpenActionMenu( moveAction, new BattleActionTarget( destination ) );
        }
        
        private async UniTask TryMoveAndOpenActionMenu( BattleAction action, BattleActionTarget target )
        {
            var selectedUnit = _selectionController.SelectedUnit;
            if ( selectedUnit == null ) return;

            if ( await TryExecuteAction( action, target ) )
            {
                _actionMenuController.Open( selectedUnit );
            }
        }
        
        private async UniTask TryExecuteSelectedAction( BattleActionTarget target )
        {
            var selectedUnit = _selectionController.SelectedUnit;
            if ( selectedUnit == null ) return;

            var action = _selectionController.SelectedAction;
            if ( action == null ) return;

            await TryExecute( action, target );
        }

        private void TryRequestCombatActionMenu( UnitController targetUnit )
        {
            if ( targetUnit == null ) return;
            if ( !_gridController.Registry.TryGetUnitCell( targetUnit, out var targetCell ) ) return;

            TryRequestCombatActionMenu( new BattleActionTarget( targetCell, targetUnit ) );
        }

        private void TryRequestCombatActionMenu( BattleCell targetCell )
        {
            if ( targetCell == null ) return;

            TryRequestCombatActionMenu( new BattleActionTarget( targetCell, _gridController.Registry.GetUnit( targetCell ) ) );
        }

        private void TryRequestCombatActionMenu( BattleActionTarget target )
        {
            var selectedUnit = _selectionController.SelectedUnit;
            if ( selectedUnit == null ) return;

            var targetUnit = target.Unit;
            if ( targetUnit == null ) return;

            var action = _selectionController.SelectedAction;
            if ( action == null ) return;
            if ( !CanExecuteSelectedActionTarget( action, selectedUnit, target ) ) return;

            _pendingCombatActionTarget = target;

            OnCombatActionMenuRequested?.Invoke( selectedUnit, targetUnit );
        }

        private bool CanExecuteSelectedActionTarget( BattleAction action, UnitController selectedUnit, BattleActionTarget target )
        {
            if ( action is RangeAttackAction )
            {
                return _combatController.IsCanAttackWithAnyWeaponFromCurrentCell( selectedUnit, target.Unit );
            }

            return action.CanExecute( target );
        }
        
        private bool IsSelectingActionTarget()
        {
            return _selectionController.SelectedAction is RangeAttackAction;
        }
    }
}
