using SoosvetGames.VVM;
using UnityEngine;

namespace Game.UI
{
    public abstract class UIScreen : UIViewFade
    {
        [ field: SerializeField ] public Canvas Canvas { get; private set; }
    }
}