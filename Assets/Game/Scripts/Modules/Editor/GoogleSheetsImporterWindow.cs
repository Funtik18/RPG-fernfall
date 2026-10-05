using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Modules.Editor
{
    public sealed class GoogleSheetsImporterWindow : EditorWindow
    {
        private readonly GoogleSheetsClient _client = new();

        [ SerializeField ] private GoogleSheetConfig _config;

        private IReadOnlyList< GoogleSheetInfo > _sheets = Array.Empty< GoogleSheetInfo >();

        private int _selectedSheetIndex;

        private bool _isBusy;

        private GoogleSheetTable _lastTable;

        [ MenuItem( "Tools/Data/Google Sheets Importer" ) ]
        private static void Open()
        {
            GetWindow< GoogleSheetsImporterWindow >( "Google Sheets" );
        }

        private void OnEnable()
        {
            _config ??= FindConfig();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField( "Google Sheets Importer", EditorStyles.boldLabel );
            EditorGUILayout.Space();

            DrawSettings();

            EditorGUILayout.Space();

            using ( new EditorGUI.DisabledScope( _isBusy ) )
            {
                if ( GUILayout.Button( "Load Sheets" ) )
                    LoadSheets();

                EditorGUILayout.Space();

                DrawSheetSelector();
            }

            EditorGUILayout.Space();

            DrawResult();
        }

        private void DrawSettings()
        {
            _config = ( GoogleSheetConfig )EditorGUILayout.ObjectField( "Config", _config, typeof( GoogleSheetConfig ), false );

            if ( _config == null )
            {
                EditorGUILayout.HelpBox( "Assign GoogleSheetConfig.", MessageType.Warning );
            }
        }

        private void DrawSheetSelector()
        {
            if ( _sheets.Count == 0 )
            {
                EditorGUILayout.HelpBox( "Press Load Sheets to receive available sheets.", MessageType.Info );

                return;
            }

            var options = _sheets.Select( x => x.Name ).ToArray();
            _selectedSheetIndex = EditorGUILayout.Popup( "Sheet", _selectedSheetIndex, options );

            if ( GUILayout.Button( "Import Selected Sheet" ) )
            {
                ImportSelectedSheet();
            }
        }

        private void DrawResult()
        {
            if ( _isBusy )
            {
                EditorGUILayout.HelpBox( "Loading...", MessageType.Info );
                return;
            }
            if ( _lastTable == null ) return;

            EditorGUILayout.LabelField( $"Imported rows: {_lastTable.Rows.Count}", EditorStyles.boldLabel );
            EditorGUILayout.LabelField( $"Columns: {string.Join( ", ", _lastTable.Headers )}" );
        }

        private async void LoadSheets()
        {
            if ( _config == null )
            {
                Debug.LogError( "GoogleSheetConfig is not assigned." );
                return;
            }

            if ( string.IsNullOrWhiteSpace( _config.SpreadsheetId ) )
            {
                Debug.LogError( "Spreadsheet ID is empty." );
                return;
            }

            if ( string.IsNullOrWhiteSpace( _config.ApiKey ) )
            {
                Debug.LogError( "API Key is empty." );
                return;
            }

            _isBusy = true;

            try
            {
                _sheets = await _client.GetSheets( _config.SpreadsheetId, _config.ApiKey );

                _selectedSheetIndex = 0;

                Debug.Log( $"[Google Sheets] Loaded {_sheets.Count} sheets." );
            }
            catch ( Exception exception )
            {
                Debug.LogException( exception );
            }
            finally
            {
                _isBusy = false;
                Repaint();
            }
        }

        private async void ImportSelectedSheet()
        {
            if ( _config == null )
            {
                Debug.LogError( "GoogleSheetConfig is not assigned." );
                return;
            }

            if ( _selectedSheetIndex < 0 || _selectedSheetIndex >= _sheets.Count ) return;
            _isBusy = true;

            try
            {
                var sheet = _sheets[ _selectedSheetIndex ];
                var data = await _client.GetSheet( _config.SpreadsheetId, _config.ApiKey, sheet.Name );

                _lastTable = GoogleSheetTableParser.Parse( data );

                Debug.Log( $"[Google Sheets] Imported '{sheet.Name}'. " + $"Rows: {_lastTable.Rows.Count}" );

                Parse( sheet.Name, _lastTable );
            }
            catch ( Exception exception )
            {
                Debug.LogException( exception );
            }
            finally
            {
                _isBusy = false;
                Repaint();
            }
        }

        private void Parse( string sheetName, GoogleSheetTable table )
        {
            Debug.Log( $"Parsing sheet: {sheetName}" );
            if ( sheetName == "Classes" )
            {
                ClassesImporter.Import( table );
            }
            else if ( sheetName == "Weapons" )
            {
                WeaponsImporter.Import( table );
            }
            else if ( sheetName == "WeaponsTriangle" )
            {
                WeaponsTriangleImporter.Import( table );
            }
            else if ( sheetName == "CombatArts" )
            {
                CombatArtsImport.Import( table );
            }
            else if ( sheetName == "ClassSkills" )
            {
                ClassSkillsImporter.Import( table );
            }
            else if ( sheetName == "PersonalSkills" )
            {
                PersonalSkillsImporter.Import( table );
            }
            else if( sheetName == "Terrain" )
            {
                TerrainImport.Import( table );
            }
            else if ( sheetName == "Fury" )
            {
                FuryImporter.Import( table );
            }
            else if ( sheetName == "Constants" )
            {
                ConstantsImporter.Import( table );
            }
            else if( sheetName == "Effects" )
            {
                EffectsImport.Import( table );
            }
        }

        private static GoogleSheetConfig FindConfig()
        {
            var guids = AssetDatabase.FindAssets( "t:GoogleSheetConfig" );

            if ( guids.Length == 0 ) return null;

            var path = AssetDatabase.GUIDToAssetPath( guids[ 0 ] );

            return AssetDatabase.LoadAssetAtPath< GoogleSheetConfig >( path );
        }
    }
}
