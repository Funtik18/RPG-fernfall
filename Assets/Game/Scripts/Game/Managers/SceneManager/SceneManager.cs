using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Managers.SceneManager
{
    public sealed class SceneManager
    {
        private string _lastSceneName;
        
        private readonly ZenjectSceneLoader _sceneLoader;

        public SceneManager( ZenjectSceneLoader sceneLoader )
        {
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException( nameof(sceneLoader) );
        }

        public async UniTask LoadSceneAdditive( string sceneName, Action callback = null )
        {
            _lastSceneName = sceneName;
            
            await _sceneLoader.LoadSceneAsync( sceneName, LoadSceneMode.Additive, extraBindings: null, LoadSceneRelationship.Child ).ToUniTask();

            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName( Path.GetFileNameWithoutExtension( sceneName ) );
            UnityEngine.SceneManagement.SceneManager.SetActiveScene( scene );
            
            callback?.Invoke();

            // var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Single );
            // if ( op == null )
            // {
            //     Debug.LogError( $"[SceneManager] Failed to load scene: {sceneName}" );
            // }
        }

        public void UnloadLastScene()
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName( Path.GetFileNameWithoutExtension( _lastSceneName ) );
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync( scene );
        }
    }
}