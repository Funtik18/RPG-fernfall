using SoosvetGames.VVM;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    [ System.Serializable ]
    public sealed class UISettings
    {
        [ field: SerializeField ] public UIRoot RootPrefab { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public List< View > Screens { get; private set; } = new();
        [ field: SerializeField ] public List< View > Dialogs { get; private set; } = new();
    }
}