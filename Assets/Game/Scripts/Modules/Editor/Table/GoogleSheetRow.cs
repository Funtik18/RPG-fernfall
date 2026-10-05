using System.Collections.Generic;

namespace Modules.Editor
{
    public sealed class GoogleSheetRow
    {
        private readonly Dictionary< string, string > _values;

        public GoogleSheetRow( Dictionary< string, string > values )
        {
            _values = values;
        }

        public string Get( string column ) => _values.TryGetValue( column, out var value ) ? value : string.Empty;

        public int GetInt( string column ) => int.TryParse( Get( column ), out var result ) ? result : 0;

        public float GetFloat( string column ) => float.TryParse( Get( column ), out var result ) ? result : 0f;

        public bool GetBool( string column ) => bool.TryParse( Get( column ), out var result ) && result;
    }
}