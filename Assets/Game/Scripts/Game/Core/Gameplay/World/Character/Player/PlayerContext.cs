using System;

namespace Game.Core.Gameplay.World.Player
{
    public sealed class PlayerContext
    {
        public PlayerTriggerController TriggerController { get; }
        
        private readonly PlayerInputController _inputController;
        private readonly PlayerMovementController _movementController;
        private readonly PlayerCameraController _cameraController;
        
        public PlayerContext(
            PlayerInputController inputController,
            PlayerMovementController movementController,
            PlayerCameraController cameraController,
            
            PlayerTriggerController triggerController
            )
        {
            _inputController = inputController ?? throw new ArgumentNullException( nameof(inputController) );
            _movementController = movementController ?? throw new ArgumentNullException( nameof(movementController) );
            _cameraController = cameraController ?? throw new ArgumentNullException( nameof(cameraController) );
        
            TriggerController = triggerController ?? throw new ArgumentNullException( nameof(triggerController) );
        }
        
        public void Initialize()
        {
            _inputController.Initialize();
            _movementController.Initialize();
            _cameraController.Initialize();
            
            TriggerController.Initialize();
        }

        public void Dispose()
        {
            _inputController.Dispose();
            _movementController.Dispose();
            _cameraController.Dispose();
            
            TriggerController.Dispose();
        }
    }
}