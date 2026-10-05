#if UNITY_EDITOR
using Game.Managers.SceneManager;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Game.Pipeline
{
    /// <summary>
    /// Перехватываем в редакторе запуск сцены и сохраняем в сессию string TARGET_SCENE_PATH
    /// Используется для того что бы Bootstrap сцена запускалась первой а потом уже выбранная сцена.
    /// </summary>
    [ InitializeOnLoad ]
    public static class BootstrapPlayMode
    {
        static BootstrapPlayMode()
        {
            EditorApplication.playModeStateChanged -= PlayModeStateChangedHandler;
            EditorApplication.playModeStateChanged += PlayModeStateChangedHandler;
            
            var scene = SceneUtils.FindScene( SceneKeys.BOOTSTRAP );
            EditorSceneManager.playModeStartScene = scene;
        }

        private static void PlayModeStateChangedHandler( PlayModeStateChange state )
        {
            if ( state != PlayModeStateChange.ExitingEditMode ) return;

            Scene activeScene = SceneManager.GetActiveScene();
            string targetScenePath = activeScene.path.Contains( SceneKeys.BOOTSTRAP ) ? string.Empty : activeScene.path;

            SessionState.SetString( SessionKeys.TARGET_SCENE_PATH, targetScenePath );
        }

        public static string GetTargetSceneOrBootstrapMenuScene()
        {
            string editorScenePath = SessionState.GetString( SessionKeys.TARGET_SCENE_PATH, string.Empty );
            SessionState.EraseString( SessionKeys.TARGET_SCENE_PATH );

            if ( !string.IsNullOrEmpty( editorScenePath ) ) return editorScenePath;

            return SceneKeys.MAIN_MENU;
        }
    }
}
#endif