using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace InfiniteCity.Model.Interfaces
{
    public interface IBoard
    {
        /// <summary>
        ///   Gets all the empty spaces of the board with an adjacent tile.
        /// </summary>
        IEnumerable<ISpace> Edges { get; }

        /// <summary>
        ///   Gets all the tiles currently placed on the board.
        /// </summary>
        IEnumerable<ITile> Tiles { get; }

        /// <summary>
        ///   Gets the space at the given point, creating it and adding it to the board if it has not been already.
        /// </summary>
        ISpace this[int x, int y] { get; }

        /// <summary>
        ///   Gets the space at the given point, creating it and adding it to the board if it has not been already.
        /// </summary>
        ISpace this[Point point] { get; }

        /// <summary>
        ///   Gets all the tiles of a certain type currently placed on the board.
        /// </summary>
        IEnumerable<ITile> Find<T>() where T: ITile;

        /// <summary>
        ///   Gets the space at the given point, returning false if it has not been already created and added to the board.
        /// </summary>
        bool TryGetSpace(int x, int y, out ISpace space);

        /// <summary>
        ///   Gets the space at the given point, returning false if it has not been already created and added to the board.
        /// </summary>
        bool TryGetSpace(Point point, out ISpace space);
    }
}