using Game.Core.Gameplay;
using Game.Core.Gameplay.Party;
using Game.Core.Systems.SheetSystem;
using Game.Systems.StorageSystem;
using System;

namespace Game.Managers.PartyManager
{
    public sealed class PartyManager
    {
        public Party Party
        {
            get
            {
                if ( _party == null )
                {
                    CreateParty();
                }
                return _party;
            }
        }
        private Party _party;

        private readonly PartyGameplayConfig _config;
        private readonly SheetFactory _sheetFactory;
        private readonly CampaignManager.CampaignManager _campaignManager;
        
        public PartyManager(
            PartyGameplayConfig config,
            SheetFactory sheetFactory,
            CampaignManager.CampaignManager campaignManager
            )
        {
            _config = config ?? throw new ArgumentNullException( nameof(config) );
            _sheetFactory = sheetFactory ?? throw new ArgumentNullException( nameof(sheetFactory) );
            _campaignManager = campaignManager ?? throw new ArgumentNullException( nameof(campaignManager) );
        }
        
        private void CreateParty()
        {
            _party = new();
            var partyData = _campaignManager.GetProgressData().Party ??= new();

            for ( int i = 0; i < _config.Sheets.Count; i++ )
            {
                var characterData = GetCharacterData( partyData, i );
                var inventoryData = characterData.Inventory;
                var sheet = inventoryData != null
                    ? _sheetFactory.Create( _config.Sheets[ i ], inventoryData )
                    : _sheetFactory.Create( _config.Sheets[ i ] );

                characterData.Inventory = sheet.Inventory.Data;

                PartyCharacter character = new( sheet );
                _party.Characters.Add( character );
            }
        }

        private static PartyCharacterData GetCharacterData( PartyData partyData, int index )
        {
            while ( partyData.Characters.Count <= index )
            {
                partyData.Characters.Add( new() );
            }

            return partyData.Characters[ index ];
        }
        
        public Sheet GetSheet( int index )
        {
            return Party.Characters[ index ].Sheet;
        }
    }
}
