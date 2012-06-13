using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Interfaces;
using Microsoft.Xna.Framework;

namespace InfiniteCity.Model
{
    internal class Board : IBoard
    {
        private static readonly int[,] Offsets = new[,] {{0, 1}, {1, 1}, {1, 0}, {0, -1}, {-1, -1}, {-1, 0}};
        private readonly IDictionary<Point, Space> _spaces = new Dictionary<Point, Space>();
        private readonly object _lock = new object();

        /// <summary>
        ///   Gets all the empty spaces of the board with an adjacent tile.
        /// </summary>
        public IEnumerable<Space> Edges
        {
            get
            {
                lock (_lock)
                {
                    var edges = new HashSet<Space>();
                    foreach (Space space in
                        _spaces.Values.Where(space => space != null && space.Tile != null).ToArray().SelectMany(
                            space => space.AdjacentSpaces.Where(edge => edge.Tile == null)))
                        edges.Add(space);
                    return edges;
                }
            }
        }

        /// <summary>
        ///   Gets all the tiles currently placed on the board.
        /// </summary>
        public IEnumerable<Tile> Tiles
        {
            get
            {
                lock (_lock)
                    return _spaces.Values.Where(space => space != null && space.Tile != null).Select(space => space.Tile).ToArray();
            }
        }

        /// <summary>
        ///   Gets all the empty spaces of the board with an adjacent tile.
        /// </summary>
        IEnumerable<ISpace> IBoard.Edges
        {
            get { return Edges; }
        }

        /// <summary>
        ///   Gets all the tiles currently placed on the board.
        /// </summary>
        IEnumerable<ITile> IBoard.Tiles
        {
            get { return Tiles; }
        }

        /// <summary>
        ///   Gets the space at the given point, creating it and adding it to the board if it has not been already.
        /// </summary>
        public Space this[int x, int y]
        {
            get { return this[new Point(x, y)]; }
        }

        /// <summary>
        ///   Gets the space at the given point, creating it and adding it to the board if it has not been already.
        /// </summary>
        public Space this[Point point]
        {
            get
            {
                lock (_lock)
                {
                    Space value;
                    if (!TryGetSpace(point, out value))
                    {
                        value = new Space(this, point);
                        _spaces[point] = value;
                    }
                    return value;
                }
            }
        }

        /// <summary>
        ///   Gets the space at the given point, creating it and adding it to the board if it has not been already.
        /// </summary>
        ISpace IBoard.this[int x, int y]
        {
            get { return this[x, y]; }
        }

        /// <summary>
        ///   Gets the space at the given point, creating it and adding it to the board if it has not been already.
        /// </summary>
        ISpace IBoard.this[Point point]
        {
            get { return this[point]; }
        }

        /// <summary>
        ///   Enumerates over the points adjacent to the given location.
        /// </summary>
        public static IEnumerable<Point> Adjacencies(Point point)
        {
            for (int i = 0; i<Offsets.GetLength(0); i++)
                yield return new Point(point.X+Offsets[i, 0], point.Y+Offsets[i, 1]);
        }

        /// <summary>
        ///   Enumerates over the points adjacent to the given location.
        /// </summary>
        public static IEnumerable<Point> Adjacencies(int x, int y)
        {
            for (int i = 0; i<Offsets.GetLength(0); i++)
                yield return new Point(x+Offsets[i, 0], y+Offsets[i, 1]);
        }

        /// <summary>
        ///   Gets all the tiles of a certain type currently placed on the board.
        /// </summary>
        public IEnumerable<ITile> Find<T>() where T: ITile
        {
            return Tiles.Where(tile => tile is T);
        }

        /// <summary>
        ///   Gets the space at the given point, returning false if it has not been already created and added to the board.
        /// </summary>
        public bool TryGetSpace(int x, int y, out Space space)
        {
            return TryGetSpace(new Point(x, y), out space);
        }

        /// <summary>
        ///   Gets the space at the given point, returning false if it has not been already created and added to the board.
        /// </summary>
        public bool TryGetSpace(Point point, out Space space)
        {
            return _spaces.TryGetValue(point, out space);
        }

        /// <summary>
        ///   Gets the space at the given point, returning false if it has not been already created and added to the board.
        /// </summary>
        bool IBoard.TryGetSpace(int x, int y, out ISpace space)
        {
            return (this as IBoard).TryGetSpace(new Point(x, y), out space);
        }

        /// <summary>
        ///   Gets the space at the given point, returning false if it has not been already created and added to the board.
        /// </summary>
        bool IBoard.TryGetSpace(Point point, out ISpace space)
        {
            Space instance;
            bool result = TryGetSpace(point, out instance);
            space = instance;
            return result;
        }
    }
}