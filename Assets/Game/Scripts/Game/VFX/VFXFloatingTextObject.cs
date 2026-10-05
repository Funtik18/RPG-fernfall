using SoosvetGames.IoC;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.VFX
{
    public sealed class VFXFloatingTextObject : ZenjectMonoPoolable
    {
        [ field: SerializeField ] public TextMeshPro DamageText { get; private set; }
        [ field: SerializeField ] public TextMeshPro MissText { get; private set; }
        [ field: SerializeField ] public TextMeshPro CritText { get; private set; }
        
        public void SetText( string text )
        {
            DamageText.text = text;
            MissText.text = text;
        }

        public void EnableDamageText()
        {
            DamageText.gameObject.SetActive( true );
            MissText.gameObject.SetActive( false );
            CritText.gameObject.SetActive( false );
        }
        
        public void EnableMissText()
        {
            DamageText.gameObject.SetActive( false );
            MissText.gameObject.SetActive( true );
            CritText.gameObject.SetActive( false );
        }
        
        public void EnableCritText()
        {
            DamageText.gameObject.SetActive( false );
            MissText.gameObject.SetActive( false );
            CritText.gameObject.SetActive( true );
        }

        public class Factory : PlaceholderFactory< VFXFloatingTextObject > {}
    }
}