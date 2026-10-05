using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TurnBasedStrategyFramework.Common.Controllers.GridStates;
using TurnBasedStrategyFramework.Common.Units;
using TurnBasedStrategyFramework.Common.Units.Abilities;
using TurnBasedStrategyFramework.Unity.Controllers;
using TurnBasedStrategyFramework.Unity.Units;
using TurnBasedStrategyFramework.Unity.Units.Abilities;

namespace Fire.UI.CombatHUD
{
    // Screen-space action menu (Attack / End Turn) for the selected unit. Move isn't a button
    // here - clicking a reachable cell already moves, per the task's own scope. Deliberately NOT
    // built as a TBSF Ability-that-renders-a-menu (ClashOfHeroes' AbilityMenu pattern) - that
    // couples the menu to being registered as one of the unit's own base abilities. This is a
    // plain external controller instead, reading GetBaseAbilities()/IsUnitAttackable the same
    // way AttackZoneHighlightController already reads reachable/attack-range data, so it needs
    // no TBSF/prefab changes.
    public class ActionMenuController : MonoBehaviour
    {
        public UnityUnitManager UnitManager;
        public UnityGridController GridController;
        public GameObject Panel;
        public Button AttackButton;
        public Button EndTurnButton;

        IUnit _current;

        void Awake()
        {
            if (AttackButton != null) AttackButton.onClick.AddListener(OnAttackClicked);
            if (EndTurnButton != null) EndTurnButton.onClick.AddListener(OnEndTurnClicked);
        }

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
            _current = unit;

            // Only the currently active player's own unit gets an action menu - selecting an
            // enemy unit (which this project's click flow allows, for inspection) must not offer
            // to act on someone else's turn.
            bool isOwnActiveUnit = GridController != null
                && GridController.TurnContext.CurrentPlayer.PlayerNumber == unit.PlayerNumber;

            if (Panel != null) Panel.SetActive(isOwnActiveUnit);
            if (isOwnActiveUnit) RefreshAttackInteractable();
        }

        void OnUnitDeselected(IUnit unit)
        {
            if (!ReferenceEquals(unit, _current)) return;
            _current = null;
            if (Panel != null) Panel.SetActive(false);
        }

        List<IUnit> AttackableEnemies()
        {
            if (_current?.CurrentCell == null || GridController == null)
                return new List<IUnit>();

            return GridController.UnitManager.GetEnemyUnits(_current.PlayerNumber)
                .Where(enemy => enemy.CurrentCell != null
                                 && _current.IsUnitAttackable(enemy, enemy.CurrentCell, _current.CurrentCell))
                .ToList();
        }

        void RefreshAttackInteractable()
        {
            if (AttackButton == null) return;
            AttackButton.interactable = _current != null && _current.ActionPoints > 0 && AttackableEnemies().Count > 0;
        }

        void OnAttackClicked()
        {
            if (_current == null || GridController == null) return;
            var targets = AttackableEnemies();

            if (targets.Count == 1)
            {
                var enemy = targets[0];
                _current.HumanExecuteAbility(new AttackCommand(enemy, _current.CalculateTotalDamage(enemy)), GridController);
            }
            else if (targets.Count > 1)
            {
                // Narrows the grid state to just the unit's Attack ability, mirroring
                // ClashOfHeroes' AbilityMenu re-selection trick, so the next click can only
                // resolve as an attack rather than risking a move. NOT exercised live in this
                // task - Test_SyntyOnGrid has exactly one enemy unit, so this branch is an
                // untested code path pending a real multi-enemy scene.
                var attackAbility = _current.GetBaseAbilities().FirstOrDefault(a => a is AttackAbility);
                if (attackAbility != null)
                    GridController.GridState = new GridStateUnitSelected(_current, attackAbility);
            }
        }

        void OnEndTurnClicked()
        {
            GridController?.EndTurn();
        }
    }
}
