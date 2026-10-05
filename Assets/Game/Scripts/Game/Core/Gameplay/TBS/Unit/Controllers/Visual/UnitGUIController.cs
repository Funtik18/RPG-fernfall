using Game.Core.Systems.SheetSystem;
using System.Threading;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitGUIController
    {
        private Sheet _sheet;
        private CancellationTokenSource _tickCancellationTokenSource;
        
        private readonly UnitGUI _view;

        public UnitGUIController( UnitObject view )
        {
            _view = view.GUI;
        }

        public void Initialize( Sheet sheet )
        {
            _sheet = sheet;
            _view.FuryBar.gameObject.SetActive( _sheet.Fraction.IsFeelFury );
            _view.FearBar.gameObject.SetActive( _sheet.Fraction.IsFeelFear );
            _sheet.Stats.HealthPoints.OnChanged += HealthPointsChangedHandler;
            _sheet.Stats.RagePoints.OnChanged += MoraleChangedHandler;
            _sheet.Stats.FearPoints.OnChanged += MoraleChangedHandler;

            UpdateHealthPoints();
            UpdateMorale();
        }

        public void Dispose()
        {
            _tickCancellationTokenSource?.Cancel();
            _tickCancellationTokenSource?.Dispose();
            _tickCancellationTokenSource = null;

            _sheet.Stats.HealthPoints.OnChanged -= HealthPointsChangedHandler;
            _sheet.Stats.RagePoints.OnChanged -= MoraleChangedHandler;
            _sheet.Stats.FearPoints.OnChanged -= MoraleChangedHandler;
        }

        public void Enable( bool trigger )
        {
            _view.gameObject.SetActive( trigger );
        }

        private void HealthPointsChangedHandler()
        {
            UpdateHealthPoints();
        }

        private void MoraleChangedHandler()
        {
            UpdateMorale();
        }

        private void UpdateHealthPoints()
        {
            if ( _view.HealthBar != null )
            {
                _view.HealthBar.SetBar( _sheet.Stats.HealthPoints.PercentValue );
            }
        }

        private void UpdateMorale()
        {
            _view.FuryBar.SetBar( _sheet.Stats.RagePoints.PercentValue );
            _view.FearBar.SetBar( _sheet.Stats.FearPoints.PercentValue );
        }
    }
}
