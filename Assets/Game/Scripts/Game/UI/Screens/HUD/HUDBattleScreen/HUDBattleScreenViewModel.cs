using Game.Core.Gameplay.TBS;
using Game.Core.Systems.SheetSystem;
using Game.Managers.BattleManager;
using Game.Managers.InputManager;
using R3;
using SoosvetGames.VVM;
using System;
using System.Collections.Generic;

namespace Game.UI.HUDBattleScreen
{
    public sealed class HUDBattleScreenViewModel : ViewModel< HUDBattleScreen >
    {
        private InputActionVoid _confirmAction;
        private InputAction1DVoid _selectorMain;
        private InputAction1DVoid _selectorAdd;
        private readonly List< UnitController > _selectableAlliedUnits = new();
        private BattleCell _hoveredCell;
        
        private readonly BattleManager _battleManager;
        private readonly WeaponRules _weaponRules;
        
        public HUDBattleScreenViewModel(
            BattleManager battleManager,
            WeaponRules weaponRules
            )
        {
            _battleManager = battleManager ?? throw new ArgumentNullException( nameof(battleManager) );
            _weaponRules = weaponRules ?? throw new ArgumentNullException( nameof(weaponRules) );
        }
        
        protected override void SubscribeView()
        {
            base.SubscribeView();

            _battleManager.BattleController.OnUnitActionMenuRequested += UnitActionMenuRequestedHandler;
            _battleManager.BattleController.OnCombatActionMenuRequested += CombatActionMenuRequestedHandler;
            _battleManager.BattleController.OnCellHovered += CellHoveredHandler;
            _battleManager.BattleController.OnCellHoverExited += CellHoverExitedHandler;
            _battleManager.BattleController.BattleSelection.OnUnitSelected += UnitSelectedHandler;
            _battleManager.BattleController.BattleSelection.OnUnitDeselected += UnitDeselectedHandler;
            
            ModelView.OnTurnButtonClicked += TurnButtonClickedHandler;
            ModelView.OnRunButtonClicked += RunButtonClickedHandler;
            ModelView.ActionMenu.OnAttackButtonClicked += AttackButtonClickedHandler;
            ModelView.ActionMenu.OnWaitButtonClicked += WaitButtonClickedHandler;
            ModelView.CombatForecast.OnAttackButtonClicked += CombatAttackButtonClickedHandler;
            ModelView.CombatForecast.OnBackButtonClicked += CombatBackButtonClickedHandler;
            ModelView.CombatForecast.OnAttackerWeaponChanged += WeaponChangedHandler;
            ModelView.UnitInfo.OnNextUnitButtonClicked += NextUnitButtonClickedHandler;
            ModelView.UnitInfo.OnPrevUnitButtonClicked += PrevUnitButtonClickedHandler;
            ModelView.UnitInfo.OnNextWeaponButtonClicked += WeaponChangedHandler;
            ModelView.UnitInfo.OnPrevWeaponButtonClicked += WeaponChangedHandler;

            _confirmAction = new( InputManager.Inputs.UI.ConfirmAction, CombatAttackButtonClickedHandler );
            _selectorMain = new( InputManager.Inputs.UI.SelectorMain, SelectorMainNegativeHandler, SelectorMainPositiveHandler );
            _selectorAdd = new( InputManager.Inputs.UI.SelectorAdd, SelectorAddNegativeHandler, SelectorAddPositiveHandler );
            RefreshInputActions();
        }

        protected override void UnSubscribeView()
        {
            base.UnSubscribeView();
            
            _battleManager.BattleController.OnUnitActionMenuRequested -= UnitActionMenuRequestedHandler;
            _battleManager.BattleController.OnCombatActionMenuRequested -= CombatActionMenuRequestedHandler;
            _battleManager.BattleController.OnCellHovered -= CellHoveredHandler;
            _battleManager.BattleController.OnCellHoverExited -= CellHoverExitedHandler;
            _battleManager.BattleController.BattleSelection.OnUnitSelected -= UnitSelectedHandler;
            _battleManager.BattleController.BattleSelection.OnUnitDeselected -= UnitDeselectedHandler;
            
            ModelView.OnTurnButtonClicked -= TurnButtonClickedHandler;
            ModelView.OnRunButtonClicked -= RunButtonClickedHandler;
            ModelView.ActionMenu.OnAttackButtonClicked -= AttackButtonClickedHandler;
            ModelView.ActionMenu.OnWaitButtonClicked -= WaitButtonClickedHandler;
            ModelView.CombatForecast.OnAttackButtonClicked -= CombatAttackButtonClickedHandler;
            ModelView.CombatForecast.OnBackButtonClicked -= CombatBackButtonClickedHandler;
            ModelView.CombatForecast.OnAttackerWeaponChanged -= WeaponChangedHandler;
            ModelView.UnitInfo.OnNextUnitButtonClicked -= NextUnitButtonClickedHandler;
            ModelView.UnitInfo.OnPrevUnitButtonClicked -= PrevUnitButtonClickedHandler;
            ModelView.UnitInfo.OnNextWeaponButtonClicked -= WeaponChangedHandler;
            ModelView.UnitInfo.OnPrevWeaponButtonClicked -= WeaponChangedHandler;

            _confirmAction?.Dispose();
            _selectorMain?.Dispose();
            _selectorAdd?.Dispose();
        }

