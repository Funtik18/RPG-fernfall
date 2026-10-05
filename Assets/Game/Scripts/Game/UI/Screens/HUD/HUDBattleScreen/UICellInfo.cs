using Game.Core.Gameplay.TBS;
using UnityEngine;

namespace Game.UI.HUDBattleScreen
{
    public sealed class UICellInfo : MonoBehaviour
    {
        [ SerializeField ] private TMPro.TextMeshProUGUI _cellTitle;
        [ SerializeField ] private TMPro.TextMeshProUGUI _cellEffect1;
        [ SerializeField ] private TMPro.TextMeshProUGUI _cellEffect2;
        
        public void SetCell( GridCellTerrainConfig config )
        {
            if ( config == null )
            {
                _cellTitle.text = string.Empty;
                _cellEffect1.text = string.Empty;
                _cellEffect2.text = string.Empty;
                return;
            }

            _cellTitle.text = config.Name;
            _cellEffect1.text = config.MoveCost.ToString();
            _cellEffect2.text = config.Avoid.ToString();
        }
    }
}
