using TMPro;
using UnityEngine;
using TurnBasedStrategyFramework.Common.Controllers;
using TurnBasedStrategyFramework.Unity.Controllers;

namespace Fire.UI.CombatHUD
{
    // Minimal turn indicator - no equivalent exists ready-made in TBSF's examples or the Fantasy
    // Warrior HUD pack (confirmed in Phase 1: only Example1/ClashOfHeroes have one at all, both
    // just as bare as this). Whose-turn-it-is only, not a full initiative queue - TBSF doesn't
    // expose one.
    public class TurnIndicatorController : MonoBehaviour
    {
        public UnityGridController GridController;
        public TMP_Text TurnText;

        void OnEnable()
        {
            if (GridController == null) return;
            GridController.TurnStarted += OnTurnStarted;

            // GridController.StartGame() fires TurnStarted once immediately at game start
            // (before any real end-turn transition) - if that already happened before this
            // component enabled, reflect the current state instead of showing stale/blank text.
            RefreshFromCurrentTurn();
        }

        void OnDisable()
        {
            if (GridController != null)
                GridController.TurnStarted -= OnTurnStarted;
        }

        void OnTurnStarted(TurnTransitionParams args) => SetText(args.TurnContext.CurrentPlayer.PlayerNumber);

        void RefreshFromCurrentTurn()
        {
            // TurnContext is a struct - before StartGame() has run it's default(TurnContext),
            // whose CurrentPlayer (a reference type) is null. That's the "game hasn't started
            // yet" signal, not TurnContext itself being null.
            var currentPlayer = GridController.TurnContext.CurrentPlayer;
            if (currentPlayer == null) return;
            SetText(currentPlayer.PlayerNumber);
        }

        void SetText(int currentPlayerNumber)
        {
            if (TurnText == null) return;
            TurnText.text = currentPlayerNumber == 0 ? "Ваш ход" : "Ход противника";
        }
    }
}
