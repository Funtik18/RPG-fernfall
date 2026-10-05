using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Gameplay.TBS
{
    public sealed class BattlePathfinder
    {
        private readonly BattleGridController _gridController;

        public BattlePathfinder( BattleGridController gridController )
        {
            _gridController = gridController ?? throw new ArgumentNullException( nameof(gridController) );
        }

        public IReadOnlyList< BattleCell > FindPath( BattleCell start, BattleCell target, UnitController unit, int maxMovementPoints = int.MaxValue )
        {
            if ( start == null ) throw new ArgumentNullException( nameof(start) );
            if ( target == null ) throw new ArgumentNullException( nameof(target) );
            if ( unit == null ) throw new ArgumentNullException( nameof(unit) );

            if ( start == target )
            {
                return Array.Empty< BattleCell >();
            }

            var startState = new PathState( start, Vector3Int.zero );
            var open = new List< PathState > { startState };
            var cameFrom = new Dictionary< PathState, PathState >();
            var scores = new Dictionary< PathState, PathScore > { [ startState ] = new PathScore( 0, 0 ) };

            while ( open.Count > 0 )
            {
                var current = GetLowestScore( open, scores );

                if ( current.Cell == target )
                {
                    return BuildPath( cameFrom, current, startState );
                }

                open.Remove( current );

                foreach ( var connection in current.Cell.Connections )
                {
                    if ( !CanTraverse( unit, current.Cell, connection ) )
                    {
                        continue;
                    }

                    var neighbour = connection.Cell;
                    var direction = GetDirection( current.Cell, neighbour );
                    var movementCost = GetMovementCost( unit, current.Cell, connection );
                    var turnCost = GetTurnCost( current.Direction, direction );
                    var currentScore = scores[ current ];

                    var newMovement = currentScore.Movement + movementCost;

                    if ( newMovement > maxMovementPoints )
                    {
                        continue;
                    }

                    var newScore = new PathScore( newMovement, currentScore.Turns + turnCost );
                    var nextState = new PathState( neighbour, direction );

                    if ( scores.TryGetValue( nextState, out var existingScore ) && !IsBetter( newScore, existingScore ) )
                    {
                        continue;
                    }

                    cameFrom[ nextState ] = current;
                    scores[ nextState ] = newScore;

                    if ( !open.Contains( nextState ) )
                    {
                        open.Add( nextState );
                    }
                }
            }

            return Array.Empty< BattleCell >();
        }

        /// <summary>
        /// Все клетки, до которых юнит может добраться
        /// за указанное количество Move Points.
        /// </summary>
        public IReadOnlyCollection< BattleCell > GetReachableCells( BattleCell start, UnitController unit, int movementPoints )
        {
            if ( start == null ) throw new ArgumentNullException( nameof(start) );
            if ( unit == null ) throw new ArgumentNullException( nameof(unit) );

            if ( movementPoints <= 0 )
            {
                return Array.Empty< BattleCell >();
            }

            var open = new List< BattleCell > { start };
            var costs = new Dictionary< BattleCell, int > { [ start ] = 0 };
            var result = new HashSet< BattleCell >();

            while ( open.Count > 0 )
            {
                var current = GetLowestCost( open, costs );
                open.Remove( current );
                var currentCost = costs[ current ];

                foreach ( var connection in current.Connections )
                {
                    if ( !CanTraverse( unit, current, connection ) )
                    {
                        continue;
                    }

                    var neighbour = connection.Cell;
                    var stepCost = GetMovementCost( unit, current, connection );
                    var newCost = currentCost + stepCost;

                    if ( newCost > movementPoints )
                    {
                        continue;
                    }

                    if ( costs.TryGetValue( neighbour, out var oldCost ) && newCost >= oldCost )
                    {
                        continue;
                    }

                    costs[ neighbour ] = newCost;
                    result.Add( neighbour );

                    if ( !open.Contains( neighbour ) )
                    {
                        open.Add( neighbour );
                    }
                }
            }

            return result;
        }

        public IReadOnlyCollection< BattleCell > GetAttackCells( BattleCell start, UnitController unit, int attackPoints )
        {
            if ( start == null ) throw new ArgumentNullException( nameof(start) );
            if ( unit == null ) throw new ArgumentNullException( nameof(unit) );

            if ( attackPoints <= 0 )
            {
                return Array.Empty< BattleCell >();
            }

            var open = new List< BattleCell > { start };
            var costs = new Dictionary< BattleCell, int > { [ start ] = 0 };
            var result = new HashSet< BattleCell >();

            while ( open.Count > 0 )
            {
                var current = GetLowestCost( open, costs );
                open.Remove( current );
                var currentCost = costs[ current ];

                foreach ( var connection in current.Connections )
                {
                    if ( !CanUseForRange( current, connection ) )
                    {
                        continue;
                    }

                    var neighbour = connection.Cell;
                    var stepCost = GetRangeCost( current, neighbour );
                    var newCost = currentCost + stepCost;

                    if ( newCost > attackPoints )
                    {
                        continue;
                    }

                    if ( costs.TryGetValue( neighbour, out var oldCost ) && newCost >= oldCost )
                    {
                        continue;
                    }

                    costs[ neighbour ] = newCost;
                    result.Add( neighbour );

                    if ( !open.Contains( neighbour ) )
                    {
                        open.Add( neighbour );
                    }
                }
            }

            return result;
        }

        private bool CanTraverse( UnitController unit, BattleCell from, BattleCellConnection connection )
        {
            if ( connection == null ) return false;

            var to = connection.Cell;
            if ( to == null ) return false;
            if ( _gridController.Registry.IsOccupied( to ) ) return false;
            if ( !connection.IsGridLink && !from.Grid.CanWalk( from.View, to.View ) ) return false;

            return true;
        }

        private bool CanUseForRange( BattleCell from, BattleCellConnection connection )
        {
            if ( connection == null || connection.Cell == null ) return false;
            if ( !connection.IsGridLink ) return true;

            return from.Grid == connection.Cell.Grid && from.Grid.CanWalk( from.View, connection.Cell.View );
        }

        public int GetPathMovementCost( UnitController unit, IReadOnlyList< BattleCell > path, BattleCell start )
        {
            var cost = 0;
            var current = start;

            foreach ( var cell in path )
            {
                if ( !current.TryGetConnection( cell, out var connection ) ) return int.MaxValue;

                cost += GetMovementCost( unit, current, connection );

                current = cell;
            }

            return cost;
        }

        private int GetMovementCost( UnitController unit, BattleCell from, BattleCellConnection connection )
        {
            var to = connection.Cell;
            if ( connection.IsGridLink ) return to.MoveCost;

            var cost = GetRangeCost( from, to );
            cost += to.MoveCost - 1;

            return cost;
        }

        private int GetRangeCost( BattleCell from, BattleCell to )
        {
            var direction = GetDirection( from, to );
            var diagonal = direction.x != 0 && direction.z != 0;
            return diagonal ? 2 : 1;
        }

        private int GetTurnCost( Vector3Int previousDirection, Vector3Int newDirection )
        {
            //
            // Первый шаг поворотом не считается.
            //
            if ( previousDirection == Vector3Int.zero )
            {
                return 0;
            }

            return previousDirection == newDirection ? 0 : 1;
        }

        private Vector3Int GetDirection( BattleCell from, BattleCell to )
        {
            var delta = to.View.transform.position - from.View.transform.position;

            return new Vector3Int( GetAxisDirection( delta.x ), GetAxisDirection( delta.y ), GetAxisDirection( delta.z ) );
        }

        private int GetAxisDirection( float value )
        {
            const float epsilon = 0.001f;
            if ( Mathf.Abs( value ) <= epsilon ) return 0;
            return value > 0f ? 1 : -1;
        }

        private PathState GetLowestScore( List< PathState > states, Dictionary< PathState, PathScore > scores )
        {
            var best = states[ 0 ];
            var bestScore = scores[ best ];

            for ( var i = 1; i < states.Count; i++ )
            {
                var state = states[ i ];
                var score = scores[ state ];

                if ( !IsBetter( score, bestScore ) )
                {
                    continue;
                }

                best = state;
                bestScore = score;
            }

            return best;
        }

        private BattleCell GetLowestCost( List< BattleCell > cells, Dictionary< BattleCell, int > costs )
        {
            var best = cells[ 0 ];
            var bestCost = costs[ best ];

            for ( var i = 1; i < cells.Count; i++ )
            {
                var cell = cells[ i ];
                var cost = costs[ cell ];

                if ( cost >= bestCost )
                {
                    continue;
                }

                best = cell;
                bestCost = cost;
            }

            return best;
        }

        private bool IsBetter( PathScore left, PathScore right )
        {
            //
            // Главное — минимальная стоимость движения.
            //
            if ( left.Movement != right.Movement )
            {
                return left.Movement < right.Movement;
            }

            //
            // При одинаковой стоимости —
            // минимальное количество поворотов.
            //
            return left.Turns < right.Turns;
        }

        private IReadOnlyList< BattleCell > BuildPath( Dictionary< PathState, PathState > cameFrom, PathState current, PathState start )
        {
            var path = new List< BattleCell >();

            while ( !current.Equals( start ) )
            {
                path.Add( current.Cell );

                if ( !cameFrom.TryGetValue( current, out current ) )
                {
                    return Array.Empty< BattleCell >();
                }
            }

            path.Reverse();

            return path;
        }

        private readonly struct PathScore
        {
            public readonly int Movement;
            public readonly int Turns;

            public PathScore( int movement, int turns )
            {
                Movement = movement;
                Turns = turns;
            }
        }

        private readonly struct PathState : IEquatable< PathState >
        {
            public readonly BattleCell Cell;
            public readonly Vector3Int Direction;

            public PathState( BattleCell cell, Vector3Int direction )
            {
                Cell = cell;
                Direction = direction;
            }

            public bool Equals( PathState other )
            {
                return ReferenceEquals( Cell, other.Cell ) && Direction == other.Direction;
            }

            public override bool Equals( object obj )
            {
                return obj is PathState other && Equals( other );
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ( ( Cell != null ? Cell.GetHashCode() : 0 ) * 397 ) ^ Direction.GetHashCode();
                }
            }
        }
    }
}
