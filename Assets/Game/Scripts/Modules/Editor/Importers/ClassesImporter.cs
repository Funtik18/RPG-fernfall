using Game.Core.Systems.SheetSystem;
using Modules.Extensions;
using SoosvetGames.CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Modules.Editor
{
    public static class ClassesImporter
    {
        public static void Import( GoogleSheetTable table )
        {
            var weaponsFamilies = AssetDatabaseUtils.LoadAssets< WeaponFamilyConfig >();
            var classesByUid = LoadClassesByUid();
            var skillsByUid = LoadSkillsByUid();
            List< string > completed = new();
            foreach ( var row in table.Rows )
            {
                var uid = row.Get( "UID" ).Trim();
                if ( string.IsNullOrWhiteSpace( uid ) )
                {
                    Debug.LogError( "[ClassesImporter] Row skipped because UID is empty." );
                    continue;
                }

                if ( !classesByUid.TryGetValue( uid, out _ ) )
                {
                    Debug.LogError( $"[ClassesImporter] Config with UID '{uid}' was not found." );
                    continue;
                }

                UpdateConfig( classesByUid[ uid ], weaponsFamilies, skillsByUid, row );
                completed.Add( uid );
            }

            foreach ( var uid in classesByUid.Keys )
            {
                if( completed.Contains( uid ) ) continue;
                Debug.LogError( $"[ClassesImporter] Config with UID '{uid}' was not found in the table." );
            }

            AssetDatabase.SaveAssets();
        }

        private static Dictionary< string, ClassConfig > LoadClassesByUid()
        {
            var result = new Dictionary< string, ClassConfig >();
            var configs = AssetDatabaseUtils.LoadAssets< ClassConfig >();

            foreach ( var config in configs )
            {
                if ( config == null || string.IsNullOrWhiteSpace( config.UID ) ) continue;

                var uid = config.UID.Trim();
                if ( !result.TryGetValue( uid, out _ ) )
                {
                    result.Add( uid, config );
                }
            }

            return result;
        }

        private static Dictionary< string, SkillConfig > LoadSkillsByUid()
        {
            var result = new Dictionary< string, SkillConfig >();
            var configs = AssetDatabaseUtils.LoadAssets< SkillConfig >();

            foreach ( var config in configs )
            {
                if ( config == null || string.IsNullOrWhiteSpace( config.UID ) ) continue;

                var uid = config.UID.Trim();
                if ( !result.TryGetValue( uid, out _ ) )
                {
                    result.Add( uid, config );
                }
            }

            return result;
        }

        private static void UpdateConfig( ClassConfig config, WeaponFamilyConfig[] families, Dictionary< string, SkillConfig > skills, GoogleSheetRow row )
        {
            var serializedObject = new SerializedObject( config );

            var name = row.Get( "Name" ).Trim();

            var healthPoints = row.GetInt( "HP base" );

            var strength = row.GetInt( "Strength base" );
            var dexterity = row.GetInt( "Dexterity base" );
            var speed = row.GetInt( "Speed base" );
            var luck = row.GetInt( "Luck base" );

            var defense = row.GetInt( "Defense base" );
            var resist = row.GetInt( "Resist base" );
            var craft = row.GetInt( "Craft base" );

            var movePoints = row.GetInt( "Movement" );

            var familyTypeName = row.Get( "Weapons Family" ).Trim();
            var family = families.FirstOrDefault( (f) => f.Name == familyTypeName );
            if ( family == null )
            {
                Debug.LogError( $"[ClassesImporter] Row skipped because Family '{familyTypeName}' is not found." );
                family = families.First( ( f ) => f.Name == "Unarmed" );
            }

            serializedObject.SetString( name, nameof( ClassConfig.Name ) );

            var stats = serializedObject.FindBackingField( nameof( ClassConfig.Stats ) );
            stats.SetInt( healthPoints, nameof( StatsSettings.HealthPoints ) );
            stats.SetInt( strength, nameof( StatsSettings.Strength ) );
            stats.SetInt( dexterity, nameof( StatsSettings.Dexterity ) );
            stats.SetInt( speed, nameof( StatsSettings.Speed ) );
            stats.SetInt( luck, nameof( StatsSettings.Luck ) );
            stats.SetInt( defense, nameof( StatsSettings.Defense ) );
            stats.SetInt( resist, nameof( StatsSettings.Resist ) );
            stats.SetInt( craft, nameof( StatsSettings.Craft ) );
            stats.SetInt( movePoints, nameof( StatsSettings.MoveRange ) );

            serializedObject.SetObject( family, nameof( ClassConfig.WeaponsFamily ) );
            SetSkills( serializedObject, row.Get( "Skills" ), skills, config.UID );

            serializedObject.ApplyModifiedProperties();

            config.RenameConfigAsset( name );
            EditorUtility.SetDirty( config );
        }

        private static void SetSkills( SerializedObject serializedObject, string skillsNames, Dictionary< string, SkillConfig > skillsByUid, string classUid )
        {
            var skillsSettings = serializedObject.FindBackingField( nameof( ClassConfig.Skills ) );
            var skills = skillsSettings.FindBackingFieldRelative( nameof( SkillsSettings.Skills ) );
            skills.ClearArray();
            if ( string.IsNullOrWhiteSpace( skillsNames ) ) return;

            foreach ( var rawSkillData in skillsNames.Split( ',' ) )
            {
                var skillData = rawSkillData.Trim();
                if ( string.IsNullOrWhiteSpace( skillData ) ) continue;

                var level = 0;
                var levelStartIndex = skillData.IndexOf( '(', StringComparison.Ordinal );
                var uid = levelStartIndex >= 0 ? skillData.Substring( 0, levelStartIndex ).Trim() : skillData;

                if ( levelStartIndex >= 0 )
                {
                    var levelEndIndex = skillData.IndexOf( ')', levelStartIndex + 1 );
                    if ( levelEndIndex < 0 )
                    {
                        Debug.LogError( $"[ClassesImporter] Skill '{skillData}' for Class '{classUid}' has no closing level bracket." );
                    }
                    else
                    {
                        var levelText = skillData.Substring( levelStartIndex + 1, levelEndIndex - levelStartIndex - 1 ).Trim();
                        if ( !int.TryParse( levelText, out level ) )
                        {
                            Debug.LogError( $"[ClassesImporter] Skill '{uid}' for Class '{classUid}' has invalid Level '{levelText}'." );
                        }
                    }
                }

                if ( string.IsNullOrWhiteSpace( uid ) ) continue;

                if ( !skillsByUid.TryGetValue( uid, out var skillConfig ) )
                {
                    Debug.LogError( $"[ClassesImporter] Skill config with UID '{uid}' for Class '{classUid}' was not found." );
                    continue;
                }

                var index = skills.arraySize;
                skills.InsertArrayElementAtIndex( index );
                var skill = skills.GetArrayElementAtIndex( index );
                skill.SetInt( level, nameof( SkillRequiredLevel.Level ) );
                skill.FindBackingFieldRelative( nameof( SkillRequiredLevel.Skill ) ).objectReferenceValue = skillConfig;
            }
        }
    }
}
