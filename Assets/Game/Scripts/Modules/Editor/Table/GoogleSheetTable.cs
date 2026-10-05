using System.Collections.Generic;

namespace Modules.Editor
{
    public sealed class GoogleSheetTable
    {
        public IReadOnlyList< string > Headers { get; }
        public IReadOnlyList< GoogleSheetRow > Rows { get; }

        public GoogleSheetTable( IReadOnlyList< string > headers, IReadOnlyList< GoogleSheetRow > rows )
        {
            Headers = headers;
            Rows = rows;
        }
    }
}