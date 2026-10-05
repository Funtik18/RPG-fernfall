using UnityEngine;

namespace Game.UI
{
    public sealed class UIRoot : MonoBehaviour
    {
        [ field: SerializeField ] public Transform ScreensRoot { get; private set; }
        [ field: SerializeField ] public Transform DialogsRoot { get; private set; }
    }
}