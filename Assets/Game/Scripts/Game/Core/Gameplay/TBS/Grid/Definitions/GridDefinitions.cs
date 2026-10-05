using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class GridDefinitions : MonoBehaviour
    {
        [ field: SerializeField ] public List< GridCellDefinition > CellDefinitions { get; private set; } = new();

        public GridCellDefinition GetDefinition( GridCellObject cell )
        {
            return CellDefinitions.FirstOrDefault( x => x.Cell == cell );
        }

        public void SetMarker( GridCellObject cell, UnitConfig config )
        {
            var definition = GetDefinition( cell );
            if ( definition == null )
            {
                definition = new GridCellDefinition();
                definition.SetCell( cell );

                CellDefinitions.Add( definition );
            }
            definition.SetCharacter( config );
        }

        public void RemoveDefinition( GridCellObject cell )
        {
            CellDefinitions.RemoveAll( x => x.Cell == cell );
        }
    }
}