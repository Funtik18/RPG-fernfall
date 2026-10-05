// using Game.Managers.AudioManager;
// using Game.Managers.VibrationManager;
using System;
using Random = UnityEngine.Random;

namespace Game.UI.Services
{
    public sealed class UIFeedbackService
    {
        // private readonly AudioSettings _audioSettings;
        // private readonly AudioManager _audioManager;
        // private readonly VibrationManager _vibrationManager;
        
        public UIFeedbackService(
            // AudioSettings audioSettings,
            // AudioManager audioManager,
            // VibrationManager vibrationManager
            )
        {
            // _audioSettings = audioSettings ?? throw new ArgumentNullException( nameof(audioSettings) );
            // _audioManager = audioManager ?? throw new ArgumentNullException( nameof(audioManager) );
            // _vibrationManager = vibrationManager ?? throw new ArgumentNullException( nameof(vibrationManager) );
        }
        
        public void PlayUIButton()
        {
            // _audioManager.PlaySound( _audioSettings.SoundClickUI, volume: Random.Range( 0.5f, 1f ), pitch: Random.Range( 0.8f, 1.3f ) );
            // _vibrationManager.Vibrate();
        }
    }
}