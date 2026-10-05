using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class GridCellObject : MonoBehaviour
    {
        public event Action< GridCellObject > OnPointerClicked;
        public event Action< GridCellObject > OnPointerEntered;
        public event Action< GridCellObject > OnPointerExited;
        
        [ field: Sirenix.OdinInspector.Title( "Grid" ) ]
        [ field: SerializeField, Sirenix.OdinInspector.ReadOnly ] public GridPosition Position { get; private set; }
        [ field: SerializeField ] public List< GridCellObject > Connections { get; private set; } = new();
        [ field: SerializeField ] public GridCellTerrainConfig Terrain { get; private set; }
        [ Sirenix.OdinInspector.Title( "Visual" ) ]
        [ SerializeField ] private GameObject _common;
        [ SerializeField ] private GameObject _hover;
        [ SerializeField ] private GameObject _reach;
        [ SerializeField ] private GameObject _attack;
        
        public void SetPosition( GridPosition position )
        {
            Position = position;
        }

        public void SetConnections( IEnumerable< GridCellObject > connections )
        {
            Connections.Clear();
            Connections.AddRange( connections );
        }

        public void EnableHover( bool trigger )
        {
            _hover.SetActive( trigger );
        }

        public void EnableSelect( bool trigger )
        {
            _common.SetActive( !trigger );
            _reach.SetActive( trigger );
        }
        
        public void EnableAttack( bool trigger )
        {
            _common.SetActive( !trigger );
            _attack.SetActive( trigger );
        }

        public void OnPointerClick()
        {
            OnPointerClicked?.Invoke( this );
        }

        public void OnPointerEnter()
        {
            OnPointerEntered?.Invoke( this );
        }

        public void OnPointerExit()
        {
            OnPointerExited?.Invoke( this );
        }

        [ Sirenix.OdinInspector.Button( DirtyOnClick = true ) ]
        private void Up()
        {
            Position = new GridPosition( Position.X, Position.Y + 1, Position.Z );
            transform.position = new Vector3( transform.position.x, transform.position.y + 1f, transform.position.z );
        }
        
        [ Sirenix.OdinInspector.Button( DirtyOnClick = true ) ]
        private void Down()
        {
            Position = new GridPosition( Position.X, Position.Y - 1, Position.Z );
            transform.position = new Vector3( transform.position.x, transform.position.y - 1f, transform.position.z );
        }
    }
}