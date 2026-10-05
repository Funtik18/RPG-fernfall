using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Utils
{
    public static class UIUtils
    {
        private static readonly List< RaycastResult > _uiHits = new();

        public static bool IsOverUI( Vector2 screenPoint )
        {
            if ( EventSystem.current == null ) return false;

            var eventData = new PointerEventData( EventSystem.current ) { position = screenPoint };

            _uiHits.Clear();
            EventSystem.current.RaycastAll( eventData, _uiHits );

            return _uiHits.Count > 0;
        }
    }
}
