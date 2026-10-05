using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace Modules.Editor
{
    public sealed class GoogleSheetsClient
    {
        private const string BaseUrl = "https://sheets.googleapis.com/v4/spreadsheets";

        public async Task< IReadOnlyList< GoogleSheetInfo > > GetSheets( string spreadsheetId, string apiKey )
        {
            var url = $"{BaseUrl}/{spreadsheetId}" + $"?fields=sheets.properties(sheetId,title)" + $"&key={apiKey}";
            var json = await Get( url );
            var root = JObject.Parse( json );
            var result = new List< GoogleSheetInfo >();
            var sheets = root[ "sheets" ] as JArray;

            if ( sheets == null ) return result;

            foreach ( var sheet in sheets )
            {
                var properties = sheet[ "properties" ];

                if ( properties == null )
                    continue;

                result.Add( new GoogleSheetInfo { Id = properties[ "sheetId" ]?.Value< int >() ?? 0, Name = properties[ "title" ]?.Value< string >() } );
            }

            return result;
        }

        public async Task< IReadOnlyList< IReadOnlyList< string > > > GetSheet( string spreadsheetId, string apiKey, string sheetName )
        {
            // Экранируем ' внутри имени листа по правилам A1 notation.
            var escapedSheetName = sheetName.Replace( "'", "''" );
            // Получаем весь используемый диапазон листа.
            var range = $"'{escapedSheetName}'";
            var encodedRange = Uri.EscapeDataString( range );
            var url = $"{BaseUrl}/{spreadsheetId}/values/{encodedRange}" + $"?majorDimension=ROWS" + $"&valueRenderOption=UNFORMATTED_VALUE" + $"&key={apiKey}";
            var json = await Get( url );
            var root = JObject.Parse( json );
            var values = root[ "values" ] as JArray;
            var result = new List< IReadOnlyList< string > >();

            if ( values == null ) return result;

            foreach ( var rowToken in values )
            {
                if ( rowToken is not JArray rowArray ) continue;

                var row = new List< string >();
                foreach ( var cell in rowArray )
                {
                    row.Add( cell?.ToString() ?? string.Empty );
                }

                result.Add( row );
            }

            return result;
        }

        private static async Task< string > Get( string url )
        {
            using var request = UnityWebRequest.Get( url );
            var operation = request.SendWebRequest();

            while ( operation.isDone == false ) await Task.Yield();

            if ( request.result != UnityWebRequest.Result.Success )
            {
                throw new Exception( $"Google Sheets request failed.\n" + $"Code: {request.responseCode}\n" + $"Error: {request.error}\n" + $"Response: {request.downloadHandler.text}" );
            }

            return request.downloadHandler.text;
        }
    }
}