        protected override void OnViewShowingChanged()
        {
            if ( !ModelView.IsShowing )
            {
                ModelView.UnitInfo.EnableUnitSelectorButtons( false );
                ModelView.CellInfo.gameObject.SetActive( false );
                _hoveredCell = null;
                DisableInputActions();
                return;
            }
            
            ModelView.UnitInfo.Enable( false );
            ModelView.UnitInfo.EnableUnitSelectorButtons( false );
            _hoveredCell = null;
            RefreshCellInfo();
            ModelView.ActionMenu.gameObject.SetActive( false );
            ModelView.CombatForecast.Initialize( _battleManager.BattleController, _weaponRules );
            ModelView.CombatForecast.Enable( false );
            RefreshCellInfo();
            RefreshInputActions();
        }

        private void UnitSelectedHandler( UnitController unit )
        {
            ModelView.UnitInfo.Enable( true );
            ModelView.UnitInfo.SetUnit( unit );
            ModelView.ActionMenu.gameObject.SetActive( false );
            ModelView.CombatForecast.Enable( false );
            RefreshCellInfo();
            RefreshInputActions();
        }
        
        private void UnitDeselectedHandler( UnitController unit )
        {
            ModelView.UnitInfo.Enable( false );
            ModelView.UnitInfo.SetUnit( null );
            ModelView.UnitInfo.EnableUnitSelectorButtons( false );
            ModelView.ActionMenu.gameObject.SetActive( false );
            ModelView.CombatForecast.Enable( false );
            RefreshCellInfo();
            RefreshInputActions();
        }
        
        private void UnitActionMenuRequestedHandler( UnitController unit )
        {
            ModelView.ActionMenu.gameObject.SetActive( true );
            ModelView.CombatForecast.Enable( false );
            RefreshCellInfo();
            RefreshInputActions();
        }

        private void CombatActionMenuRequestedHandler( UnitController attacker, UnitController defender )
        {
            ModelView.UnitInfo.Enable( false );
            ModelView.UnitInfo.EnableUnitSelectorButtons( false );
            ModelView.ActionMenu.gameObject.SetActive( false );
            ModelView.CombatForecast.SetUnits( attacker, defender );
            ModelView.CombatForecast.Enable( true );
            RefreshCellInfo();
            RefreshInputActions();
        }

        private void TurnButtonClickedHandler()
        {
            ModelView.ActionMenu.gameObject.SetActive( false );
            ModelView.CombatForecast.Enable( false );
            RefreshCellInfo();
            RefreshInputActions();
            _battleManager.BattleController.Battle.EndCurrentTurn();
        }

        private void RunButtonClickedHandler()
        {
            _battleManager.RunBattle();
        }
        
        private void AttackButtonClickedHandler()
        {
            if ( _battleManager.BattleController.SelectAttackAction() )
            {
                ModelView.ActionMenu.gameObject.SetActive( false );
                ModelView.CombatForecast.Enable( false );
                RefreshCellInfo();
                RefreshInputActions();
            }
        }

        private void CombatAttackButtonClickedHandler()
        {
            if ( _battleManager.BattleController.ConfirmCombatAction( ModelView.CombatForecast.SelectedAttackerCombatArt ) )
            {
                ModelView.CombatForecast.Enable( false );
                RefreshCellInfo();
                RefreshInputActions();
            }
        }
        
        private void CombatBackButtonClickedHandler()
        {
            if ( _battleManager.BattleController.CancelCombatAction() )
            {
                ModelView.CombatForecast.Enable( false );
                RefreshCellInfo();
                RefreshInputActions();
            }
        }

        private void WaitButtonClickedHandler()
        {
            if ( _battleManager.BattleController.WaitSelectedUnit() )
            {
                ModelView.ActionMenu.gameObject.SetActive( false );
                ModelView.CombatForecast.Enable( false );
                RefreshCellInfo();
                RefreshInputActions();
            }
        }

        private void CellHoveredHandler( BattleCell cell )
        {
            _hoveredCell = cell;
            RefreshCellInfo();
        }

