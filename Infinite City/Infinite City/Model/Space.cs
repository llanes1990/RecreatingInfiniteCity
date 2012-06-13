using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Interfaces;
using Microsoft.Xna.Framework;

namespace InfiniteCity.Model
{
    internal sealed class Space : ISpace
    {
        private readonly Board _board;
        private Tile _tile;

        internal Space(Board board, Point location)
        {
            _board = board;
            Location = location;
        }

        internal Space()
        {
            _board = null;
            Location = new Point(-1, -1);
        }

        /// <summary>
        ///   Gets all the spaces adjacent to this one.
        /// </summary>
        public IEnumerable<Space> AdjacentSpaces
        {
            get { return Board.Adjacencies(Location).Select(p => _board[p]); }
        }

        /// <summary>
        ///   Gets all the tiles adjacent to this space.
        /// </summary>
        public IEnumerable<Tile> AdjacentTiles
        {
            get
            {
                foreach (Point point in Board.Adjacencies(Location))
                {
                    Space space;
                    if (_board.TryGetSpace(point, out space) && space.Tile != null)
                        yield return space.Tile;
                }
            }
        }

        /// <summary>
        ///   Gets the board this space is on.
        /// </summary>
        public Board Board
        {
            get { return _board; }
        }

        /// <summary>
        ///   Gets the location of this space.
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        ///   Gets the tile placed on this space.
        /// </summary>
        public Tile Tile
        {
            get { return _tile; }
            set
            {
                if (ReferenceEquals(_tile, value))
                    return;
                Tile old = _tile;
                _tile = value;
                if (old != null)
                    old.Space = null;
                if (_tile != null)
                    _tile.Space = this;
                Debug.Assert(_tile == null || ReferenceEquals(this, _tile.Space));
            }
        }

        /// <summary>
        ///   Gets all the spaces adjacent to this one.
        /// </summary>
        IEnumerable<ISpace> ISpace.AdjacentSpaces
        {
            get { return AdjacentSpaces; }
        }

        /// <summary>
        ///   Gets all the tiles adjacent to this space.
        /// </summary>
        IEnumerable<ITile> ISpace.AdjacentTiles
        {
            get { return AdjacentTiles; }
        }

        /// <summary>
        ///   Gets the board this space is on.
        /// </summary>
        IBoard ISpace.Board
        {
            get { return Board; }
        }

        /// <summary>
        ///   Gets the tile placed on this space.
        /// </summary>
        ITile ISpace.Tile
        {
            get { return Tile; }
        }

        /// <summary>
        ///   Determines whether or not this space is adjacent to the given point.
        /// </summary>
        public bool IsAdjacentTo(Point point)
        {
            return Board.Adjacencies(Location).Contains(point);
        }

        /// <summary>
        ///   Determines whether or not this space is adjacent to the given point.
        /// </summary>
        public bool IsAdjacentTo(int x, int y)
        {
            return IsAdjacentTo(new Point(x, y));
        }

        /// <summary>
        ///   Determines whether or not this space is adjacent to the given tile.
        /// </summary>
        public bool IsAdjacentTo(ITile tile)
        {
            return tile.Space != null && IsAdjacentTo(tile.Space);
        }

        /// <summary>
        ///   Determines whether or not this space is adjacent to the given space.
        /// </summary>
        public bool IsAdjacentTo(ISpace space)
        {
            return IsAdjacentTo(space.Location);
        }

        /// <summary>
        ///   Determines whether or not this space is adjacent to the given type of tile.
        /// </summary>
        public bool IsAdjacentTo<T>() where T: ITile
        {
            return AdjacentTiles.Any(tile => tile is T);
        }
    }
}