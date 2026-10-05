using UnityEngine;

namespace Modules.Editor
{
    [ CreateAssetMenu( fileName = "GoogleSheetConfig", menuName = "GoogleSheetConfig" ) ]
    public sealed class GoogleSheetConfig : ScriptableObject
    {
        [ field: SerializeField ] public string SpreadsheetId { get; private set; }
        [ field: SerializeField ] public string ApiKey { get; private set; }
    }
}
