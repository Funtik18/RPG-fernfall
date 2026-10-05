using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Unity.Units;
using Fire.Gameplay.TestGrid;

namespace Fire.UI.CombatHUD
{
    // Screen-space panel showing the currently selected unit's name/HP, modeled on
    // ClashOfHeroes' UnitDetailsDisplay (a fixed-position HUD panel driven by UnitSelected/
    // UnitDeselected + HealthChanged) rather than a world-space Canvas on the unit - the
    // per-unit floating health bars are a separate, non-conflicting layer that stays as-is.
    public class SelectedUnitPanelController : MonoBehaviour
    {
        public UnityUnitManager UnitManager;
        public GameObject Panel;
        public TMP_Text NameText;
        public TMP_Text HealthText;
        public Slider HealthSlider;

        IUnit _current;

        void OnEnable()
        {
            if (UnitManager != null)
                UnitManager.UnitAdded += Subscribe;
            if (Panel != null)
                Panel.SetActive(false);
        }

        void OnDisable()
        {
            if (UnitManager != null)
                UnitManager.UnitAdded -= Subscribe;
        }

        void Subscribe(IUnit unit)
        {
            if (unit is Unit u)
            {
                u.UnitSelected += OnUnitSelected;
                u.UnitDeselected += OnUnitDeselected;
            }
        }

        void OnUnitSelected(IUnit unit)
        {
            if (_current is Unit previous)
                previous.HealthChanged -= OnHealthChanged;

            _current = unit;
            if (unit is Unit u)
                u.HealthChanged += OnHealthChanged;

            Refresh();
            if (Panel != null) Panel.SetActive(true);
        }

        void OnUnitDeselected(IUnit unit)
        {
            if (!ReferenceEquals(unit, _current)) return;

            if (unit is Unit u)
                u.HealthChanged -= OnHealthChanged;
            _current = null;
            if (Panel != null) Panel.SetActive(false);
        }

        void OnHealthChanged(HealthChangedEventArgs args) => Refresh();

        void Refresh()
        {
            if (_current == null) return;

            var displayName = (_current as TestGridUnit)?.DisplayName;
            if (NameText != null)
                NameText.text = string.IsNullOrEmpty(displayName) ? ((Component)_current).gameObject.name : displayName;

            if (HealthText != null)
                HealthText.text = $"{Mathf.CeilToInt(Mathf.Max(_current.Health, 0f))} / {Mathf.CeilToInt(_current.MaxHealth)}";

            if (HealthSlider != null)
                HealthSlider.value = _current.MaxHealth > 0f ? Mathf.Clamp01(_current.Health / _current.MaxHealth) : 0f;
        }
    }
}
