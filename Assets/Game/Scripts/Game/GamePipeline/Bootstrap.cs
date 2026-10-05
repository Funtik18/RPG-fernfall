using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Managers.InputManager;
using Game.Managers.SceneManager;
using Game.Systems.CameraSystem;
using Game.Systems.StorageSystem;
using Game.UI;
using System;
using UnityEngine;

namespace Game.Pipeline
{
    public sealed class Bootstrap
    {
        private readonly SceneManager _sceneManager;
        private readonly UIManager _uiManager;
        private readonly DataHolder _dataHolder;
        private readonly CameraFacingService _cameraFacingService;
        
        public Bootstrap(
            SceneManager sceneManager,
            UIManager uiManager,
            DataHolder dataHolder,
            CameraFacingService cameraFacingService
            )
        {
            _sceneManager = sceneManager ?? throw new ArgumentNullException( nameof(sceneManager) );
            _uiManager = uiManager ?? throw new ArgumentNullException( nameof(uiManager) );
            _dataHolder = dataHolder ?? throw new ArgumentNullException( nameof(dataHolder) );
            _cameraFacingService = cameraFacingService ?? throw new ArgumentNullException( nameof(cameraFacingService) );
            
            Initialize();
        }
        
        private void Initialize()
        {
            Debug.Log( "[GamePipeline] Bootstrap" );
         
            var preferences = _dataHolder.GameStorageData.PreferencesData.Value;
            if ( preferences == null )
            {
                _dataHolder.GameStorageData.PreferencesData.SetData( new PreferencesData()
                {
                    Music = true,
                    Sound = true,
                    Vibration = true,
                } );
            }
            
            _uiManager.Initialize();
            _cameraFacingService.Initialize();
            
            InputManager.Initialize();

#if UNITY_EDITOR
            _sceneManager.LoadSceneAdditive( BootstrapPlayMode.GetTargetSceneOrBootstrapMenuScene() ).Forget();
#else
            _sceneManager.LoadSceneAdditive( SceneKeys.MAIN_MENU ).Forget();
#endif
        }
    }
}