        private void CellHoverExitedHandler( BattleCell cell )
        {
            if ( _hoveredCell != cell ) return;

            _hoveredCell = null;
            RefreshCellInfo();
        }

        private void RefreshCellInfo()
        {
            if ( ModelView.CombatForecast.IsShowing || _hoveredCell == null )
            {
                ModelView.CellInfo.gameObject.SetActive( false );
                return;
            }

            ModelView.CellInfo.SetCell( _battleManager.BattleController.GetCellTerrainConfig( _hoveredCell ) );
            ModelView.CellInfo.gameObject.SetActive( true );
        }

        #region Selectors

        private void SelectorMainNegativeHandler()
        {
            if ( ModelView.CombatForecast.IsShowing )
            {
                ModelView.CombatForecast.OnLeftWeaponButtonClick();
                return;
            }

            if ( ModelView.UnitInfo.IsShowing )
            {
                ModelView.UnitInfo.OnPrevWeaponButtonClick();
            }
        }

        private void SelectorMainPositiveHandler()
        {
            if ( ModelView.CombatForecast.IsShowing )
            {
                ModelView.CombatForecast.OnRightWeaponButtonClick();
                return;
            }

            if ( ModelView.UnitInfo.IsShowing )
            {
                ModelView.UnitInfo.OnNextWeaponButtonClick();
            }
        }

        private void SelectorAddNegativeHandler()
        {
            if ( ModelView.CombatForecast.IsShowing )
            {
                ModelView.CombatForecast.OnLeftArtButtonClick();
                return;
            }

            if ( ModelView.UnitInfo.IsShowing )
            {
                SelectAlliedUnit( -1 );
            }
        }

        private void SelectorAddPositiveHandler()
        {
            if ( ModelView.CombatForecast.IsShowing )
            {
                ModelView.CombatForecast.OnRightArtButtonClick();
                return;
            }

            if ( ModelView.UnitInfo.IsShowing )
            {
                SelectAlliedUnit( 1 );
            }
        }

        private void NextUnitButtonClickedHandler()
        {
            SelectAlliedUnit( 1 );
        }

        private void PrevUnitButtonClickedHandler()
        {
            SelectAlliedUnit( -1 );
        }

        private void WeaponChangedHandler()
        {
            _battleManager.BattleController.RefreshSelectedUnitCellsPreview();
        }

        private void SelectAlliedUnit( int direction )
        {
            var units = GetSelectableAlliedUnits();
            if ( units.Count <= 1 ) return;

            var selectedUnit = _battleManager.BattleController.BattleSelection.SelectedUnit;
            var selectedIndex = units.IndexOf( selectedUnit );
            if ( selectedIndex < 0 )
            {
                selectedIndex = direction > 0 ? -1 : 0;
            }

            var nextIndex = ( selectedIndex + direction + units.Count ) % units.Count;
            if ( _battleManager.BattleController.TrySelectUnit( units[ nextIndex ] ) )
            {
                RefreshInputActions();
            }
        }

        private List< UnitController > GetSelectableAlliedUnits()
        {
            _selectableAlliedUnits.Clear();

            var battleController = _battleManager.BattleController;
            var turn = battleController.Battle.CurrentRound.CurrentTurn;
            if ( turn == null ) return _selectableAlliedUnits;

            foreach ( var unit in turn.Team.Units )
            {
                if ( !battleController.CanSelectUnit( unit ) ) continue;

                _selectableAlliedUnits.Add( unit );
            }

            return _selectableAlliedUnits;
        }
        #endregion
        
        private void RefreshInputActions()
        {
            var isCombatForecastShowing = ModelView.CombatForecast.IsShowing;
            var isUnitInfoShowing = ModelView.UnitInfo.IsShowing;
            var canSelectAlliedUnits = isUnitInfoShowing && ( GetSelectableAlliedUnits().Count > 1 );

            ModelView.UnitInfo.EnableUnitSelectorButtons( canSelectAlliedUnits );

            if ( isCombatForecastShowing )
            {
                _confirmAction.Enable();
            }
            else
            {
                _confirmAction.Disable();
            }

            if ( isCombatForecastShowing || canSelectAlliedUnits )
            {
                _selectorAdd.Enable();
            }
            else
            {
                _selectorAdd.Disable();
            }

            if ( isCombatForecastShowing || isUnitInfoShowing )
            {
                _selectorMain.Enable();
            }
            else
            {
                _selectorMain.Disable();
            }
        }

        private void DisableInputActions()
        {
            _confirmAction?.Disable();
            _selectorMain?.Disable();
            _selectorAdd?.Disable();
        }
    }
}
