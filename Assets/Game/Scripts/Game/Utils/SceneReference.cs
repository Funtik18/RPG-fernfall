using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Utils
{
    /// <summary>
    /// Сериализуемая ссылка на сцену. В редакторе хранит прямую ссылку на ассет
    /// сцены (SceneAsset) — её можно перетащить в инспектор. В рантайме отдаёт имя
    /// и путь сцены для загрузки через SceneManager, не завязываясь на строку.
    /// </summary>
    [Serializable]
    public sealed class SceneReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        [Tooltip("Ассет сцены (.unity). Перетащите сцену сюда.")]
        [SerializeField] private SceneAsset _sceneAsset;
#endif
        [SerializeField, HideInInspector] private string _scenePath;
        [SerializeField, HideInInspector] private string _sceneName;

        /// <summary>Имя сцены (без пути и расширения) для SceneManager.LoadScene.</summary>
        public string SceneName => _sceneName;

        /// <summary>Полный путь к ассету сцены (например, Assets/Scenes/GamePlay.unity).</summary>
        public string ScenePath => _scenePath;

        /// <summary>true, если сцена назначена.</summary>
        public bool IsAssigned => !string.IsNullOrEmpty(_sceneName);

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // Синхронизируем строковые поля с ассетом сцены, чтобы они были
            // доступны в рантайме (где SceneAsset уже не существует).
            if (_sceneAsset != null)
            {
                _scenePath = AssetDatabase.GetAssetPath(_sceneAsset);
                _sceneName = System.IO.Path.GetFileNameWithoutExtension(_scenePath);
            }
            else
            {
                _scenePath = string.Empty;
                _sceneName = string.Empty;
            }
#endif
        }

        public void OnAfterDeserialize() { }

        public override string ToString() => _sceneName;
    }
}
