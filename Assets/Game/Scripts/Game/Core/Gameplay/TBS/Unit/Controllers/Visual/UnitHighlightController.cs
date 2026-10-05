using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class UnitHighlightController
    {
        private MeshRenderer _meshRenderer;
        
        public UnitHighlightController( UnitObject view )
        {
            _meshRenderer = view.Highlighter;

            Clear();
        }
        
        public void Highlight()
        {
            Show();
        }

        public void Clear()
        {
            _meshRenderer.enabled = false;
        }

        private void Show()
        {
            _meshRenderer.enabled = true;
        }
    }
}