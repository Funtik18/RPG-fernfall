using UnityEngine;

namespace Game.UI.HUDBattleScreen
{
    public sealed class UICombatStrikeForecast : MonoBehaviour
    {
        [ SerializeField ] private TMPro.TextMeshProUGUI _attackerHit;
        [ SerializeField ] private TMPro.TextMeshProUGUI _attackerCrit;
        [ SerializeField ] private TMPro.TextMeshProUGUI _defenderDamage;
        [ SerializeField ] private GameObject _defenderKill;

        public void Set( int attackerHit, int attackerCrit, int defenderDamage, bool isDefenderKill )
        {
            _attackerHit.text = attackerHit.ToString();
            _attackerCrit.text = attackerCrit.ToString();
            _defenderDamage.text = defenderDamage.ToString();
            _defenderKill.SetActive( isDefenderKill );
        }
    }
}