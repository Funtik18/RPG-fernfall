using Game.Utils;
using UnityEngine;

namespace Game.Managers.SceneManager
{
    [ System.Serializable ]
    public sealed class SceneSettings
    {
        [ field: SerializeField ] public SceneReference MainMenuScene { get; private set; }
        [ field: SerializeField ] public SceneReference GameplayScene { get; private set; }
        [ field: SerializeField ] public SceneReference CampScene { get; private set; }
        [ field: SerializeField ] public SceneReference BattleScene { get; private set; }
    }
}