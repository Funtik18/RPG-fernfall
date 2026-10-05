using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class GridEffects : MonoBehaviour
    {
        [ field: SerializeField ] public List< GridCellEffect > CellDefinitions { get; private set; } = new();

        public GridCellEffect GetDefinition( GridCellObject cell )
        {
            return CellDefinitions?.FirstOrDefault( x => x != null && x.Cell == cell );
        }

        public GridCellEffect AddDefinition( GridCellObject cell )
        {
            if ( cell == null )
            {
                return null;
            }

            CellDefinitions ??= new List< GridCellEffect >();

            var definition = GetDefinition( cell );
            if ( definition != null )
            {
                return definition;
            }

            definition = new GridCellEffect();
            definition.SetCell( cell );

            CellDefinitions.Add( definition );

            return definition;
        }

        public void RemoveDefinition( GridCellObject cell )
        {
            CellDefinitions?.RemoveAll( x => x == null || x.Cell == cell );
        }
    }
}
