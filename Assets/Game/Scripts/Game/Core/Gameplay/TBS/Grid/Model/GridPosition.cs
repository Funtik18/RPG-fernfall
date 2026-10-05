using System;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    [ Serializable ]
    public struct GridPosition : IEquatable< GridPosition >
    {
        [ SerializeField ] private int _x;
        [ SerializeField ] private int _y;
        [ SerializeField ] private int _z;

        public int X => _x;
        public int Y => _y;
        public int Z => _z;

        public GridPosition( int x, int y, int z )
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public bool Equals( GridPosition other ) => _x == other._x && _y == other._y && _z == other._z;

        public override bool Equals( object obj ) => obj is GridPosition other && Equals( other );

        public override int GetHashCode() => HashCode.Combine( _x, _y, _z );

        public override string ToString() => $"({_x}, {_y}, {_z})";

        public static bool operator ==( GridPosition left, GridPosition right ) => left.Equals( right );

        public static bool operator !=( GridPosition left, GridPosition right ) => !left.Equals( right );
    }
}