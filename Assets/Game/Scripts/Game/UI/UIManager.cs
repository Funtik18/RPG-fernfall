using SoosvetGames.VVM;
using System;
using UnityEngine;
using Zenject;

namespace Game.UI
{
    public sealed class UIManager
    {
        public UIRoot Root { get; private set; }
        
        public ViewModelAggregator ScreenAggregator { get; private set; }
        public ViewModelAggregator DialogAggregator { get; private set; }
        // public ViewModelAggregator TutorialAggregator { get; private set; }
        
        private readonly UISettings _settings;
        private readonly DiContainer _diContainer;
        
        public UIManager(
            UISettings settings,
            DiContainer diContainer
            )
        {
            _settings = settings ?? throw new ArgumentNullException( nameof(settings) );
            _diContainer = diContainer ?? throw new ArgumentNullException( nameof(diContainer) );
        }

        public void Initialize()
        {
            Root = GameObject.Instantiate( _settings.RootPrefab );
            GameObject.DontDestroyOnLoad( Root );
            
            ScreenAggregator = ViewModelAggregator.Create( _diContainer, Root.ScreensRoot, _settings.Screens );
            DialogAggregator = ViewModelAggregator.Create( _diContainer, Root.DialogsRoot, _settings.Dialogs );
            // TutorialAggregator = ViewModelAggregator.Create( diContainer, DynamicCanvas.TutorialsRoot, _settings.Tutorials );
        }
    }
}