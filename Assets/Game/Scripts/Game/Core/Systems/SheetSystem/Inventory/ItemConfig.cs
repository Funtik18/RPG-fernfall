using UnityEngine;

namespace Game.Core.Systems.SheetSystem
{
    [ CreateAssetMenu( fileName = "Item", menuName = "Game/Sheet/Inventory/Item" ) ]
    public class ItemConfig : ScriptableObject
    {
        [ field: SerializeField ] public string UID { get; private set; }
        [ field: SerializeField ] public string Name { get; private set; }
        [ field: SerializeField ] public string NameId { get; private set; }
        [ field: SerializeField ] public Sprite Icon { get; private set; }
        [ field: Space ]
        [ field: SerializeField ] public Vector2 WorldSize { get; private set; } = Vector2.one;
        [ field: Space ]
        [ field: Sirenix.OdinInspector.PropertyOrder( 999 ) ]
        [ field: SerializeField ] public int Cost { get; private set; } = 1;
        [ field: Sirenix.OdinInspector.PropertyOrder( 999 ) ]
        [ field: SerializeField ] public int Weight { get; private set; } = 1;
    }
}