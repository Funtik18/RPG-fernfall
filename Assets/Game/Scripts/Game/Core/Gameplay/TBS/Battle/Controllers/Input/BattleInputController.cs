using Cysharp.Threading.Tasks;
using Game.Managers.InputManager;
using Game.Utils;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattleInputController
    {
        public event Action< UnitController > OnUnitClicked;
        public event Action< BattleCell > OnCellClicked;
        public event Action< BattleCell > OnCellHovered;
        public event Action< BattleCell > OnCellHoverExited;
        public event Action< bool > OnPointerOverUIChanged;
        
        public GridCellObject HoveredCell { get; private set; }
        public bool IsPointerOverUI { get; private set; }
        
        private Camera _camera;
        private CancellationTokenSource _cancellationTokenSource;
        
        private readonly BattleGridController _gridController;
        
        public BattleInputController( BattleGridController gridController )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
        }
        
        public void Initialize()
        {
            _camera = Camera.main;
            
            InputManager.Inputs.Camera.Point.Enable();
            InputManager.Inputs.Battle.Select.Enable();
            
            InputManager.Inputs.Battle.Select.performed += SelectedHandler;
            SetPointerOverUI();

            _cancellationTokenSource = new();
            Tick( _cancellationTokenSource.Token ).Forget();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            
            InputManager.Inputs.Camera.Point.Disable();
            InputManager.Inputs.Battle.Select.Disable();
            
            InputManager.Inputs.Battle.Select.performed -= SelectedHandler;
        }

        private async UniTask Tick( CancellationToken cancellationToken = default )
        {
            while ( !cancellationToken.IsCancellationRequested )
            {
                cancellationToken.ThrowIfCancellationRequested();

                SetPointerOverUI();
                if ( IsPointerOverUI )
                {
                    SetHoveredCell( null );
                    await UniTask.Yield( PlayerLoopTiming.Update, cancellationToken );
                    continue;
                }

                var collider = GetColliderUnderPointer();
                if( collider != null )
                {
                    var cell = GetCellUnderPointer( collider );
                    SetHoveredCell( cell );

                    var unit = GetUnitUnderPointer( collider );
                    if ( unit != null )
                    {
                        cell = _gridController.Registry.GetUnitCell( unit.Controller ).View;
                        SetHoveredCell( cell );
                    }
                }
                
                await UniTask.Yield( PlayerLoopTiming.Update, cancellationToken );
            }
        }

        private Collider GetColliderUnderPointer()
        {
            Vector2 pointerPosition = InputManager.GetPoint();
            Ray ray = _camera.ScreenPointToRay( pointerPosition );

            if ( !Physics.Raycast( ray, out var hit ) ) return null;
            return hit.collider;
        }
        
        private UnitObject GetUnitUnderPointer( Collider collider ) => collider.GetComponentInParent< UnitObject >();

        private GridCellObject GetCellUnderPointer( Collider collider ) => collider.GetComponentInParent< GridCellObject >();

        private void SetHoveredCell( GridCellObject cell )
        {
            if ( HoveredCell == cell ) return;

            if ( HoveredCell != null )
            {
                HoveredCell.OnPointerExit();
                OnCellHoverExited?.Invoke( _gridController.GetCell( HoveredCell ) );
            }
            HoveredCell = cell;
            if ( HoveredCell != null )
            {
                HoveredCell.OnPointerEnter();
                OnCellHovered?.Invoke( _gridController.GetCell( HoveredCell ) );
            }
        }

        private void SelectedHandler( InputAction.CallbackContext context )
        {
            SetPointerOverUI();
            if ( IsPointerOverUI ) return;

            var collider = GetColliderUnderPointer();
            if ( collider == null ) return;

            var unit = GetUnitUnderPointer( collider );
            if ( unit != null )
            {
                OnUnitClicked?.Invoke( unit.Controller );
                return;
            }

            var cell = GetCellUnderPointer( collider );
            if ( cell != null )
            {
                OnCellClicked?.Invoke( _gridController.GetCell( cell ) );
            }
        }

        private void SetPointerOverUI()
        {
            bool isOverUI = UIUtils.IsOverUI( InputManager.GetPoint() );
            if ( IsPointerOverUI == isOverUI ) return;

            IsPointerOverUI = isOverUI;
            OnPointerOverUIChanged?.Invoke( IsPointerOverUI );
        }
    }
}
