using System;
using System.Collections.Generic;

namespace Modules.Editor
{
    public static class GoogleSheetTableParser
    {
        public static GoogleSheetTable Parse( IReadOnlyList< IReadOnlyList< string > > data )
        {
            if ( data == null || data.Count == 0 )
            {
                return new GoogleSheetTable( Array.Empty< string >(), Array.Empty< GoogleSheetRow >() );
            }

            var headers = new List< string >( data[ 0 ] );
            var rows = new List< GoogleSheetRow >();

            for ( var rowIndex = 1; rowIndex < data.Count; rowIndex++ )
            {
                var sourceRow = data[ rowIndex ];
                if ( IsEmpty( sourceRow ) ) continue;

                var values = new Dictionary< string, string >( StringComparer.OrdinalIgnoreCase );
                for ( var columnIndex = 0; columnIndex < headers.Count; columnIndex++ )
                {
                    var header = headers[ columnIndex ];
                    if ( string.IsNullOrWhiteSpace( header ) ) continue;
                    values[ header ] = columnIndex < sourceRow.Count ? sourceRow[ columnIndex ] : string.Empty;
                }

                rows.Add( new GoogleSheetRow( values ) );
            }

            return new GoogleSheetTable( headers, rows );
        }

        private static bool IsEmpty( IReadOnlyList< string > row )
        {
            for ( var i = 0; i < row.Count; i++ )
            {
                if ( !string.IsNullOrWhiteSpace( row[ i ] ) )
                {
                    return false;
                }
            }

            return true;
        }
    }
}