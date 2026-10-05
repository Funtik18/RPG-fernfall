using System;
using UnityEngine;

namespace Game.UI.HUDBattleScreen
{
    public sealed class UIActionMenu : MonoBehaviour
    {
        public event Action OnAttackButtonClicked;
        public event Action OnUnitButtonClicked;
        public event Action OnItemButtonClicked;
        public event Action OnWaitButtonClicked;
        public event Action OnCancelButtonClicked;
        
        public void OnAttackButtonClick() => OnAttackButtonClicked?.Invoke();
        public void OnUnitButtonClick() => OnUnitButtonClicked?.Invoke();
        public void OnItemButtonClick() => OnItemButtonClicked?.Invoke();
        public void OnWaitButtonClick() => OnWaitButtonClicked?.Invoke();
        public void OnCancelButtonClick() => OnCancelButtonClicked?.Invoke();
    }
}